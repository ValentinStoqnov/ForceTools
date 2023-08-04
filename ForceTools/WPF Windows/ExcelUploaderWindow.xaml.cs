using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
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

        public event PropertyChangedEventHandler PropertyChanged;

        public ExcelUploaderWindow(OperationType operationType)
        {
            _operationType = operationType;
            InitializeComponent();
        }

        private void UploadExcelBtn_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FileSystemHelper.OpenFileDialogAndGetExcelFilePath();
            ExcelReader excelReader = new ExcelReader();
            excelDataTable = excelReader.GetDataTable(filePath);
            ExcelDataGrid.ItemsSource = excelDataTable.DefaultView;
            CreateComboBoxes();
        }
        private void ConfirmFinalEditBtn_Click(object sender, RoutedEventArgs e)
        {
            int currentRow = 0;
            int totalRows = excelDataTable.Rows.Count;
            while (currentRow < totalRows)
            {
                InvoiceSingleEditor.InsertNewInvoiceInSqlTableFromExcelUploader(_operationType, currentRow, excelDataTable);
                currentRow++;
            }
            MessageBox.Show($"Добавени са {totalRows} документа.");
            this.Close();
            UiNavigationHelper.MainWindow.ContentFrame.Content = new InvoiceGridPage(DocumentStatuses.UnAccountedDocuments, _operationType);
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
                ExcelExtractedDataInterpreter interpreter = new ExcelExtractedDataInterpreter(_operationType,currentRow,comboBoxList,excelDataTable);
                FinalEditDataTable.Rows.Add(interpreter.GetInterpreterDataRow());
                currentRow++;
            }

            ExcelDataGrid.Visibility = Visibility.Hidden;
            AddDocumentsBtn.Visibility = Visibility.Hidden;
            FinalEditDataGrid.Visibility = Visibility.Visible;
            ConfirmFinalEditBtn.Visibility = Visibility.Visible;
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
                comboBoxList.Add(newCb);
                ComboBoxesStackPanel.Children.Add(newCb);
                newCb.SelectionChanged += NewCb_SelectionChanged;
            }
        }
        private void NewCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboBoxSelectedIndexesList.Clear();
            foreach (var comboBox in comboBoxList)
            {
                comboBoxSelectedIndexesList.Add(comboBox.SelectedIndex);
                Console.WriteLine(comboBox.SelectedIndex);
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
            }
        }
        private void ExcelDataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ComboBoxesScrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset);
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
