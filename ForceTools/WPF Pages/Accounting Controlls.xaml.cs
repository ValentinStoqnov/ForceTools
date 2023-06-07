using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using System.Configuration;
using Excel = Microsoft.Office.Interop.Excel;           
using iTextSharp.text;
using System.Drawing;
using Microsoft.SqlServer.Server;
using System.Globalization;

namespace ForceTools
{
    /// <summary>
    /// Interaction logic for Accounting_Controlls.xaml
    /// </summary>
    public partial class Accounting_Controlls : Page
    {

        public static SqlConnection sqlConnection;
        public static SqlCommand SqCmd = new SqlCommand();
        public static SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(SqCmd);
        public DataTable dataTable = new DataTable("CountingDt");

        public string NewCount { get; set; }
        public string UnAcCount { get; set; }
        public string RdyExpCount { get; set; }
        public string ExportedCount { get; set; }
        public string AllCount { get; set; }

        private string isPurchaseOrSale; // Continue From here => change to enum to change it in mainwindow too !!!!!!!!

        public Accounting_Controlls()
        {
            InitializeComponent();
        }

        public Accounting_Controlls(string PurchaseOrSale) : this()
        {
            isPurchaseOrSale = PurchaseOrSale;
            GetInvoicesCount();
            SetLabelPorS();
        }

        private void SetLabelPorS()
        {
            switch (isPurchaseOrSale)
            {
                case "Purchase":
                    labelPurchasesSales.Content = "Покупки";
                    break;
                case "Sale":
                    labelPurchasesSales.Content = "Продажби";
                    break;
            }
        }

        private void GetInvoicesCount()
        {
            switch (isPurchaseOrSale)
            {
                case "Purchase":
                    using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
                    {
                        sqlConnection.Open();
                        SqCmd.Connection = sqlConnection;

                        SqCmd.CommandText = "select count (*) as Counting from Fakturi where (AccountingStatusId = 1 and PurchaseOrSale = 'Purchase')";
                        dataTable.Clear();
                        sqlDataAdapter.Fill(dataTable);
                        NewCount = $"( {dataTable.Rows[0].Field<int>("Counting")} )";

                        SqCmd.CommandText = "select count (*) as Counting from Fakturi where (AccountingStatusId = 2 and PurchaseOrSale = 'Purchase')";
                        dataTable.Clear();
                        sqlDataAdapter.Fill(dataTable);
                        UnAcCount = $"( {dataTable.Rows[0].Field<int>("Counting")} )";

                        SqCmd.CommandText = "select count (*) as Counting from Fakturi where (AccountingStatusId = 3 and PurchaseOrSale = 'Purchase')";
                        dataTable.Clear();
                        sqlDataAdapter.Fill(dataTable);
                        RdyExpCount = $"( {dataTable.Rows[0].Field<int>("Counting")} )";

                        SqCmd.CommandText = "select count (*) as Counting from Fakturi where (AccountingStatusId = 4 and PurchaseOrSale = 'Purchase')";
                        dataTable.Clear();
                        sqlDataAdapter.Fill(dataTable);
                        ExportedCount = $"( {dataTable.Rows[0].Field<int>("Counting")} )";

                        SqCmd.CommandText = "select count (*) as Counting from Fakturi where PurchaseOrSale = 'Purchase'";
                        dataTable.Clear();
                        sqlDataAdapter.Fill(dataTable);
                        AllCount = $"( {dataTable.Rows[0].Field<int>("Counting")} )";

                        dataTable.Clear();
                        dataTable.Columns.Clear();
                    }
                    sqlConnection.Close();
                    break;
                case "Sale":
                    using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
                    {
                        sqlConnection.Open();
                        SqCmd.Connection = sqlConnection;

                        SqCmd.CommandText = "select count (*) as Counting from Fakturi where (AccountingStatusId = 1 and PurchaseOrSale = 'Sale')";
                        dataTable.Clear();
                        sqlDataAdapter.Fill(dataTable);
                        NewCount = $"( {dataTable.Rows[0].Field<int>("Counting")} )";

                        SqCmd.CommandText = "select count (*) as Counting from Fakturi where (AccountingStatusId = 2 and PurchaseOrSale = 'Sale')";
                        dataTable.Clear();
                        sqlDataAdapter.Fill(dataTable);
                        UnAcCount = $"( {dataTable.Rows[0].Field<int>("Counting")} )";

                        SqCmd.CommandText = "select count (*) as Counting from Fakturi where (AccountingStatusId = 3 and PurchaseOrSale = 'Sale')";
                        dataTable.Clear();
                        sqlDataAdapter.Fill(dataTable);
                        RdyExpCount = $"( {dataTable.Rows[0].Field<int>("Counting")} )";

                        SqCmd.CommandText = "select count (*) as Counting from Fakturi where (AccountingStatusId = 4 and PurchaseOrSale = 'Sale')";
                        dataTable.Clear();
                        sqlDataAdapter.Fill(dataTable);
                        ExportedCount = $"( {dataTable.Rows[0].Field<int>("Counting")} )";

                        SqCmd.CommandText = "select count (*) as Counting from Fakturi where PurchaseOrSale = 'Sale'";
                        dataTable.Clear();
                        sqlDataAdapter.Fill(dataTable);
                        AllCount = $"( {dataTable.Rows[0].Field<int>("Counting")} )";

                        dataTable.Clear();
                        dataTable.Columns.Clear();
                    }
                    sqlConnection.Close();
                    break;
            }
        }

        private void AllInvoicesButton_Click(object sender, RoutedEventArgs e)
        {
            InvoiceGridPage igp = new InvoiceGridPage(0, isPurchaseOrSale);
            MainWindow mw = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive) as MainWindow;
            mw.ContentFrame.Content = igp.Content;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InvoiceGridPage igp = new InvoiceGridPage(1, isPurchaseOrSale);
            MainWindow mw = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive) as MainWindow;
            mw.ContentFrame.Content = igp.Content;
        }

        private void UnAccountedInvoicesBtn_Click(object sender, RoutedEventArgs e)
        {
            InvoiceGridPage igp = new InvoiceGridPage(2, isPurchaseOrSale);
            MainWindow mw = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive) as MainWindow;
            mw.ContentFrame.Content = igp.Content;
        }

        private void RdyToBeExportedBtn_Click(object sender, RoutedEventArgs e)
        {
            InvoiceGridPage igp = new InvoiceGridPage(3, isPurchaseOrSale);
            MainWindow mw = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive) as MainWindow;
            mw.ContentFrame.Content = igp.Content;
        }

        private void ExportedInvoicesBtn_Click(object sender, RoutedEventArgs e)
        {
            InvoiceGridPage igp = new InvoiceGridPage(4, isPurchaseOrSale);
            MainWindow mw = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive) as MainWindow;
            mw.ContentFrame.Content = igp.Content;
        }

        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            //Connecting to SQL and Filling datatable
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                dataTable.Clear();
                sqlConnection.Open();
                SqCmd.Connection = sqlConnection;
                SqCmd.CommandText = "Select DocumentTypes.TypeName as 'Тип Документ', Fakturi.Number as Номер, Fakturi.Date as Дата, Kontragenti.Name as Контрагент, Kontragenti.EIK as ЕИК, Kontragenti.DDSNumber as  'ДДС Номер' , Fakturi.DO as 'Данъчна основа' , Fakturi.DDS as ДДС , Fakturi.FullValue as Общо ,Fakturi.InCashAccount as 'В брой', Fakturi.Account as Сметка, Fakturi.Note as Бележка, DocumentTypes.Id as 'Код Тип Документ', KindOfDeals.Id as 'Код Тип Сделка', AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id JOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE AccountingStatusId = 3";
                sqlDataAdapter.Fill(dataTable);
            }
            sqlConnection.Close();

            // Opening Excel and Filling WorkSheet from datatable
            var ExcelApplication = new Excel.Application();
            ExcelApplication.Visible = true;
            ExcelApplication.Workbooks.Add();
            Excel.Worksheet workSheet = (Excel.Worksheet)ExcelApplication.ActiveSheet;
            workSheet.Name = "Sheet1";

            try
            {
                if (dataTable == null || dataTable.Columns.Count == 0)
                    throw new Exception("ExportToExcel: Null or empty input table!\n");


                // Getting and filling columns
                for (var i = 0; i < dataTable.Columns.Count; i++)
                {
                    workSheet.Cells[1, i + 1] = dataTable.Columns[i].ColumnName;
                }

                // Getting and filling rows
                for (var i = 0; i < dataTable.Rows.Count; i++)
                {

                    for (var j = 0; j < dataTable.Columns.Count; j++)
                    {
                        //Filling Excel rows with raw data from datatable if its not Data column
                        if (j != 2)
                        {
                            workSheet.Cells[i + 2, j + 1] = dataTable.Rows[i][j];
                        }
                        //Filling with Converted Data if its Data column
                        else
                        {
                            workSheet.Cells[i + 2, j + 1] = dataTable.Rows[i].Field<DateTime>("Дата").ToString("dd,MM,yyyy", CultureInfo.InvariantCulture).Replace(",", ".");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ExportToExcel: \n" + ex.Message);
            }
            finally
            {
                // Formating Columns
                for (var i = 1; i < dataTable.Columns.Count + 1; i++)
                {
                    ((Excel.Range)workSheet.Columns[i]).ColumnWidth = 12;
                    ExcelApplication.Cells[1, i].Interior.Color = ColorTranslator.FromHtml("#0078D4");
                    ExcelApplication.Cells[1, i].Font.Color = Excel.XlRgbColor.rgbWhite;
                }
                ((Excel.Range)workSheet.Columns[3]).AutoFit();
                ((Excel.Range)workSheet.Columns[9]).AutoFit();
                dataTable.Clear();
            }

        }
    }
}

