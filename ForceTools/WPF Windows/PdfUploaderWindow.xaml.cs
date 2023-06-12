using Microsoft.Win32;
using Pdf2Image;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.Drawing.Imaging;
using ForceTools.Models;
using System.ComponentModel;
using Tesseract;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;
using ForceTools.ViewModels;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Media.Media3D;

namespace ForceTools
{
    /// <summary>
    /// Interaction logic for PdfUploaderWindow.xaml
    /// </summary>
    public partial class PdfUploaderWindow : Window, INotifyPropertyChanged
    {
        //Initializing SQL Tools
        private static SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString);
        private static SqlCommand SqCmd;
        private static SqlDataAdapter sqlDataAdapter;

        //Getting All needed default paths for operations
        public static string TempFolderPath { get; set; } = AppDomain.CurrentDomain.BaseDirectory + "\\Temp";
        public static string TessTrainedDataFolder = AppDomain.CurrentDomain.BaseDirectory + "\\TessTrainedData";
        public static string OcrTempFolder = TempFolderPath + "\\Ocr\\";
        public static string RightOcrTxtFilePath = OcrTempFolder + "RightOcrTempText.txt";
        public static string LeftOcrTxtFilePath = OcrTempFolder + "LeftOcrTempText.txt";
        public static string FullOcrTxtFilePath = OcrTempFolder + "FullOcrTempText.txt";

        //Getting or Setting the Default Values Variables
        private int DefaultPurchaseAccount;
        private int DefaultSaleAccount;
        private int DefaultCashRegAccount;
        private string DefaultNote;

        //The Main list for all uploaded invoices
        public MainListOfLists MainList { get; set; } = new MainListOfLists();

        //List of Currently Selected Images
        List<InvoiceImage> selectedInvoiceImages = new List<InvoiceImage>();

        //Getting if the User is working on Purchases or Sales
        private OperationType OperationType;

        //Document drag/drop navigation Variables
        private System.Windows.Point startPoint = new System.Windows.Point();
        private int startIndex = -1;
        private int innerStartIndex = -1;
        private string ComingFrom = "";

        //List of filepaths for image disposal
        List<string> ImagesToBeDeleted = new List<string>();

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
            DataContext = MainList;
            GetDefaultValues();
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
            var OFD = new OpenFileDialog();
            OFD.Multiselect = true;
            OFD.Filter = "PDF Файлове|*.PDF";
            OFD.ShowDialog();
            string[] FilePaths = OFD.FileNames;


            //Converting the PDFs to Jpegs
            foreach (string FileToBeConverted in FilePaths)
            {
                #region OldCode PdfSplitter Code
                //List<System.Drawing.Image> images = PdfSplitter.GetImages(FileToBeConverted, PdfSplitter.Scale.VeryHigh);
                //PdfSplitter.WriteImages(FileToBeConverted, TempFolderPath, PdfSplitter.Scale.VeryHigh, PdfSplitter.CompressionLevel.None);
                #endregion
                #region My Converter Integration
                PdfConverter.WriteImages(FileToBeConverted, TempFolderPath, PdfConverter.Scale.VeryHigh, PdfConverter.CompressionLevel.None);
                #endregion
            }
            GetImages(TempFolderPath);

            //Hiding the PurchaseOrSaleLbl
            PurchaseOrSaleLbl.Visibility = Visibility.Hidden;
            //Document Counter
            CountDocuments();
        }

        private void GetDefaultValues()
        {
            //Getting the Table with the default values
            sqlConnection.Open();
            SqCmd = new SqlCommand("Select * from DefaultValues", sqlConnection);
            sqlDataAdapter = new SqlDataAdapter(SqCmd);
            DataTable DefaultValuesTbl = new DataTable("DefaultValuesTb");
            sqlDataAdapter.Fill(DefaultValuesTbl);

            //Getting the values
            DefaultPurchaseAccount = Convert.ToInt32(DefaultValuesTbl.Rows[0][2]);
            DefaultSaleAccount = Convert.ToInt32(DefaultValuesTbl.Rows[1][2]);
            DefaultCashRegAccount = Convert.ToInt32(DefaultValuesTbl.Rows[2][2]);
            DefaultNote = Convert.ToString(DefaultValuesTbl.Rows[3][2]);
            sqlConnection.Close();

        }

        public void GetImages(string Temp)
        {
            ProgbarValue = 0;


            DirectoryInfo TempFolder = new DirectoryInfo(Temp);
            if (TempFolder.Exists)
            {

                ProgbarPopupOpen = true;
                ProgbarMaximum = TempFolder.GetFiles().Length;
                ProgbarText = $"{ProgbarValue} / {ProgbarMaximum}";

                foreach (FileInfo finfo in TempFolder.GetFiles())
                {
                    if (".jpg".Contains(finfo.Extension.ToLower()))
                    {
                        AddImages(finfo.FullName);
                        ProgbarValue++;
                        if (ProgbarValue == ProgbarMaximum)
                        {
                            ProgbarPopupOpen = false;
                        }
                    }
                }
            }
        }

        private void AddImages(string ImageWithPath)
        {
            //This is creating the source
            var BI = new BitmapImage();
            BI.BeginInit();
            BI.CacheOption = BitmapCacheOption.OnLoad;
            BI.DecodePixelWidth = 400;
            BI.UriSource = new Uri(ImageWithPath, UriKind.Absolute);
            BI.EndInit();
            BI.Freeze();

            //This is Adding the Items to a ObservableCollection
            var ic = new InvoiceImage() { BitImage = BI, ImagePath = ImageWithPath };
            var invoiceImageList = new InvoiceImageList();
            invoiceImageList.Add(ic);
            MainList.Add(invoiceImageList);
            
        }

        public void CombineInvoices()
        {
            foreach (InvoiceImageList iml in PuWListView.Items)
            {
                if (iml.ListOfImages.Count > 1)
                {
                    var lstbitmap = new List<Bitmap>();
                    foreach (InvoiceImage imageClass in iml.ListOfImages)
                    {
                        var bitmap = new Bitmap(imageClass.ImagePath);
                        lstbitmap.Add(bitmap);
                        ImagesToBeDeleted.Add(imageClass.ImagePath);
                    }

                    var width = 0;
                    var height = 0;
                    float DpiHorizontal = 0; //Added by me 
                    float DpiVertical = 0; //Added by me 
                    foreach (var image in lstbitmap)
                    {
                        width = image.Width;
                        height += image.Height;
                        DpiHorizontal = image.HorizontalResolution; //Added by me 
                        DpiVertical = image.VerticalResolution; //Added by me 
                    }
                    var bitmap2 = new Bitmap(width, height);
                    bitmap2.SetResolution(DpiHorizontal, DpiVertical);
                    var g = Graphics.FromImage(bitmap2);
                    var localWidth = 0;
                    var localHeight = 0;
                    foreach (var image in lstbitmap)
                    {
                        g.DrawImage(image, localWidth, localHeight);
                        localHeight += image.Height + 50;
                    }

                    string[] files = Directory.GetFiles(TempFolderPath, "Combined*.jpg");
                    string baseName = System.IO.Path.Combine(TempFolderPath, "Combined");
                    string filename;
                    int i = 0;
                    do
                    {
                        filename = baseName + ++i + ".jpg";
                    } while (files.Contains(filename));

                    bitmap2.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
        }

        public void DeleteUnusedImages()
        {
            foreach (string s in ImagesToBeDeleted)
            {
                //System.GC.Collect();
                //System.GC.WaitForPendingFinalizers();
                File.Delete(s);
            }
        }

        public void ClearTemp()
        {
            var TempFolder = new DirectoryInfo(TempFolderPath);
            foreach (FileInfo finfo in TempFolder.GetFiles())
            {
                if (".jpg".Contains(finfo.Extension.ToLower()))
                {
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    File.Delete(finfo.FullName);
                }
            }
        }

        private async void PdfAddBtn_Click(object sender, RoutedEventArgs e)
        {
            var ImagesForOcr = new List<string>();

            CombineInvoices();
            DeleteUnusedImages();
            ProgbarPopupOpen = true;
            ProgbarValue = 0;
            var TempFolder = new DirectoryInfo(TempFolderPath);

            foreach (var finfo in TempFolder.GetFiles())
            {
                if (".jpg".Contains(finfo.Extension.ToLower()))
                {
                    ImagesForOcr.Add(finfo.FullName);
                }
            }

            ProgbarMaximum = ImagesForOcr.Count;
            foreach (string OcrFilePath in ImagesForOcr)
            {
                var OcrL = Task.Run(() => DoOcrLeft(OcrFilePath));
                var OcrR = Task.Run(() => DoOcrRight(OcrFilePath));
                var OcrFl = Task.Run(() => DoOcrFull(OcrFilePath));
                await Task.WhenAll(OcrL, OcrR, OcrFl);
                ExtractAndAdd(OcrFilePath);
                ProgbarValue++;
                ProgbarText = $"{ProgbarValue} / {ProgbarMaximum}";

            }
            if (ProgbarValue == ProgbarMaximum)
            {
                ProgbarPopupOpen = false;
            }

            string DocumentItemCount = Convert.ToString(PuWListView.Items.Count);
            MessageBox.Show("Добавени са " + (DocumentItemCount) + " документа.");

            //Cleaning up Images in temp and Bitmaps in Lists
            ClearTempAndMemory();
            //Closing the PDF Uploader Window and Updating info in Invoice Grid Page
            this.Close();
            InvoiceGridPage igp = new InvoiceGridPage(DocumentStatuses.UnAccountedDocuments, OperationType);
            MainWindow mw = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive) as MainWindow;
            mw.ContentFrame.Content = igp.Content; //BUG
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

        private void PuWListView_MouseMove(object sender, MouseEventArgs e)
        {
            #region OriginalCode
            //// Get the current mouse position
            //System.Windows.Point mousePos = e.GetPosition(null);
            //Vector diff = startPoint - mousePos;

            //if (e.LeftButton == MouseButtonState.Pressed &&
            //    (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
            //           Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            //{
            //    // Get the dragged ListViewItem
            //    var listView = sender as ListView;
            //    var listViewItem = FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);
            //    // Abort
            //    if (listViewItem == null) return;
            //    // Find the data behind the ListViewItem

            //    try
            //    {
            //        InvoiceImageList item = (InvoiceImageList)listView.ItemContainerGenerator.ItemFromContainer(listViewItem);
            //        // Abort
            //        if (item == null) return;
            //        // Initialize the drag & drop operation

            //        foreach (var im in item.ListOfImages)
            //        {
            //            selectedInvoiceImages.Add(im);
            //        }
            //        startIndex = listView.SelectedIndex;
            //        ComingFrom = "mainlist";
            //        DataObject dragData = new DataObject("InvoiceImage", item);
            //        DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Copy | DragDropEffects.Move);
            //    }
            //    catch
            //    {
            //        return;
            //    }
            //}
            #endregion
            #region NewReworked Code
            var listView = sender as ListView;
            startIndex = listView.SelectedIndex;
            ComingFrom = "mainlist";
            #endregion
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
                    MainList.MainList.Add(IIL);
                    selectedInvoiceImages.Clear();

                    switch (ComingFrom)
                    {
                        case "mainlist":
                            MainList.MainList.RemoveAt(startIndex);
                            break;
                        case "InnerList":
                            MainList.MainList[startIndex].ListOfImages.RemoveAt(innerStartIndex);
                            if (MainList.MainList[startIndex].ListOfImages.Count <= 0)
                            {
                                MainList.MainList.RemoveAt(startIndex);
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
                        MainList.MainList.Add(IIL);
                        selectedInvoiceImages.Clear();

                        switch (ComingFrom)
                        {
                            case "mainlist":
                                MainList.MainList.RemoveAt(startIndex);
                                break;
                            case "InnerList":
                                MainList.MainList[startIndex].ListOfImages.RemoveAt(innerStartIndex);
                                if (MainList.MainList[startIndex].ListOfImages.Count <= 0)
                                {
                                    MainList.MainList.RemoveAt(startIndex);
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

                    startIndex = MainList.MainList.IndexOf(listItem);
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
                    index = MainList.MainList.IndexOf(item);
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
                    try
                    {


                        if (startIndex != index)
                        {
                            switch (ComingFrom)
                            {
                                case "mainlist":
                                    MainList.MainList.RemoveAt(startIndex);
                                    break;
                                case "InnerList":
                                    MainList.MainList[startIndex].ListOfImages.RemoveAt(innerStartIndex);
                                    if (MainList.MainList[startIndex].ListOfImages.Count <= 0)
                                    {
                                        MainList.MainList.RemoveAt(startIndex);
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

        #region OldOcrTechnique
        //private void DoOcr(string sourceForOcrFilePath)
        //{

        //    if (!Directory.Exists(OcrTempFolder))
        //    {
        //        Directory.CreateDirectory(OcrTempFolder);
        //    }


        //    using (var ocrEngine = new TesseractEngine(TessTrainedDataFolder, "bul", EngineMode.TesseractAndLstm))
        //    {
        //        ocrEngine.SetVariable("user_defined_dpi", "300"); //set dpi for supressing warning
        //        using (var img = Pix.LoadFromFile(sourceForOcrFilePath))
        //        {
        //            //LeftOcr
        //            using (var page = ocrEngine.Process(img, Tesseract.Rect.FromCoords(0, 0, img.Width / 2, img.Height / 3)))
        //            {
        //                var ocrText = page.GetText();

        //                File.WriteAllText(LeftOcrTxtFilePath, ocrText);

        //            }
        //            //RightOcr
        //            using (var page = ocrEngine.Process(img, Tesseract.Rect.FromCoords(img.Width / 2, 0, img.Width, img.Height / 3)))
        //            {
        //                var ocrText = page.GetText();

        //                File.WriteAllText(RightOcrTxtFilePath, ocrText);

        //            }
        //            //FullOcr
        //            using (var page = ocrEngine.Process(img, Tesseract.Rect.FromCoords(0, 0, img.Width, img.Height)))
        //            {
        //                var ocrText = page.GetText();

        //                File.WriteAllText(FullOcrTxtFilePath, ocrText);

        //            }
        //        }
        //    }
        //}
        #endregion

        #region NewOcrTechnique
        private void DoOcrLeft(string sourceForOcrFilePath)
        {

            if (!Directory.Exists(OcrTempFolder))
            {
                Directory.CreateDirectory(OcrTempFolder);
            }


            using (var ocrEngine = new TesseractEngine(TessTrainedDataFolder, "bul", EngineMode.TesseractAndLstm))
            {
                ocrEngine.SetVariable("user_defined_dpi", "300"); //set dpi for supressing warning
                using (var img = Pix.LoadFromFile(sourceForOcrFilePath))
                {
                    //LeftOcr
                    using (var page = ocrEngine.Process(img, Tesseract.Rect.FromCoords(0, 0, img.Width / 2, img.Height / 3)))
                    {
                        var ocrText = page.GetText();

                        File.WriteAllText(LeftOcrTxtFilePath, ocrText);

                    }
                }
            }
        }

        private void DoOcrRight(string sourceForOcrFilePath)
        {
            using (var ocrEngine = new TesseractEngine(TessTrainedDataFolder, "bul", EngineMode.TesseractAndLstm))
            {
                ocrEngine.SetVariable("user_defined_dpi", "300"); //set dpi for supressing warning
                using (var img = Pix.LoadFromFile(sourceForOcrFilePath))
                {
                    //RightOcr
                    using (var page = ocrEngine.Process(img, Tesseract.Rect.FromCoords(img.Width / 2, 0, img.Width, img.Height / 3)))
                    {
                        var ocrText = page.GetText();

                        File.WriteAllText(RightOcrTxtFilePath, ocrText);

                    }
                }
            }
        }

        private void DoOcrFull(string sourceForOcrFilePath)
        {

            using (var ocrEngine = new TesseractEngine(TessTrainedDataFolder, "bul", EngineMode.TesseractAndLstm))
            {
                ocrEngine.SetVariable("user_defined_dpi", "300"); //set dpi for supressing warning
                using (var img = Pix.LoadFromFile(sourceForOcrFilePath))
                {
                    //FullOcr
                    using (var page = ocrEngine.Process(img, Tesseract.Rect.FromCoords(0, 0, img.Width, img.Height)))
                    {
                        var ocrText = page.GetText();

                        File.WriteAllText(FullOcrTxtFilePath, ocrText);

                    }
                }
            }
        }
        #endregion

        public void ExtractAndAdd(string ImageFilePath)
        {
            string TextLeftFile = File.ReadAllText(LeftOcrTxtFilePath);
            string TextRightFIle = File.ReadAllText(RightOcrTxtFilePath);
            string TextFullFile = File.ReadAllText(FullOcrTxtFilePath);

            #region Kontragent Extraction
            //Getting Kontragent Name / Determening if the document is for a purchase or a sale
            Regex kontragentExtraction = new Regex("");
            Regex kontragentExtractionPT2 = new Regex("");
            switch (OperationType)
            {
                case OperationType.Purchase:
                    kontragentExtraction = new Regex(@"(?<=Доставчик:?).*", RegexOptions.IgnoreCase);
                    kontragentExtractionPT2 = new Regex(@"(?<=Доставчик:?\n).*", RegexOptions.IgnoreCase);
                    break;
                case OperationType.Sale:
                    kontragentExtraction = new Regex(@"(?<=Получател:?).*", RegexOptions.IgnoreCase);
                    kontragentExtractionPT2 = new Regex(@"(?<=Получател:?\n).*", RegexOptions.IgnoreCase);
                    break;
            }

            //Getting EIK
            Regex EikExtractionPT1 = new Regex(@"(?<=ЕИК|Идент|Ном\.:).*", RegexOptions.IgnoreCase);
            Regex EikExtractionPT2 = new Regex("[0-9]{9,10}");
            string EIKStr = "";
            string DDSNumberStr = "";
            long InvoiceNumberInt = 0;
            string KontragentStr = string.Empty;
            try
            {
                if (kontragentExtraction.Match(TextRightFIle).ToString() != String.Empty)
                {
                    KontragentStr = kontragentExtraction.Match(TextRightFIle).ToString().Trim();
                    string StrEikExtPT1 = EikExtractionPT1.Match(TextRightFIle).ToString();
                    EIKStr = EikExtractionPT2.Match(StrEikExtPT1).ToString();

                    //Getting DDS Number
                    if (EIKStr != "" && EIKStr.ToString().Length < 10)
                    {
                        Regex DDSNumberRegexPt1 = new Regex(@"(?<=ДДС).{11,30}", RegexOptions.IgnoreCase);
                        string DDSNumberStringPt1 = DDSNumberRegexPt1.Match(TextRightFIle).ToString();
                        Regex DDSNumberRegexPt2 = new Regex(@"(\d{9})");

                        DDSNumberStr = $"BG{DDSNumberRegexPt2.Match(DDSNumberStringPt1)}";
                    }
                    if (DDSNumberStr == "")
                    {
                        DDSNumberStr = $"{EIKStr}";
                    }
                }
                else if (kontragentExtraction.Match(TextLeftFile).ToString() != String.Empty)
                {
                    KontragentStr = kontragentExtraction.Match(TextLeftFile).ToString().Trim();
                    string StrEikExtPT1 = EikExtractionPT1.Match(TextLeftFile).ToString();
                    string conversion = EikExtractionPT2.Match(StrEikExtPT1).ToString();
                    EIKStr = conversion;

                    //Getting DDS Number
                    if (EIKStr != "" && EIKStr.ToString().Length < 10)
                    {
                        Regex DDSNumberRegexPt1 = new Regex(@"(?<=ДДС).{11,30}", RegexOptions.IgnoreCase);
                        string DDSNumberStringPt1 = DDSNumberRegexPt1.Match(TextLeftFile).ToString();
                        Regex DDSNumberRegexPt2 = new Regex(@"(\d{9})");

                        DDSNumberStr = $"BG{DDSNumberRegexPt2.Match(DDSNumberStringPt1)}";
                    }
                    if (DDSNumberStr == "")
                    {
                        DDSNumberStr = $"{EIKStr}";
                    }
                }
                else if (kontragentExtractionPT2.Match(TextRightFIle).ToString() != String.Empty)
                {
                    KontragentStr = kontragentExtractionPT2.Match(TextRightFIle).ToString().Trim();
                    string StrEikExtPT1 = EikExtractionPT1.Match(TextRightFIle).ToString();
                    EIKStr = EikExtractionPT2.Match(StrEikExtPT1).ToString();

                    //Getting DDS Number
                    if (EIKStr != "" && EIKStr.ToString().Length < 10)
                    {
                        Regex DDSNumberRegexPt1 = new Regex(@"(?<=ДДС).{11,30}", RegexOptions.IgnoreCase);
                        string DDSNumberStringPt1 = DDSNumberRegexPt1.Match(TextRightFIle).ToString();
                        Regex DDSNumberRegexPt2 = new Regex(@"(\d{9})");

                        DDSNumberStr = $"BG{DDSNumberRegexPt2.Match(DDSNumberStringPt1)}";
                    }
                    if (DDSNumberStr == "")
                    {
                        DDSNumberStr = $"{EIKStr}";
                    }
                }
                else if (kontragentExtractionPT2.Match(TextLeftFile).ToString() != String.Empty)
                {
                    KontragentStr = kontragentExtractionPT2.Match(TextLeftFile).ToString().Trim();
                    string StrEikExtPT1 = EikExtractionPT1.Match(TextLeftFile).ToString();
                    EIKStr = EikExtractionPT2.Match(StrEikExtPT1).ToString();

                    //Getting DDS Number
                    if (EIKStr != "" && EIKStr.ToString().Length < 10)
                    {
                        Regex DDSNumberRegexPt1 = new Regex(@"(?<=ДДС).{11,30}", RegexOptions.IgnoreCase);
                        string DDSNumberStringPt1 = DDSNumberRegexPt1.Match(TextLeftFile).ToString();
                        Regex DDSNumberRegexPt2 = new Regex(@"(\d{9})");

                        DDSNumberStr = $"BG{DDSNumberRegexPt2.Match(DDSNumberStringPt1)}";
                    }
                    if (DDSNumberStr == "")
                    {
                        DDSNumberStr = $"{EIKStr}";
                    }
                }
                else
                {
                    MessageBox.Show("Nimoa kontragenta");
                }
            }
            catch
            {
                EIKStr = "";
            }
            #endregion
            #region Invoice Number Extraction
            Regex InvoiceNumberExtractionPT3 = new Regex("[0-9]{10}", RegexOptions.IgnoreCase);
            InvoiceNumberInt = Convert.ToInt64(InvoiceNumberExtractionPT3.Match(TextFullFile).ToString());
            #endregion
            #region FullValueExtraction
            decimal FullValueDec = 0;
            Regex FullValueExtractionPt1 = new Regex(@"(?<=Сума за плащане:?\s?|Общо:?\s?|Всичко:?\s?).*", RegexOptions.IgnoreCase);
            Regex FullValueExtractionPt2 = new Regex(@"\d{1,5},\d{0,2}");
            string FullValuePt1 = FullValueExtractionPt1.Match(TextFullFile).ToString().Trim().Replace(" ", "");
            try
            {
                FullValueDec = Convert.ToDecimal(FullValueExtractionPt2.Match(FullValuePt1).ToString());
            }
            catch
            {
                FullValueDec = 0;
            }
            #endregion
            #region DateExtraction
            Regex DateExtractionPT1 = new Regex(@"(?<=Дата).*", RegexOptions.IgnoreCase);
            Regex DateExtractionPT2 = new Regex(@"\d{1,2}\.\d{1,2}\.\d{1,4}");
            string StrDateExtPT1 = DateExtractionPT1.Match(TextFullFile).ToString();
            DateTime dateTimeDt = Convert.ToDateTime(DateExtractionPT2.Match(StrDateExtPT1).ToString());
            #endregion
            #region DanOsnExtraction
            decimal DoDec = 0;
            Regex DanOsnExtractionPt1 = new Regex(@"(?<=Данъчна основа:?\s?|ДО:\s?|Дан. основа:?\s?).*", RegexOptions.IgnoreCase);
            Regex DanOsnExtractionPt2 = new Regex(@"\d{1,5},\d{0,2}");
            string DanOsnPt1 = DanOsnExtractionPt1.Match(TextFullFile).ToString().Trim().Replace(" ", "");
            try
            {
                DoDec = Convert.ToDecimal(DanOsnExtractionPt2.Match(DanOsnPt1).ToString());
            }
            catch
            {
                DoDec = 0;
            }
            #endregion
            #region DocType Extraction
            Regex FacTypeRegex = new Regex(@"(Фактура)", RegexOptions.IgnoreCase);
            Regex KiTypeRegex = new Regex(@"(Кредитно известие)", RegexOptions.IgnoreCase);
            Regex ToFaktura = new Regex(@"(Към Фактура)", RegexOptions.IgnoreCase);
            int DocTypeIdInt = 0;

            if (FacTypeRegex.IsMatch(TextFullFile) == true && KiTypeRegex.IsMatch(TextFullFile) == false)
            {
                switch (ToFaktura.IsMatch(TextFullFile))
                {
                    case true:
                        break;
                    case false:
                        DocTypeIdInt = 1;
                        break;

                }


            }
            else if (FacTypeRegex.IsMatch(TextFullFile) == false && KiTypeRegex.IsMatch(TextFullFile) == true)
            {
                switch (ToFaktura.IsMatch(TextFullFile))
                {
                    case true:
                        DocTypeIdInt = 3;
                        break;
                    case false:
                        DocTypeIdInt = 3;
                        break;

                }
            }
            else if (FacTypeRegex.IsMatch(TextFullFile) == true && KiTypeRegex.IsMatch(TextFullFile) == true)
            {
                switch (ToFaktura.IsMatch(TextFullFile))
                {
                    case true:
                        DocTypeIdInt = 3;
                        break;
                    case false:
                        DocTypeIdInt = 3;
                        break;

                }
            }

            #endregion
            #region Image to Byte Array 
            byte[] ImageInBytes = File.ReadAllBytes(ImageFilePath);
            #endregion
            #region Final Logic Conversions
            //Converting Do,DDS,FullValue considering DocType
            if (DocTypeIdInt == 3)
            {
                if (DoDec > 0)
                {
                    DoDec = -DoDec;
                }
                if (FullValueDec > 0)
                {
                    FullValueDec = -FullValueDec;
                }
            }
            //Setting the note to "КИ" considering DocType
            if (DocTypeIdInt == 3)
            {
                DefaultNote = "КИ";
            }
            else
            {
                GetDefaultValues();
            }
            //Setting DealKind considering DO, FullValue and Purchase or Sale document
            int DealKindIdInt = 0;
            switch (OperationType)
            {
                case OperationType.Purchase:
                    if (DoDec != 0 && FullValueDec != 0)
                    {
                        if (DoDec == FullValueDec)
                        {
                            DealKindIdInt = 12; /////////////////////////CHANGE THIS 
                        }
                        else
                        {
                            DealKindIdInt = 12;
                        }
                    }
                    break;
                case OperationType.Sale:
                    if (DoDec != 0 && FullValueDec != 0)
                    {
                        if (DoDec == FullValueDec)
                        {
                            DealKindIdInt = 25;
                        }
                        else
                        {
                            DealKindIdInt = 21;
                        }
                    }
                    break;
            }
            #endregion
            #region Adding Data to SQL Server
            //Adding data to SQL Server
            sqlConnection.Open();
            //Adding Data to Kontragenti Table
            int? KontIdFromSearchTable = null;
            if (EIKStr != "")
            {
                SqCmd = new SqlCommand("Select Kontragenti.Id from Kontragenti where EIK like '%" + EIKStr + "%'", sqlConnection);
                sqlDataAdapter = new SqlDataAdapter(SqCmd);
                DataTable KontByIdTb = new DataTable("KontById");
                sqlDataAdapter.Fill(KontByIdTb);
                if (KontByIdTb.Rows.Count == 0)
                {
                    SqCmd = new SqlCommand("INSERT into Kontragenti (Name, EIK, DDSNumber) VALUES (@KontragentName, @EIK, @DDSNumber)", sqlConnection);

                    SqCmd.Parameters.AddWithValue("@KontragentName", KontragentStr);
                    SqCmd.Parameters.AddWithValue("@EIK", EIKStr);
                    SqCmd.Parameters.AddWithValue("@DDSNumber", DDSNumberStr);
                    SqCmd.ExecuteNonQuery();
                    KontByIdTb.Clear();
                    sqlDataAdapter.Fill(KontByIdTb);

                }

                if (KontByIdTb.Rows.Count > 0 && KontByIdTb.Rows.Count < 2)
                {
                    KontIdFromSearchTable = KontByIdTb.Rows[0].Field<int>("Id");
                }
            }
            else
            {
                KontIdFromSearchTable = 0;
            }
            //Adding Data to Fakturi Table
            SqCmd = new SqlCommand("INSERT into Fakturi (KontragentiId, Date, Number, DO, DDS, FullValue,AccountingStatusId,image,AccDate, DealKindId, DocTypeId, Account, InCashAccount, Note, PurchaseOrSale) VALUES (@KontragentiId, @Date, @Number, @DO, @DDS, @FullValue,@AccountingStatusId,@image, @AccDate, @DealKindId, @DocTypeId, @Account, @InCashAccount, @Note, @PurchaseOrSale)", sqlConnection);

            SqCmd.Parameters.AddWithValue("@KontragentiId", KontIdFromSearchTable);
            SqCmd.Parameters.AddWithValue("@Date", dateTimeDt);
            SqCmd.Parameters.AddWithValue("@Number", InvoiceNumberInt);
            SqCmd.Parameters.AddWithValue("@DO", DoDec);
            SqCmd.Parameters.AddWithValue("@DDS", FullValueDec - DoDec);
            SqCmd.Parameters.AddWithValue("@FullValue", FullValueDec);
            SqCmd.Parameters.AddWithValue("@AccountingStatusId", 2);
            SqCmd.Parameters.AddWithValue("@image", ImageInBytes);
            SqCmd.Parameters.AddWithValue("@AccDate", dateTimeDt);
            SqCmd.Parameters.AddWithValue("@DealKindId", DealKindIdInt);
            SqCmd.Parameters.AddWithValue("@DocTypeId", DocTypeIdInt);
            SqCmd.Parameters.AddWithValue("@InCashAccount", DefaultCashRegAccount);
            SqCmd.Parameters.AddWithValue("@Note", DefaultNote);
            //Setting DocType Specific info 
            switch (OperationType)
            {
                case OperationType.Purchase:
                    SqCmd.Parameters.AddWithValue("@Account", DefaultPurchaseAccount);
                    SqCmd.Parameters.AddWithValue("@PurchaseOrSale", "Purchase");
                    break;
                case OperationType.Sale:
                    SqCmd.Parameters.AddWithValue("@Account", DefaultSaleAccount);
                    SqCmd.Parameters.AddWithValue("@PurchaseOrSale", "Sale");
                    break;
            }

            SqCmd.ExecuteNonQuery();
            sqlConnection.Close();
            #endregion
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
            MainList.MainList.Clear();
            ClearTemp();
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            //Casting the sender as Image
            System.Windows.Controls.Image image = sender as System.Windows.Controls.Image;

            //Getting the current InvoiceImage Class
            ContentPresenter contentPresenter = VisualTreeHelper.GetParent(image) as ContentPresenter;
            Border border = VisualTreeHelper.GetParent(contentPresenter) as Border;
            ListViewItem lvi = VisualTreeHelper.GetParent(border) as ListViewItem;
            InvoiceImage invImg = lvi.Content as InvoiceImage;

            //Getting the Tooltip Image Component
            ToolTip toolTip = image.ToolTip as ToolTip;
            Border ttBorder = toolTip.Content as Border;
            System.Windows.Controls.Image ttImage = ttBorder.Child as System.Windows.Controls.Image;

            //This is creating the source
            var BI = new BitmapImage();
            BI.BeginInit();
            BI.CacheOption = BitmapCacheOption.OnLoad;
            BI.UriSource = new Uri(invImg.ImagePath, UriKind.Absolute);
            BI.EndInit();
            BI.Freeze();
            
            //Setting the source on the tooltip Image
            ttImage.Source = BI;
        }
    }
}



