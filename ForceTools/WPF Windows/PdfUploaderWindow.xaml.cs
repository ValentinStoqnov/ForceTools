using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ForceTools.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ForceTools
{
    public partial class PdfUploaderWindow : Window, INotifyPropertyChanged
    {
        //The Main list for all uploaded invoices
        private MainListOfLists _MainList;
        public MainListOfLists MainList { get { return _MainList; } set { _MainList = value; OnPropertyChanged(); } }

        //List of Currently Selected Images
        List<InvoiceImage> selectedInvoiceImages = new List<InvoiceImage>();

        //Getting if the User is working on Purchases or Sales
        private OperationType OperationType;

        //Document drag/drop navigation Variables
        private Point startPoint = new Point();
        private int startIndex = -1;
        private int innerStartIndex = -1;
        private string ComingFrom = "";

        //Progress Bar Variables
        private int _ProgbarValue;
        public int ProgbarValue { get { return _ProgbarValue; } set { _ProgbarValue = value; OnPropertyChanged(); } }
        private int _ProgbarMaxmimum;
        public int ProgbarMaximum { get { return _ProgbarMaxmimum; } set { _ProgbarMaxmimum = value; OnPropertyChanged(); } }
        private string _ProgText;
        public string ProgbarText { get { return _ProgText; } set { _ProgText = value; OnPropertyChanged(); } }
        private bool _ProgbarPopupOpen;
        public bool ProgbarPopupOpen { get { return _ProgbarPopupOpen; } set { _ProgbarPopupOpen = value; OnPropertyChanged(); } }

        //Document Counter Variables
        private string _DocCounter;
        public string DocCounter { get { return _DocCounter; } set { _DocCounter = value; OnPropertyChanged(); } }

        //Brush Converter so I can use #colors
        private BrushConverter bc = new BrushConverter();

        //Rectangle for Automatic Scrolling
        System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();



        #region How to use the Progress Bar
        //ProgbarMaximum = 100;
        //ProgbarValue = 95;
        //ProgText = $"{ProgbarValue} / {ProgbarMaximum}";
        #endregion


        public event PropertyChangedEventHandler PropertyChanged;

        public PdfUploaderWindow()
        {
            InitializeComponent();
        }

        public PdfUploaderWindow(OperationType operationType) : this()
        {
            OperationType = operationType;
            SetPurchaseOrSaleLbl();
        }

        private void test_Click(object sender, RoutedEventArgs e)
        {
            MainList.MainList.Clear();
            GC.Collect();
        }

        private void SetPurchaseOrSaleLbl()
        {
            switch (OperationType)
            {
                case OperationType.Purchase:
                    PurchaseOrSaleLbl.Content = "Импортиране на фактури за покупки";
                    break;
                case OperationType.Sale:
                    PurchaseOrSaleLbl.Content = "Импортиране на фактури за продажби";
                    break;
            }
        }

        private void PdfUploadBtn_Click(object sender, RoutedEventArgs e)
        {
            InvoiceImageListsManipulations invoiceImageListsCreator = new InvoiceImageListsManipulations();
            MainList = invoiceImageListsCreator.CreateMainListOfInvoiceImageLists();
            PurchaseOrSaleLbl.Visibility = Visibility.Hidden;
            CountDocuments();
        }


        private async void PdfAddBtn_Click(object sender, RoutedEventArgs e)
        {
            InvoiceImageListsManipulations.CombineInvoiceFromMainListOfLists(MainList);
            var ImagesForOcr = FileSystemHelper.GetAllJpgFilePathsInTempFolder();

            ProgbarPopupOpen = true;
            ProgbarValue = 0;
            ProgbarMaximum = ImagesForOcr.Length;
            ProgbarText = $"{ProgbarValue} / {ProgbarMaximum}";

            foreach (string OcrFilePath in ImagesForOcr)
            {
                await Task.WhenAll(Task.Run(() => OcrHelper.RunOcr(OcrFilePath)));
                InvoiceSingleEditor.InsertNewInvoiceInSqlTableFromPdfUploader(OperationType,OcrFilePath);
                ProgbarValue++;
                ProgbarText = $"{ProgbarValue} / {ProgbarMaximum}";
            }
            if (ProgbarValue == ProgbarMaximum) ProgbarPopupOpen = false;

            int DocumentItemCount = PuWListView.Items.Count;
            MessageBox.Show($"Добавени са {DocumentItemCount} документа.");
            this.Close();
            UiNavigationHelper.MainWindow.ContentFrame.Content = new InvoiceGridPage(DocumentStatuses.UnAccountedDocuments, OperationType);
        }

        #region Helper Methods
        private static T FindAnchestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);


            }
            while (current != null);
            return null;
        }
        public static DependencyObject GetScrollViewer(DependencyObject o)
        {
            // Return the DependencyObject if it is a ScrollViewer
            if (o is ScrollViewer)
            { return o; }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);

                var result = GetScrollViewer(child);
                if (result == null)
                {
                    continue;
                }
                else
                {
                    return result;
                }
            }
            return null;
        }
        public static childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            // Search immediate children first (breadth-first)
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is childItem)
                    return (childItem)child;

                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);

                    if (childOfChild != null)
                        return childOfChild;
                }
            }

            return null;
        }
        #endregion

        private void PuWListView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
        }

        private void PuWListView_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("InvoiceImage") || sender != e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void PuWListView_Drop(object sender, DragEventArgs e)
        {
            #region Original Code
            //int index = -1;

            //if (e.Data.GetDataPresent("InvoiceImage") && sender == e.Source)
            //{
            //    // Get the drop ListViewItem destination
            //    var listView = sender as ListView;
            //    var listViewItem = FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);

            //    if (listViewItem == null)
            //    {
            //        e.Effects = DragDropEffects.Move;

            //        var IIL = new InvoiceImageList();
            //        foreach (InvoiceImage im in selectedInvoiceImages)
            //        {
            //            IIL.Add(im);
            //        }
            //        MainList.MainList.Add(IIL);
            //        selectedInvoiceImages.Clear();

            //        switch (ComingFrom)
            //        {
            //            case "mainlist":
            //                MainList.MainList.RemoveAt(startIndex);
            //                break;
            //            case "InnerList":
            //                MainList.MainList[startIndex].ListOfImages.RemoveAt(innerStartIndex);
            //                if (MainList.MainList[startIndex].ListOfImages.Count <= 0)
            //                {
            //                    MainList.MainList.RemoveAt(startIndex);
            //                }
            //                break;
            //            default:
            //                //code here
            //                break;
            //        }
            //        ComingFrom = "";
            //    }
            //    else if (listViewItem != null)
            //    {


            //        // Find the data behind the ListViewItem
            //        try
            //        {
            //            var item = (InvoiceImageList)listView.ItemContainerGenerator.ItemFromContainer(listViewItem);
            //            // Move item into observable collection 
            //            // (this will be automatically reflected to lstView.ItemsSource)
            //            e.Effects = DragDropEffects.Move;
            //            index = MainList.MainList.IndexOf(item);



            //            if (startIndex >= 0 && index >= 0)
            //            {

            //                if (startIndex != index)
            //                {
            //                    foreach (InvoiceImage Im in selectedInvoiceImages)
            //                    {
            //                        item.Add(Im);
            //                    }
            //                }
            //            }
            //            selectedInvoiceImages.Clear();
            //            try
            //            {


            //                if (startIndex != index)
            //                {

            //                    switch (ComingFrom)
            //                    {
            //                        case "mainlist":
            //                            MainList.MainList.RemoveAt(startIndex);
            //                            break;
            //                        case "InnerList":
            //                            MainList.MainList[startIndex].ListOfImages.RemoveAt(innerStartIndex);
            //                            if (MainList.MainList[startIndex].ListOfImages.Count <= 0)
            //                            {
            //                                MainList.MainList.RemoveAt(startIndex);
            //                            }
            //                            break;
            //                        default:
            //                            //code here
            //                            break;
            //                    }
            //                    ComingFrom = "";
            //                }
            //                else
            //                {
            //                    return;
            //                }
            //            }
            //            catch
            //            {
            //                return;
            //            }
            //        }
            //        catch
            //        {

            //            return;
            //        }

            //    }
            //    startIndex = -1;        // Done!
            //    innerStartIndex = -1;

            //    //Document Counter
            //    CountDocuments();
            //}
            #endregion // OL 
            #region NewReworked Code

            if (e.Data.GetDataPresent("InvoiceImage") && sender == e.Source)
            {
                // Get the drop ListViewItem destination
                var listView = sender as ListView;
                var listViewItem = FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);

                if (listViewItem == null)
                {
                    e.Effects = DragDropEffects.Move;

                    var IIL = new InvoiceImageList();
                    foreach (InvoiceImage im in selectedInvoiceImages)
                    {
                        IIL.Add(im);
                    }
                    MainList.Add(IIL);
                    selectedInvoiceImages.Clear();
                    Console.WriteLine("Selected list Cleared SENT FROM OUTSIDE LIST"); // DEBUGING HERE

                    switch (ComingFrom)
                    {
                        case "mainlist":
                            MainList.RemoveAt(startIndex);
                            break;
                        case "InnerList":
                            MainList[startIndex].ListOfImages.RemoveAt(innerStartIndex);
                            if (MainList[startIndex].ListOfImages.Count <= 0)
                            {
                                MainList.RemoveAt(startIndex);
                            }
                            break;
                        default:
                            //code here
                            break;
                    }
                    ComingFrom = "";
                }
                else if (listViewItem != null)
                {
                    if (selectedInvoiceImages.Count != 0)
                    {

                        e.Effects = DragDropEffects.Move;

                        var IIL = new InvoiceImageList();
                        foreach (InvoiceImage im in selectedInvoiceImages)
                        {
                            IIL.Add(im);
                        }
                        MainList.Add(IIL);
                        selectedInvoiceImages.Clear();
                        Console.WriteLine("Selected list Cleared SENT FROM OUTSIDE LIST"); // DEBUGING HERE

                        switch (ComingFrom)
                        {
                            case "mainlist":
                                MainList.RemoveAt(startIndex);
                                break;
                            case "InnerList":
                                MainList[startIndex].ListOfImages.RemoveAt(innerStartIndex);
                                if (MainList[startIndex].ListOfImages.Count <= 0)
                                {
                                    MainList.RemoveAt(startIndex);
                                }
                                break;
                            default:
                                //code here
                                break;
                        }
                        ComingFrom = "";

                    }
                }
            }
            #endregion  
        }

        private void PuWListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CountDocuments();
        }

        private void PuWListView_DragOver(object sender, DragEventArgs e)
        {
            ScrollViewer sv = FindVisualChild<ScrollViewer>(PuWListView);

            double tolerance = 40;
            double verticalPos = e.GetPosition(PuWListView).Y;
            double offset = 15;

            if (verticalPos < tolerance) // Top of visible list?
            {
                sv.ScrollToVerticalOffset(sv.VerticalOffset - offset); //Scroll up.
                rec.Height = 40;
                rec.Fill = (System.Windows.Media.Brush)bc.ConvertFrom("#595CB8FF");
                rec.IsHitTestVisible = false;
                rec.VerticalAlignment = VerticalAlignment.Top;
                Grid.SetRow(rec, 1);
                if (MainGrid.Children.Contains(rec) != true)
                {
                    MainGrid.Children.Add(rec);
                }
            }
            else if (verticalPos > PuWListView.ActualHeight - tolerance) //Bottom of visible list?
            {
                sv.ScrollToVerticalOffset(sv.VerticalOffset + offset); //Scroll down.
                rec.Height = 40;
                rec.IsHitTestVisible = false;
                rec.Fill = (System.Windows.Media.Brush)bc.ConvertFrom("#595CB8FF");
                rec.VerticalAlignment = VerticalAlignment.Bottom;
                Grid.SetRow(rec, 1);
                if (MainGrid.Children.Contains(rec) != true)
                {
                    MainGrid.Children.Add(rec);
                }
            }
            else
            {
                if (MainGrid.Children.Contains(rec) == true)
                {
                    MainGrid.Children.Remove(rec);
                }
            };
        }

        private void InternalListView_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            System.Windows.Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                       Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Get the dragged ListViewItem
                var listView = sender as ListView;
                var listViewItem = FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);


                var mainViewItem1 = FindAnchestor<ListViewItem>((DependencyObject)listView);
                var mainlist = FindAnchestor<ListView>((DependencyObject)mainViewItem1);



                // Abort
                if (listViewItem == null) return;
                // Find the data behind the ListViewItem

                try
                {
                    var item = (InvoiceImage)listView.ItemContainerGenerator.ItemFromContainer(listViewItem);

                    var listItem = (InvoiceImageList)mainlist.ItemContainerGenerator.ItemFromContainer(mainViewItem1);

                    // Abort
                    if (item == null) return;
                    // Initialize the drag & drop operation
                    selectedInvoiceImages.Add(item);
                    Console.WriteLine("Selected list ITEM ADDED"); // DEBUGING HERE

                    startIndex = MainList.IndexOf(listItem);
                    innerStartIndex = listView.SelectedIndex;
                    ComingFrom = "InnerList";
                    DataObject dragData = new DataObject("InvoiceImage", item);
                    DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Copy | DragDropEffects.Move);
                }
                catch
                {
                    return;
                }
            }
        }

        private void InternalListView_Drop(object sender, DragEventArgs e)
        {

            int index = -1;

            if (e.Data.GetDataPresent("InvoiceImage") && sender == e.Source)
            {
                // Get the drop ListViewItem destination
                var listView = sender as ListView;
                var listViewItem = FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);

                var MainViewItem = FindAnchestor<ListViewItem>((DependencyObject)listView);
                var mainList = FindAnchestor<ListView>((DependencyObject)MainViewItem);

                if (listViewItem == null)
                {
                    // Abort
                    e.Effects = DragDropEffects.None;
                    return;
                }
                // Find the data behind the ListViewItem
                try
                {
                    var item = (InvoiceImageList)mainList.ItemContainerGenerator.ItemFromContainer(MainViewItem);
                    // Move item into observable collection 
                    // (this will be automatically reflected to lstView.ItemsSource)
                    e.Effects = DragDropEffects.Move;
                    index = MainList.IndexOf(item);
                    if (startIndex >= 0 && index >= 0)
                    {

                        if (startIndex != index)
                        {
                            foreach (var Im in selectedInvoiceImages)
                            {
                                item.Add(Im);
                            }
                        }
                    }
                    selectedInvoiceImages.Clear();
                    Console.WriteLine("Selected list Cleared SENT FROM INSIDE LIST"); // DEBUGING HERE
                    try
                    {
                        if (startIndex != index)
                        {
                            switch (ComingFrom)
                            {
                                case "mainlist":
                                    MainList.RemoveAt(startIndex);
                                    break;
                                case "InnerList":
                                    MainList[startIndex].ListOfImages.RemoveAt(innerStartIndex);
                                    if (MainList[startIndex].ListOfImages.Count <= 0)
                                    {
                                        MainList.RemoveAt(startIndex);
                                    }
                                    break;
                                default:
                                    //code here
                                    break;
                            }
                            ComingFrom = "";
                        }
                        else
                        {
                            return;
                        }
                    }
                    catch
                    {
                        return;
                    }
                }
                catch
                {

                    return;
                }
                startIndex = -1;        // Done!
                innerStartIndex = -1;
            }

        }

        
        private void CountDocuments()
        {
            //Document Counter
            DocCounter = $"Общ брой докумени: {PuWListView.Items.Count}";
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ClearTempAndMemory();
        }

        private void ClearTempAndMemory()
        {
            MainList.Clear();
            FileSystemHelper.ClearTempFolder();
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            //Casting the sender as Image
            Image image = sender as Image;

            //Getting the current InvoiceImage Class
            ContentPresenter contentPresenter = VisualTreeHelper.GetParent(image) as ContentPresenter;
            Border border = VisualTreeHelper.GetParent(contentPresenter) as Border;
            ListViewItem lvi = VisualTreeHelper.GetParent(border) as ListViewItem;
            InvoiceImage invImg = lvi.Content as InvoiceImage;

            //Getting the Tooltip Image Component
            ToolTip toolTip = image.ToolTip as ToolTip;
            Border ttBorder = toolTip.Content as Border;
            Image ttImage = ttBorder.Child as Image;

            //Setting the source on the tooltip Image
            ttImage.Source = BitmapCreator.InvoiceImageFromFilePathHighQuality(invImg.ImagePath);
        }
    }
}



