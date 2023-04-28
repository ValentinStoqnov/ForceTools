using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.Win32;
using ForceTools.WPF_Windows;
using ForceTools.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ForceTools
{
    /// <summary>
    /// Interaction logic for InvoiceGridPage.xaml
    /// </summary>
    public partial class InvoiceGridPage : Page, INotifyPropertyChanged
    {

        SqlConnection sqlConnection;
        SqlCommand SqCmd;
        SqlDataAdapter sqlDataAdapter;
        public DataTable dataTable { get { return _dataTable; } set { _dataTable = value; OnPropertyChanged();} }
        private DataTable _dataTable;
        private int DocStatusId;
        private string isPurchaseOrSale;
        private int MassEditType;

        public event PropertyChangedEventHandler PropertyChanged;

        public InvoiceGridPage()
        {
            InitializeComponent();
        }



        public InvoiceGridPage(int StatusId, string PurchaseOrSale) : this()
        {
            isPurchaseOrSale = PurchaseOrSale;
            DocStatusId = StatusId;
            BindDataGridtoSqlDatabase(DocStatusId);
            SetPurchaseOrSaleLbl();
        }

        private void SetPurchaseOrSaleLbl()
        {
            string TranslatedPorS = "";

            switch (isPurchaseOrSale)
            {
                case "Purchase":
                    TranslatedPorS = "Покупки";
                    break;
                case "Sale":
                    TranslatedPorS = "Продажби";
                    break;
            }

            switch (DocStatusId)
            {
                case 0:
                    PurchaseOrSaleLbl.Content = $"{TranslatedPorS} - Всчики";
                    break;
                case 1:
                    PurchaseOrSaleLbl.Content = $"{TranslatedPorS} - Задържани";
                    break;
                case 2:
                    PurchaseOrSaleLbl.Content = $"{TranslatedPorS} - Неосчетоводени";
                    break;
                case 3:
                    PurchaseOrSaleLbl.Content = $"{TranslatedPorS} - Готови за експорт";
                    break;
                case 4:
                    PurchaseOrSaleLbl.Content = $"{TranslatedPorS} - Експортирани";
                    break;
            }
        }

        public void BindDataGridtoSqlDatabase(int ChosenFilter)
        {
            sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString;
            sqlConnection.Open();
            SqCmd = new SqlCommand();

            switch (isPurchaseOrSale)
            {
                case "Purchase":
                    switch (ChosenFilter)
                    {
                        case 0:
                            SqCmd.CommandText = "Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, Kontragenti.Name,Kontragenti.EIK, Kontragenti.DDSNumber,Fakturi.DO, Fakturi.DDS, Fakturi.FullValue, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id JOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE PurchaseOrSale = 'Purchase'";
                            break;
                        case 1:
                            SqCmd.CommandText = "Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, Kontragenti.Name,Kontragenti.EIK, Kontragenti.DDSNumber,Fakturi.DO, Fakturi.DDS, Fakturi.FullValue, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id JOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE (AccountingStatusId = 1 and PurchaseOrSale = 'Purchase')";
                            break;
                        case 2:
                            SqCmd.CommandText = "Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, Kontragenti.Name,Kontragenti.EIK, Kontragenti.DDSNumber,Fakturi.DO, Fakturi.DDS, Fakturi.FullValue, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id JOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE (AccountingStatusId = 2 and PurchaseOrSale = 'Purchase')";
                            break;
                        case 3:
                            SqCmd.CommandText = "Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, Kontragenti.Name,Kontragenti.EIK, Kontragenti.DDSNumber,Fakturi.DO, Fakturi.DDS, Fakturi.FullValue, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id JOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE (AccountingStatusId = 3 and PurchaseOrSale = 'Purchase')";
                            break;
                        case 4:
                            SqCmd.CommandText = "Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, Kontragenti.Name,Kontragenti.EIK, Kontragenti.DDSNumber,Fakturi.DO, Fakturi.DDS, Fakturi.FullValue, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id JOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE (AccountingStatusId = 4 and PurchaseOrSale = 'Purchase')";
                            break;

                    }
                    break;
                case "Sale":
                    switch (ChosenFilter)
                    {
                        case 0:
                            SqCmd.CommandText = "Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, Kontragenti.Name,Kontragenti.EIK, Kontragenti.DDSNumber,Fakturi.DO, Fakturi.DDS, Fakturi.FullValue, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id JOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE PurchaseOrSale = 'Sale'";
                            break;
                        case 1:
                            SqCmd.CommandText = "Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, Kontragenti.Name,Kontragenti.EIK, Kontragenti.DDSNumber,Fakturi.DO, Fakturi.DDS, Fakturi.FullValue, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id JOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE (AccountingStatusId = 1 and PurchaseOrSale = 'Sale')";
                            break;
                        case 2:
                            SqCmd.CommandText = "Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, Kontragenti.Name,Kontragenti.EIK, Kontragenti.DDSNumber,Fakturi.DO, Fakturi.DDS, Fakturi.FullValue, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id JOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE (AccountingStatusId = 2 and PurchaseOrSale = 'Sale')";
                            break;
                        case 3:
                            SqCmd.CommandText = "Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, Kontragenti.Name,Kontragenti.EIK, Kontragenti.DDSNumber,Fakturi.DO, Fakturi.DDS, Fakturi.FullValue, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id JOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE (AccountingStatusId = 3 and PurchaseOrSale = 'Sale')";
                            break;
                        case 4:
                            SqCmd.CommandText = "Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, Kontragenti.Name,Kontragenti.EIK, Kontragenti.DDSNumber,Fakturi.DO, Fakturi.DDS, Fakturi.FullValue, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id JOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE (AccountingStatusId = 4 and PurchaseOrSale = 'Sale')";
                            break;

                    }
                    break;
            }
            SqCmd.Connection = sqlConnection;
            sqlDataAdapter = new SqlDataAdapter(SqCmd);
            dataTable = new DataTable("Fakturi");
            sqlDataAdapter.Fill(dataTable);
            //InvoicesDataGrid.ItemsSource = dataTable.DefaultView; ///////////////THIS WAS BEFORE THE BINDING
            sqlConnection.Close();
        }

        private void NewInvoicesButton_Click(object sender, RoutedEventArgs e)
        {
            NICW InvCreWin = new NICW(isPurchaseOrSale);
            InvCreWin.ShowDialog();
        }

        private void ImportFromExcelBtn_Click(object sender, RoutedEventArgs e)
        {
            string ExcelFileName;


            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            if (ofd.ShowDialog() == true)
            {
                ExcelFileName = ofd.FileName;


            }
        }

        private void Page_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            MessageBox.Show("a key was pressed");
        }

        private void ImportFromPdfBtn_Click(object sender, RoutedEventArgs e)
        {
            PdfUploaderWindow PUW = new PdfUploaderWindow(isPurchaseOrSale);
            PUW.ShowDialog();
        }

        private void InvoicesDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid dg = (DataGrid)sender;
            DataRowView dataRow = (DataRowView)dg.SelectedItem;
            if (dataRow != null)
            {
                int FakID = Convert.ToInt32(dataRow.Row.ItemArray[0]);
                InvoiceEditWindow IEW = new InvoiceEditWindow(FakID, DocStatusId,isPurchaseOrSale);
                IEW.ShowDialog();
            }
            else
            {
                return;
            }
        }

        private void PreviewAndEditBtn_Click(object sender, RoutedEventArgs e)
        {
            DataGrid dg = InvoicesDataGrid;
            DataRowView dataRow = (DataRowView)dg.SelectedItem;
            if (dataRow != null)
            {
                int FakID = Convert.ToInt32(dataRow.Row.ItemArray[0]);
                InvoiceEditWindow IEW = new InvoiceEditWindow(FakID, DocStatusId,isPurchaseOrSale);
                IEW.ShowDialog();
            }
            else
            {
                return;
            }
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

        private void SearchBarTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                using (SqCmd = new SqlCommand())
                {
                    switch (isPurchaseOrSale)
                    {
                        case "Purchase":
                            switch (DocStatusId)
                            {
                                case 0:
                                    SqCmd.CommandText = $"Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, Kontragenti.Name,Kontragenti.EIK, Kontragenti.DDSNumber,Fakturi.DO, Fakturi.DDS, Fakturi.FullValue, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id JOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE PurchaseOrSale = 'Purchase' and (Number like N'%{SearchBarTb.Text}%' Or AccDate like N'%{SearchBarTb.Text}%' Or Date like N'%{SearchBarTb.Text}%' Or Name like N'%{SearchBarTb.Text}%' Or EIK like N'%{SearchBarTb.Text}%' Or DDSNumber like N'%{SearchBarTb.Text}%' Or DO like N'%{SearchBarTb.Text}%' Or DDS like N'%{SearchBarTb.Text}%' Or FullValue like N'%{SearchBarTb.Text}%' Or Account like N'%{SearchBarTb.Text}%' Or InCashAccount like N'%{SearchBarTb.Text}%' Or Note like N'%{SearchBarTb.Text}%' Or TypeName like N'%{SearchBarTb.Text}%' Or DealName like N'%{SearchBarTb.Text}%')";
                                    break;
                                case 1:
                                    SqCmd.CommandText = $"Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, Kontragenti.Name,Kontragenti.EIK, Kontragenti.DDSNumber,Fakturi.DO, Fakturi.DDS, Fakturi.FullValue, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id JOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE (AccountingStatusId = 1 and PurchaseOrSale = 'Purchase') and (Number like N'%{SearchBarTb.Text}%' Or AccDate like N'%{SearchBarTb.Text}%' Or Date like N'%{SearchBarTb.Text}%' Or Name like N'%{SearchBarTb.Text}%' Or EIK like N'%{SearchBarTb.Text}%' Or DDSNumber like N'%{SearchBarTb.Text}%' Or DO like N'%{SearchBarTb.Text}%' Or DDS like N'%{SearchBarTb.Text}%' Or FullValue like N'%{SearchBarTb.Text}%' Or Account like N'%{SearchBarTb.Text}%' Or InCashAccount like N'%{SearchBarTb.Text}%' Or Note like N'%{SearchBarTb.Text}%' Or TypeName like N'%{SearchBarTb.Text}%' Or DealName like N'%{SearchBarTb.Text}%')";
                                    break;
                                case 2:
                                    SqCmd.CommandText = $"Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, Kontragenti.Name,Kontragenti.EIK, Kontragenti.DDSNumber,Fakturi.DO, Fakturi.DDS, Fakturi.FullValue, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id JOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE (AccountingStatusId = 2 and PurchaseOrSale = 'Purchase') and (Number like N'%{SearchBarTb.Text}%' Or AccDate like N'%{SearchBarTb.Text}%' Or Date like N'%{SearchBarTb.Text}%' Or Name like N'%{SearchBarTb.Text}%' Or EIK like N'%{SearchBarTb.Text}%' Or DDSNumber like N'%{SearchBarTb.Text}%' Or DO like N'%{SearchBarTb.Text}%' Or DDS like N'%{SearchBarTb.Text}%' Or FullValue like N'%{SearchBarTb.Text}%' Or Account like N'%{SearchBarTb.Text}%' Or InCashAccount like N'%{SearchBarTb.Text}%' Or Note like N'%{SearchBarTb.Text}%' Or TypeName like N'%{SearchBarTb.Text}%' Or DealName like N'%{SearchBarTb.Text}%')";
                                    break;
                                case 3:
                                    SqCmd.CommandText = $"Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, Kontragenti.Name,Kontragenti.EIK, Kontragenti.DDSNumber,Fakturi.DO, Fakturi.DDS, Fakturi.FullValue, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id JOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE (AccountingStatusId = 3 and PurchaseOrSale = 'Purchase') and (Number like N'%{SearchBarTb.Text}%' Or AccDate like N'%{SearchBarTb.Text}%' Or Date like N'%{SearchBarTb.Text}%' Or Name like N'%{SearchBarTb.Text}%' Or EIK like N'%{SearchBarTb.Text}%' Or DDSNumber like N'%{SearchBarTb.Text}%' Or DO like N'%{SearchBarTb.Text}%' Or DDS like N'%{SearchBarTb.Text}%' Or FullValue like N'%{SearchBarTb.Text}%' Or Account like N'%{SearchBarTb.Text}%' Or InCashAccount like N'%{SearchBarTb.Text}%' Or Note like N'%{SearchBarTb.Text}%' Or TypeName like N'%{SearchBarTb.Text}%' Or DealName like N'%{SearchBarTb.Text}%')";
                                    break;
                                case 4:
                                    SqCmd.CommandText = $"Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, Kontragenti.Name,Kontragenti.EIK, Kontragenti.DDSNumber,Fakturi.DO, Fakturi.DDS, Fakturi.FullValue, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id JOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE (AccountingStatusId = 4 and PurchaseOrSale = 'Purchase') and (Number like N'%{SearchBarTb.Text}%' Or AccDate like N'%{SearchBarTb.Text}%' Or Date like N'%{SearchBarTb.Text}%' Or Name like N'%{SearchBarTb.Text}%' Or EIK like N'%{SearchBarTb.Text}%' Or DDSNumber like N'%{SearchBarTb.Text}%' Or DO like N'%{SearchBarTb.Text}%' Or DDS like N'%{SearchBarTb.Text}%' Or FullValue like N'%{SearchBarTb.Text}%' Or Account like N'%{SearchBarTb.Text}%' Or InCashAccount like N'%{SearchBarTb.Text}%' Or Note like N'%{SearchBarTb.Text}%' Or TypeName like N'%{SearchBarTb.Text}%' Or DealName like N'%{SearchBarTb.Text}%')";
                                    break;

                            }
                            break;
                        case "Sale":
                            switch (DocStatusId)
                            {
                                case 0:
                                    SqCmd.CommandText = $"Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, Kontragenti.Name,Kontragenti.EIK, Kontragenti.DDSNumber,Fakturi.DO, Fakturi.DDS, Fakturi.FullValue, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id JOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE PurchaseOrSale = 'Sale' and (Number like N'%{SearchBarTb.Text}%' Or AccDate like N'%{SearchBarTb.Text}%' Or Date like N'%{SearchBarTb.Text}%' Or Name like N'%{SearchBarTb.Text}%' Or EIK like N'%{SearchBarTb.Text}%' Or DDSNumber like N'%{SearchBarTb.Text}%' Or DO like N'%{SearchBarTb.Text}%' Or DDS like N'%{SearchBarTb.Text}%' Or FullValue like N'%{SearchBarTb.Text}%' Or Account like N'%{SearchBarTb.Text}%' Or InCashAccount like N'%{SearchBarTb.Text}%' Or Note like N'%{SearchBarTb.Text}%' Or TypeName like N'%{SearchBarTb.Text}%' Or DealName like N'%{SearchBarTb.Text}%')";
                                    break;
                                case 1:
                                    SqCmd.CommandText = $"Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, Kontragenti.Name,Kontragenti.EIK, Kontragenti.DDSNumber,Fakturi.DO, Fakturi.DDS, Fakturi.FullValue, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id JOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE (AccountingStatusId = 1 and PurchaseOrSale = 'Sale') and (Number like N'%{SearchBarTb.Text}%' Or AccDate like N'%{SearchBarTb.Text}%' Or Date like N'%{SearchBarTb.Text}%' Or Name like N'%{SearchBarTb.Text}%' Or EIK like N'%{SearchBarTb.Text}%' Or DDSNumber like N'%{SearchBarTb.Text}%' Or DO like N'%{SearchBarTb.Text}%' Or DDS like N'%{SearchBarTb.Text}%' Or FullValue like N'%{SearchBarTb.Text}%' Or Account like N'%{SearchBarTb.Text}%' Or InCashAccount like N'%{SearchBarTb.Text}%' Or Note like N'%{SearchBarTb.Text}%' Or TypeName like N'%{SearchBarTb.Text}%' Or DealName like N'%{SearchBarTb.Text}%')";
                                    break;
                                case 2:
                                    SqCmd.CommandText = $"Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, Kontragenti.Name,Kontragenti.EIK, Kontragenti.DDSNumber,Fakturi.DO, Fakturi.DDS, Fakturi.FullValue, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id JOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE (AccountingStatusId = 2 and PurchaseOrSale = 'Sale') and (Number like N'%{SearchBarTb.Text}%' Or AccDate like N'%{SearchBarTb.Text}%' Or Date like N'%{SearchBarTb.Text}%' Or Name like N'%{SearchBarTb.Text}%' Or EIK like N'%{SearchBarTb.Text}%' Or DDSNumber like N'%{SearchBarTb.Text}%' Or DO like N'%{SearchBarTb.Text}%' Or DDS like N'%{SearchBarTb.Text}%' Or FullValue like N'%{SearchBarTb.Text}%' Or Account like N'%{SearchBarTb.Text}%' Or InCashAccount like N'%{SearchBarTb.Text}%' Or Note like N'%{SearchBarTb.Text}%' Or TypeName like N'%{SearchBarTb.Text}%' Or DealName like N'%{SearchBarTb.Text}%')";
                                    break;
                                case 3:
                                    SqCmd.CommandText = $"Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, Kontragenti.Name,Kontragenti.EIK, Kontragenti.DDSNumber,Fakturi.DO, Fakturi.DDS, Fakturi.FullValue, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id JOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE (AccountingStatusId = 3 and PurchaseOrSale = 'Sale') and (Number like N'%{SearchBarTb.Text}%' Or AccDate like N'%{SearchBarTb.Text}%' Or Date like N'%{SearchBarTb.Text}%' Or Name like N'%{SearchBarTb.Text}%' Or EIK like N'%{SearchBarTb.Text}%' Or DDSNumber like N'%{SearchBarTb.Text}%' Or DO like N'%{SearchBarTb.Text}%' Or DDS like N'%{SearchBarTb.Text}%' Or FullValue like N'%{SearchBarTb.Text}%' Or Account like N'%{SearchBarTb.Text}%' Or InCashAccount like N'%{SearchBarTb.Text}%' Or Note like N'%{SearchBarTb.Text}%' Or TypeName like N'%{SearchBarTb.Text}%' Or DealName like N'%{SearchBarTb.Text}%')";
                                    break;
                                case 4:
                                    SqCmd.CommandText = $"Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, Kontragenti.Name,Kontragenti.EIK, Kontragenti.DDSNumber,Fakturi.DO, Fakturi.DDS, Fakturi.FullValue, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id JOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE (AccountingStatusId = 4 and PurchaseOrSale = 'Sale') and (Number like N'%{SearchBarTb.Text}%' Or AccDate like N'%{SearchBarTb.Text}%' Or Date like N'%{SearchBarTb.Text}%' Or Name like N'%{SearchBarTb.Text}%' Or EIK like N'%{SearchBarTb.Text}%' Or DDSNumber like N'%{SearchBarTb.Text}%' Or DO like N'%{SearchBarTb.Text}%' Or DDS like N'%{SearchBarTb.Text}%' Or FullValue like N'%{SearchBarTb.Text}%' Or Account like N'%{SearchBarTb.Text}%' Or InCashAccount like N'%{SearchBarTb.Text}%' Or Note like N'%{SearchBarTb.Text}%' Or TypeName like N'%{SearchBarTb.Text}%' Or DealName like N'%{SearchBarTb.Text}%')";
                                    break;

                            }
                            break;
                    }
                    SqCmd.Connection = sqlConnection;
                    sqlDataAdapter = new SqlDataAdapter(SqCmd);
                    dataTable = new DataTable("Fakturi");
                    sqlDataAdapter.Fill(dataTable);
                    InvoicesDataGrid.ItemsSource = dataTable.DefaultView;
                }
            }
        }

        private void MassEditAccDate_Click(object sender, RoutedEventArgs e)
        {
            OpenMassEdit(1);
        }

        private void MassEditAccStatus_Click(object sender, RoutedEventArgs e)
        {
            OpenMassEdit(2);
        }

        private void MassEditAccount_Click(object sender, RoutedEventArgs e)
        {
            OpenMassEdit(3);
        }

        private void MassEditNote_Click(object sender, RoutedEventArgs e)
        {
            OpenMassEdit(4);
        }

        private void MassSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveMassEdit(MassEditType);
        }

        private void MassCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            MassEditPopup.IsOpen = false;
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var Res = MessageBox.Show($"Сигурни ли сте че искате да изтриете {InvoicesDataGrid.SelectedItems.Count} документа ?", "Изтриване на документи", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

            if (Res == MessageBoxResult.Yes)
            {
                foreach (DataRowView dataRowView in InvoicesDataGrid.SelectedItems)
                {

                    using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
                    {
                        using (SqCmd = new SqlCommand())
                        {
                            SqCmd.CommandText = $"DELETE FROM Fakturi WHERE id = {dataRowView.Row.ItemArray[0].ToString()}";
                        }
                        sqlConnection.Open();
                        SqCmd.Connection = sqlConnection;
                        SqCmd.ExecuteNonQuery();
                        sqlConnection.Close();
                    }
                }
                BindDataGridtoSqlDatabase(DocStatusId);
            }
            else
            {
                return;
            }
        }

        private void OpenMassEdit(int Case)
        {
            MassEditPopup.IsOpen = true;
            MassSelectedCountLbl.Content = $"Брой избрани документи: {InvoicesDataGrid.SelectedItems.Count}";

            switch (Case)
            {
                case 1: //Accounting Date
                    MassTypeLbl.Content = "Счетоводна дата";
                    MassNoteTb.Visibility = Visibility.Hidden;
                    MassAccountTb.Visibility = Visibility.Hidden;
                    MassAccStatusCb.Visibility = Visibility.Hidden;
                    MassAccDateTb.Visibility = Visibility.Visible;
                    break;
                case 2: //Accounting status
                    MassTypeLbl.Content = "Счетоводен статус";
                    MassNoteTb.Visibility = Visibility.Hidden;
                    MassAccountTb.Visibility = Visibility.Hidden;
                    MassAccDateTb.Visibility = Visibility.Hidden;
                    MassAccStatusCb.Visibility = Visibility.Visible;
                    break;
                case 3: //Account
                    MassTypeLbl.Content = "Сметка";
                    MassNoteTb.Visibility = Visibility.Hidden;
                    MassAccountTb.Visibility = Visibility.Visible;
                    MassAccDateTb.Visibility = Visibility.Hidden;
                    MassAccStatusCb.Visibility = Visibility.Hidden;
                    break;
                case 4: //Note
                    MassTypeLbl.Content = "Бележка";
                    MassNoteTb.Visibility = Visibility.Visible;
                    MassAccountTb.Visibility = Visibility.Hidden;
                    MassAccDateTb.Visibility = Visibility.Hidden;
                    MassAccStatusCb.Visibility = Visibility.Hidden;
                    break;
            }

            MassEditType = Case;
        }
        private void SaveMassEdit(int Case)
        {
            switch (Case)
            {
                case 1: //Accounting Date
                    foreach (DataRowView dataRowView in InvoicesDataGrid.SelectedItems)
                    {

                        using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
                        {
                            using (SqCmd = new SqlCommand())
                            {
                                SqCmd.CommandText = $"select * from Fakturi where id = {dataRowView.Row.ItemArray[0].ToString()}";
                            }
                            SqCmd.Connection = sqlConnection;
                            sqlDataAdapter = new SqlDataAdapter(SqCmd);
                            DataTable UpdateDt = new DataTable("UpdateDt");
                            sqlDataAdapter.Fill(UpdateDt);

                            if (MassAccDateTb.Text != "")
                            {
                                UpdateDt.Rows[0][3] = MassAccDateTb.Text;
                            }
                            else
                            {
                                MessageBox.Show("Полето не може да бъде празно");
                            }
                            SqlCommandBuilder builder = new SqlCommandBuilder(sqlDataAdapter);
                            sqlDataAdapter.UpdateCommand = builder.GetUpdateCommand();
                            sqlDataAdapter.Update(UpdateDt);
                        }
                    }
                    break;
                case 2: //Accounting status
                    foreach (DataRowView dataRowView in InvoicesDataGrid.SelectedItems)
                    {

                        using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
                        {
                            using (SqCmd = new SqlCommand())
                            {
                                SqCmd.CommandText = $"select * from Fakturi where id = {dataRowView.Row.ItemArray[0].ToString()}";
                            }
                            SqCmd.Connection = sqlConnection;
                            sqlDataAdapter = new SqlDataAdapter(SqCmd);
                            DataTable UpdateDt = new DataTable("UpdateDt");
                            sqlDataAdapter.Fill(UpdateDt);

                            switch (MassAccStatusCb.SelectedIndex)
                            {
                                case 0:
                                    UpdateDt.Rows[0][15] = 1;
                                    break;
                                case 1:
                                    UpdateDt.Rows[0][15] = 2;
                                    break;
                                case 2:
                                    UpdateDt.Rows[0][15] = 3;
                                    break;
                                case 3:
                                    UpdateDt.Rows[0][15] = 4;
                                    break;
                                case 4:
                                    UpdateDt.Rows[0][15] = 5;
                                    break;
                            }



                            SqlCommandBuilder builder = new SqlCommandBuilder(sqlDataAdapter);
                            sqlDataAdapter.UpdateCommand = builder.GetUpdateCommand();
                            sqlDataAdapter.Update(UpdateDt);
                        }
                    }
                    break;
                case 3: //Account
                    foreach (DataRowView dataRowView in InvoicesDataGrid.SelectedItems)
                    {

                        using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
                        {
                            using (SqCmd = new SqlCommand())
                            {
                                SqCmd.CommandText = $"select * from Fakturi where id = {dataRowView.Row.ItemArray[0].ToString()}";
                            }
                            SqCmd.Connection = sqlConnection;
                            sqlDataAdapter = new SqlDataAdapter(SqCmd);
                            DataTable UpdateDt = new DataTable("UpdateDt");
                            sqlDataAdapter.Fill(UpdateDt);

                            if (MassAccountTb.Text != "")
                            {
                                UpdateDt.Rows[0][11] = MassAccountTb.Text;
                            }
                            else
                            {
                                MessageBox.Show("Полето не може да бъде празно");
                            }
                            SqlCommandBuilder builder = new SqlCommandBuilder(sqlDataAdapter);
                            sqlDataAdapter.UpdateCommand = builder.GetUpdateCommand();
                            sqlDataAdapter.Update(UpdateDt);
                        }
                    }
                    break;
                case 4: //Note
                    foreach (DataRowView dataRowView in InvoicesDataGrid.SelectedItems)
                    {

                        using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
                        {
                            using (SqCmd = new SqlCommand())
                            {
                                SqCmd.CommandText = $"select * from Fakturi where id = {dataRowView.Row.ItemArray[0].ToString()}";
                            }
                            SqCmd.Connection = sqlConnection;
                            sqlDataAdapter = new SqlDataAdapter(SqCmd);
                            DataTable UpdateDt = new DataTable("UpdateDt");
                            sqlDataAdapter.Fill(UpdateDt);

                            if (MassNoteTb.Text != "")
                            {
                                UpdateDt.Rows[0][13] = MassNoteTb.Text;
                            }
                            else
                            {
                                MessageBox.Show("Полето не може да бъде празно");
                            }
                            SqlCommandBuilder builder = new SqlCommandBuilder(sqlDataAdapter);
                            sqlDataAdapter.UpdateCommand = builder.GetUpdateCommand();
                            sqlDataAdapter.Update(UpdateDt);
                        }
                    }
                    break;
            }
            BindDataGridtoSqlDatabase(DocStatusId);
            MassEditPopup.IsOpen = false;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
