using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace ForceTools
{
    /// <summary>
    /// Interaction logic for InvoiceGridPage.xaml
    /// </summary>
    public partial class InvoiceGridPage : Page, INotifyPropertyChanged
    {
        public DataTable dataTable { get { return _dataTable; } set { _dataTable = value; OnPropertyChanged(); } }
        private DataTable _dataTable;
        private DocumentStatuses _docStatusId;
        private OperationType _operationType;
        private MassEditOperationType _massEditOperationType;

        public event PropertyChangedEventHandler PropertyChanged;

        public InvoiceGridPage()
        {
            InitializeComponent();
        }
        public InvoiceGridPage(DocumentStatuses statusId, OperationType operationType) : this()
        {
            _operationType = operationType;
            _docStatusId = statusId;
            RefreshGridDataTable(operationType, statusId);
            SetPurchaseOrSaleLbl(operationType, statusId);
        }

        private void SetPurchaseOrSaleLbl(OperationType operationType, DocumentStatuses statusId)
        {
            string purchaseOrSaleOperationText = "";
            string purchaseOrSaleStatusText = "";

            switch (_operationType)
            {
                case OperationType.Purchase:
                    purchaseOrSaleOperationText = "Покупки";
                    break;
                case OperationType.Sale:
                    purchaseOrSaleOperationText = "Продажби";
                    break;
            }

            switch (_docStatusId)
            {
                case DocumentStatuses.AccountedDocuments:
                    purchaseOrSaleStatusText = "Всчики";
                    break;
                case DocumentStatuses.HeldDocuments:
                    purchaseOrSaleStatusText = "Задържани";
                    break;
                case DocumentStatuses.UnAccountedDocuments:
                    purchaseOrSaleStatusText = "Неосчетоводени";
                    break;
                case DocumentStatuses.ReadyToBeExportedDocuments:
                    purchaseOrSaleStatusText = "Готови за експорт";
                    break;
                case DocumentStatuses.ExportedDocuments:
                    purchaseOrSaleStatusText = "Експортирани";
                    break;
            }
            PurchaseOrSaleLbl.Content = $"{purchaseOrSaleOperationText} - {purchaseOrSaleStatusText}";
        }
        private void RefreshGridDataTable(OperationType operationType, DocumentStatuses ChosenFilter)
        {
            dataTable = InvoiceDataFilters.GetInvoiceDataTableByStatusAndOperation(operationType, ChosenFilter);
        }
        private void GetInvoiceIdAndOpenEditingWindow(object sender)
        {
            DataGrid dg = (DataGrid)sender;
            DataRowView dataRow = (DataRowView)dg.SelectedItem;
            if (dataRow == null) return;
            int FakID = Convert.ToInt32(dataRow.Row.ItemArray[0]);
            UiNavigationHelper.OpenInvoiceEditWindow(FakID, _operationType, _docStatusId);
        }
        private void OpenMassEdit(MassEditOperationType massEditOperationType)
        {
            MassNoteTb.Visibility = Visibility.Hidden;
            MassAccountTb.Visibility = Visibility.Hidden;
            MassAccStatusCb.Visibility = Visibility.Hidden;
            MassAccDateTb.Visibility = Visibility.Hidden;
            switch (massEditOperationType)
            {
                case MassEditOperationType.AccountingDate:
                    MassTypeLbl.Content = "Счетоводна дата";
                    MassAccDateTb.Visibility = Visibility.Visible;
                    break;
                case MassEditOperationType.AccountingStatus:
                    MassTypeLbl.Content = "Счетоводен статус";
                    MassAccStatusCb.Visibility = Visibility.Visible;
                    break;
                case MassEditOperationType.Account:
                    MassTypeLbl.Content = "Сметка";
                    MassAccountTb.Visibility = Visibility.Visible;
                    break;
                case MassEditOperationType.Note:
                    MassTypeLbl.Content = "Бележка";
                    MassNoteTb.Visibility = Visibility.Visible;
                    break;
            }
            MassSelectedCountLbl.Content = $"Брой избрани документи: {InvoicesDataGrid.SelectedItems.Count}";
            MassEditPopup.IsOpen = true;
            _massEditOperationType = massEditOperationType;
        }
        private void SaveMassEdit(MassEditOperationType massEditOperationType)
        {
            List<string> InvoicesToBeAffected = new List<string>();
            foreach (DataRowView dataRowView in InvoicesDataGrid.SelectedItems)
            {
                InvoicesToBeAffected.Add(dataRowView.Row.ItemArray[0].ToString());
            }
            switch (massEditOperationType)
            {
                case MassEditOperationType.AccountingDate: 
                    if (MassAccDateTb.Text == "") { MessageBox.Show("Полето не може да бъде празно"); return; }
                    InvoicesMassEditor.ChangeInvoicesDates(InvoicesToBeAffected, MassAccDateTb.Text);
                    break;
                case MassEditOperationType.AccountingStatus: 
                    if (MassAccStatusCb.SelectedIndex == -1) { MessageBox.Show("Полето не може да бъде празно"); return; }
                    InvoicesMassEditor.ChangeInvoicesStatuses(InvoicesToBeAffected, MassAccStatusCb.SelectedIndex);
                    break;
                case MassEditOperationType.Account: 
                    if (MassAccountTb.Text == "") { MessageBox.Show("Полето не може да бъде празно"); return; }
                    InvoicesMassEditor.ChangeInvoicesAccounts(InvoicesToBeAffected, MassAccountTb.Text);
                    break;
                case MassEditOperationType.Note: 
                    if (MassNoteTb.Text == "") { MessageBox.Show("Полето не може да бъде празно"); return; }
                    InvoicesMassEditor.ChangeInvoicesNotes(InvoicesToBeAffected, MassNoteTb.Text);
                    break;
            }
            RefreshGridDataTable(_operationType, _docStatusId);
            MassEditPopup.IsOpen = false;
        }

        private void NewInvoicesButton_Click(object sender, RoutedEventArgs e)
        {
            UiNavigationHelper.OpenNICWWindow(_operationType);
        }
        private void ImportFromExcelBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not Implemented yet");


            //string ExcelFileName;


            //OpenFileDialog ofd = new OpenFileDialog();
            //ofd.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            //if (ofd.ShowDialog() == true)
            //{
            //    ExcelFileName = ofd.FileName;


            //}
        }
        private void ImportFromPdfBtn_Click(object sender, RoutedEventArgs e)
        {
            UiNavigationHelper.OpenPdfUploaderWindow(_operationType);
        }
        private void InvoicesDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            GetInvoiceIdAndOpenEditingWindow(sender);
        }
        private void PreviewAndEditBtn_Click(object sender, RoutedEventArgs e)
        {
            GetInvoiceIdAndOpenEditingWindow(sender);
        }
        private void SearchBarBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SearchBarTb.IsVisible == true && SearchBarLbl.IsVisible == true)
            {
                SearchBarTb.Visibility = Visibility.Collapsed;
                SearchBarLbl.Visibility = Visibility.Collapsed;
            }
            else
            {
                SearchBarTb.Visibility = Visibility.Visible;
                SearchBarLbl.Visibility = Visibility.Visible;
                Keyboard.Focus(SearchBarTb);
            }

        }
        private void MassEditAccDate_Click(object sender, RoutedEventArgs e)
        {
            OpenMassEdit(MassEditOperationType.AccountingDate);
        }
        private void MassEditAccStatus_Click(object sender, RoutedEventArgs e)
        {
            OpenMassEdit(MassEditOperationType.AccountingStatus);
        }
        private void MassEditAccount_Click(object sender, RoutedEventArgs e)
        {
            OpenMassEdit(MassEditOperationType.Account);
        }
        private void MassEditNote_Click(object sender, RoutedEventArgs e)
        {
            OpenMassEdit(MassEditOperationType.Note);
        }
        private void MassSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveMassEdit(_massEditOperationType);
        }
        private void MassCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            MassEditPopup.IsOpen = false;
        }
        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var Res = MessageBox.Show($"Сигурни ли сте че искате да изтриете {InvoicesDataGrid.SelectedItems.Count} документа ?", "Изтриване на документи", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (Res != MessageBoxResult.Yes) return;
            List<string> InvoicesToBeDeletedList = new List<string>();
            foreach (DataRowView dataRowView in InvoicesDataGrid.SelectedItems)
            {
                InvoicesToBeDeletedList.Add(dataRowView.Row.ItemArray[0].ToString());
            }
            InvoicesMassEditor.DeleteInvoicesFromFakturiTable(InvoicesToBeDeletedList);
            RefreshGridDataTable(_operationType, _docStatusId);
        }

        private void Page_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }
        private void SearchBarTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            dataTable = InvoiceDataFilters.GetInvoiceDataTableByStatusAndOperationAndSearchText(SearchBarTb.Text, _operationType, _docStatusId);
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
