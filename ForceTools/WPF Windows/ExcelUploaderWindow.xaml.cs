using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace ForceTools.WPF_Windows
{
    public partial class ExcelUploaderWindow : Window, INotifyPropertyChanged
    {
        private readonly OperationType _operationType;
        private readonly List<ComboBox> comboBoxList = new List<ComboBox>();
        private readonly List<int> comboBoxSelectedIndexesList = new List<int>();
        private DataTable excelDataTable;
        private DataTable _FinalEditDataTable;
        public DataTable FinalEditDataTable { get { return _FinalEditDataTable; } set { _FinalEditDataTable = value; OnPropertyChanged(); } }


        private DataTable _DocTypesDataTable;
        private DataTable _KindOfDealsDataTable;
        public DataTable DocTypesDataTable { get {return _DocTypesDataTable; } set { _DocTypesDataTable = value; OnPropertyChanged(); } }
        public DataTable KindOfDealsDataTable { get { return _KindOfDealsDataTable; } set { _KindOfDealsDataTable = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;

        public ExcelUploaderWindow(OperationType operationType)
        {
            _operationType = operationType;
            DocTypesDataTable = SavedTypesRetriever.GetDocTypesDataTable();
            KindOfDealsDataTable = SavedTypesRetriever.GetKindOfDealsDataTable();
            InitializeComponent();
        }
        private void CreateComboBoxes()
        {
            for (int i = 0; i < ExcelDataGrid.Columns.Count; i++)
            {
                ComboBox newCb = new ComboBox() { MaxWidth = 100, Width = 100, Height = 30 };
                ComboBoxItem cbItem = new ComboBoxItem() { Content = "Null" };
                ComboBoxItem cbItem1 = new ComboBoxItem() { Content = "Номер Док." };
                ComboBoxItem cbItem2 = new ComboBoxItem() { Content = "Дата" };
                ComboBoxItem cbItem3 = new ComboBoxItem() { Content = "Контрагент" };
                ComboBoxItem cbItem4 = new ComboBoxItem() { Content = "ЕИК" };
                ComboBoxItem cbItem5 = new ComboBoxItem() { Content = "ДДС Ном." };
                ComboBoxItem cbItem6 = new ComboBoxItem() { Content = "Данъчна основа" };
                ComboBoxItem cbItem7 = new ComboBoxItem() { Content = "ДДС стойност" };
                ComboBoxItem cbItem8 = new ComboBoxItem() { Content = "Общо стойност" };
                ComboBoxItem cbItem9 = new ComboBoxItem() { Content = "ЕИК + ДДС Ном." };
                ComboBoxItem cbItem10 = new ComboBoxItem() { Content = "Тип Документ" };
                ComboBoxItem cbItem11 = new ComboBoxItem() { Content = "Плащане в брой" };
                newCb.Items.Add(cbItem);
                newCb.Items.Add(cbItem1);
                newCb.Items.Add(cbItem2);
                newCb.Items.Add(cbItem3);
                newCb.Items.Add(cbItem4);
                newCb.Items.Add(cbItem5);
                newCb.Items.Add(cbItem6);
                newCb.Items.Add(cbItem7);
                newCb.Items.Add(cbItem8);
                newCb.Items.Add(cbItem9);
                newCb.Items.Add(cbItem10);
                newCb.Items.Add(cbItem11);
                comboBoxList.Add(newCb);
                ComboBoxesStackPanel.Children.Add(newCb);
                newCb.SelectionChanged += NewCb_SelectionChanged;
            }
        }
        private void SetLabelsBackgroundColorAccordingToFields()
        {
            comboBoxSelectedIndexesList.Clear();
            foreach (var comboBox in comboBoxList)
            {
                comboBoxSelectedIndexesList.Add(comboBox.SelectedIndex);
                if (comboBox.SelectedIndex == 1) DocNumLbl.Background = Brushes.Green;
                if (comboBox.SelectedIndex == 2) DateLbl.Background = Brushes.Green;
                if (comboBox.SelectedIndex == 3) KontLbl.Background = Brushes.Green;
                if (comboBox.SelectedIndex == 4) EikLbl.Background = Brushes.Green;
                if (comboBox.SelectedIndex == 5) DdsNumLbl.Background = Brushes.Green;
                if (comboBox.SelectedIndex == 6) DoLbl.Background = Brushes.Green;
                if (comboBox.SelectedIndex == 7) DdsLbl.Background = Brushes.Green;
                if (comboBox.SelectedIndex == 8) FullValueLbl.Background = Brushes.Green;
                if (comboBox.SelectedIndex == 9) { EikLbl.Background = Brushes.Green; DdsNumLbl.Background = Brushes.Green; }
                if (comboBox.SelectedIndex == 10) DocTypeLbl.Background = Brushes.Green;
                if (comboBox.SelectedIndex == 11) InCashLbl.Background = Brushes.Green;
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (comboBoxSelectedIndexesList.Contains(1) == false) DocNumLbl.Background = Brushes.Red;
                if (comboBoxSelectedIndexesList.Contains(2) == false) DateLbl.Background = Brushes.Red;
                if (comboBoxSelectedIndexesList.Contains(3) == false) KontLbl.Background = Brushes.Red;
                if (comboBoxSelectedIndexesList.Contains(4) == false && comboBoxSelectedIndexesList.Contains(9) == false) EikLbl.Background = Brushes.Red;
                if (comboBoxSelectedIndexesList.Contains(5) == false && comboBoxSelectedIndexesList.Contains(9) == false) DdsNumLbl.Background = Brushes.Red;
                if (comboBoxSelectedIndexesList.Contains(6) == false) DoLbl.Background = Brushes.Red;
                if (comboBoxSelectedIndexesList.Contains(7) == false) DdsLbl.Background = Brushes.Red;
                if (comboBoxSelectedIndexesList.Contains(8) == false && comboBoxSelectedIndexesList.Contains(6) && comboBoxSelectedIndexesList.Contains(7)) FullValueLbl.Background = Brushes.Blue;
                else if (comboBoxSelectedIndexesList.Contains(8) == false) FullValueLbl.Background = Brushes.Red;
                if (comboBoxSelectedIndexesList.Contains(10) == false) DocTypeLbl.Background = Brushes.Blue;
                if (comboBoxSelectedIndexesList.Contains(11) == false) InCashLbl.Background = Brushes.Blue;
            }
        }
        private void CheckIfAllMendatoryComboBoxFieldsAreFilled(List<int> comboBoxSelectedIndexesList)
        {
            if (comboBoxSelectedIndexesList.Contains(1) == false)
            {
                AddDocumentsBtn.IsEnabled = false;
                return;
            }
            if (comboBoxSelectedIndexesList.Contains(2) == false)
            {
                AddDocumentsBtn.IsEnabled = false;
                return;
            }
            if (comboBoxSelectedIndexesList.Contains(3) == false)
            {
                AddDocumentsBtn.IsEnabled = false;
                return;
            }
            if (comboBoxSelectedIndexesList.Contains(4) == false && comboBoxSelectedIndexesList.Contains(9) == false)
            {
                AddDocumentsBtn.IsEnabled = false;
                return;
            }
            if (comboBoxSelectedIndexesList.Contains(5) == false && comboBoxSelectedIndexesList.Contains(9) == false)
            {
                AddDocumentsBtn.IsEnabled = false;
                return;
            }
            if (comboBoxSelectedIndexesList.Contains(6) == false)
            {
                AddDocumentsBtn.IsEnabled = false;
                return;
            }
            if (comboBoxSelectedIndexesList.Contains(7) == false)
            {
                AddDocumentsBtn.IsEnabled = false;
                return;
            }
            AddDocumentsBtn.IsEnabled = true;
        }

        private void UploadExcelBtn_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FileSystemHelper.OpenFileDialogAndGetExcelFilePath();
            ExcelReader excelReader = new ExcelReader();
            excelDataTable = excelReader.GetDataTable(filePath);
            ExcelDataGrid.ItemsSource = excelDataTable.DefaultView;
            CreateComboBoxes();
            SetLabelsBackgroundColorAccordingToFields();
        }
        private void AddDocumentsBtn_Click(object sender, RoutedEventArgs e)
        {
            int currentRow = 0;
            int totalRows = excelDataTable.Rows.Count;

            FinalEditDataTable = new DataTable();
            FinalEditDataTable.Columns.Add("Date");
            FinalEditDataTable.Columns.Add("Number");
            FinalEditDataTable.Columns.Add("Name");
            FinalEditDataTable.Columns.Add("EIK");
            FinalEditDataTable.Columns.Add("DDSNumber");
            FinalEditDataTable.Columns.Add("DO");
            FinalEditDataTable.Columns.Add("DDS");
            FinalEditDataTable.Columns.Add("FullValue");
            FinalEditDataTable.Columns.Add("InCashAccount");
            FinalEditDataTable.Columns.Add("Account");
            FinalEditDataTable.Columns.Add("Note");
            FinalEditDataTable.Columns.Add("DocType");
            FinalEditDataTable.Columns.Add("DealKind");


            while (currentRow < totalRows)
            {
                ExcelExtractedDataInterpreter interpreter = new ExcelExtractedDataInterpreter(_operationType, currentRow, comboBoxList, excelDataTable);
                FinalEditDataTable.Rows.Add(interpreter.GetInterpreterDataRow());
                currentRow++;
            }

            ExcelDataGrid.Visibility = Visibility.Hidden;
            AddDocumentsBtn.Visibility = Visibility.Hidden;
            FinalEditDataGrid.Visibility = Visibility.Visible;
            ConfirmFinalEditBtn.Visibility = Visibility.Visible;
        }
        private void ConfirmFinalEditBtn_Click(object sender, RoutedEventArgs e)
        {
            int currentRow = 0;
            int totalRows = excelDataTable.Rows.Count;
            while (currentRow < totalRows)
            {
                InvoiceSingleEditor.InsertNewInvoiceInSqlTableFromExcelUploader(_operationType, currentRow, FinalEditDataTable);
                currentRow++;
            }
            MessageBox.Show($"Добавени са {totalRows} документа.");
            this.Close();
            UiNavigationHelper.MainWindow.ContentFrame.Content = new InvoiceGridPage(DocumentStatuses.UnAccountedDocuments, _operationType);
        }
        private void FinalEditInvoiceSplitBtn_Click(object sender, RoutedEventArgs e)
        {
            int SelectedPosition = FinalEditDataGrid.SelectedIndex;
            int InsertPosition = FinalEditDataGrid.SelectedIndex + 1;
            DataTable sortedDataTable = FinalEditDataTable.DefaultView.ToTable();
            ExcelInvoiceSplitterWindow editingWindow = new ExcelInvoiceSplitterWindow(this, sortedDataTable, SelectedPosition, InsertPosition);
            editingWindow.ShowDialog();
            FinalEditDataGrid.SelectedIndex = InsertPosition;
            FinalEditDataGrid.CurrentCell = new DataGridCellInfo(FinalEditDataGrid.Items[InsertPosition], FinalEditDataGrid.Columns[1]);
        }

        private void NewCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetLabelsBackgroundColorAccordingToFields();
            CheckIfAllMendatoryComboBoxFieldsAreFilled(comboBoxSelectedIndexesList);
        }
        private void ExcelDataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ComboBoxesScrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset);
        }

        private void DocTypeEditingTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.Focus();
            StackPanel stackPanel = textBox.Parent as StackPanel;
            Popup popup = stackPanel.Children[1] as Popup;
            DataGrid templateDg = popup.Child as DataGrid;
            popup.IsOpen = true;
            SelectRowByIndex(templateDg, 0);
        }
        private void FinalEditDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Up && e.Key != Key.Down && e.Key != Key.Left && e.Key != Key.Right)
            {
                if (FinalEditDataGrid.CurrentCell.Column.DisplayIndex == 11) FinalEditDataGrid.BeginEdit();
                if (FinalEditDataGrid.CurrentCell.Column.DisplayIndex == 12) FinalEditDataGrid.BeginEdit();
            }
        }
        private void DocTypeDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Up && e.Key != Key.Down && e.Key != Key.Left && e.Key != Key.Right)
            {
                DataGrid dg = sender as DataGrid;
                Popup popup = dg.Parent as Popup;
                StackPanel stackPanel = popup.Parent as StackPanel;
                TextBox tb = stackPanel.Children[0] as TextBox;
                tb.Focus();
            }
            if (e.Key == Key.Enter)
            {
                DataGrid dg = sender as DataGrid;
                var selectedItem = DocTypesDataTable.Rows[dg.SelectedIndex][0];
                FinalEditDataTable.Rows[FinalEditDataGrid.SelectedIndex][11] = selectedItem;
                Popup popup = dg.Parent as Popup;
                popup.IsOpen = false;
                FinalEditDataGrid.CommitEdit();
            }
        }
        private void DealKindEditingTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.Focus();
            StackPanel stackPanel = textBox.Parent as StackPanel;
            Popup popup = stackPanel.Children[1] as Popup;
            DataGrid templateDg = popup.Child as DataGrid;
            popup.IsOpen = true;
            SelectRowByIndex(templateDg, 0);
        }

        private void DealKindDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Up && e.Key != Key.Down && e.Key != Key.Left && e.Key != Key.Right)
            {
                DataGrid dg = sender as DataGrid;
                Popup popup = dg.Parent as Popup;
                StackPanel stackPanel = popup.Parent as StackPanel;
                TextBox tb = stackPanel.Children[0] as TextBox;
                tb.Focus();
            }
            if (e.Key == Key.Enter)
            {
                DataGrid dg = sender as DataGrid;
                var selectedItem = KindOfDealsDataTable.Rows[dg.SelectedIndex][0];
                FinalEditDataTable.Rows[FinalEditDataGrid.SelectedIndex][12] = selectedItem;
                Popup popup = dg.Parent as Popup;
                popup.IsOpen = false;
                FinalEditDataGrid.CommitEdit();
            }
        }
        public static void SelectRowByIndex(DataGrid dataGrid, int rowIndex)
        {
            try
            {
                object item = dataGrid.Items[rowIndex];
                dataGrid.SelectedItem = item;

                DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
                if (row == null)
                {
                    dataGrid.ScrollIntoView(item);
                    row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
                }

                if (row != null)
                {
                    DataGridCell cell = GetCell(dataGrid, row, 0);
                    if (cell != null)
                        cell.Focus();
                }
            }
            catch { }
        }
        public static DataGridCell GetCell(DataGrid dataGrid, DataGridRow rowContainer, int column)
        {
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                if (presenter == null)
                {
                    rowContainer.ApplyTemplate();
                    presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                }
                if (presenter != null)
                {
                    DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    if (cell == null)
                    {
                        dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                        cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    }
                    return cell;
                }
            }
            return null;
        }
        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
