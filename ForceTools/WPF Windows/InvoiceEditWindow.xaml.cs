using ForceTools.ViewModels;
using iTextSharp.text;
using Org.BouncyCastle.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ForceTools.WPF_Windows
{
    /// <summary>
    /// Interaction logic for InvoiceEditWindow.xaml
    /// </summary>
    public partial class InvoiceEditWindow : Window, INotifyPropertyChanged
    {
        //Sql Variables
        SqlConnection SqlConnection;
        SqlDataAdapter SqlDataAdapter;
        SqlCommand SqlCommand;
        DataTable VisualisationDt;
        DataTable FakturiUpdateDt;

        public BitmapImage BitImage { get { return _BitImage; } set { _BitImage = value; OnPropertyChanged(); } }
        private BitmapImage _BitImage;
        //Kontragent Variables
        public DataTable SearchDtEik { get { return _SearchDtEik; } set { _SearchDtEik = value; OnPropertyChanged(); } }
        private DataTable _SearchDtEik;
        public bool isEikPopupOpen { get { return _isEikPopupOpen; } set { _isEikPopupOpen = value; OnPropertyChanged(); } }
        private bool _isEikPopupOpen = false;
        public string EikSearchText { get { return _EikSearchText; } set { _EikSearchText = value; OnPropertyChanged(); } }
        private string _EikSearchText;
        //DocType Variables
        public bool isDocTypePopupOpen { get { return _isDocTypePopupOpen; } set { _isDocTypePopupOpen = value; OnPropertyChanged(); } }
        private bool _isDocTypePopupOpen = false;
        public DataTable SearchDtDocType { get { return _SearchDtDocType; } set { _SearchDtDocType = value; OnPropertyChanged(); } }
        private DataTable _SearchDtDocType;
        public string DocTypeSearchText { get { return _DocTypeSearchText; } set { _DocTypeSearchText = value; OnPropertyChanged(); } }
        private string _DocTypeSearchText;
        //DealKind Variables
        public bool isDealKindPopupOpen { get { return _isDealKindPopupOpen; } set { _isDealKindPopupOpen = value; OnPropertyChanged(); } }
        private bool _isDealKindPopupOpen = false;
        public DataTable SearchDtDealKind { get { return _SearchDtDealKind; } set { _SearchDtDealKind = value; OnPropertyChanged(); } }
        private DataTable _SearchDtDealKind;
        public string DealKindSearchText { get { return _DealKindSearchText; } set { _DealKindSearchText = value; OnPropertyChanged(); } }
        private string _DealKindSearchText;
        //Accounts Variable
        public bool isAccountsPopupOpen { get { return _isAccountsPopupOpen; } set { _isAccountsPopupOpen = value; OnPropertyChanged(); } }
        private bool _isAccountsPopupOpen = false;
        public DataTable SearchDtAccounts { get { return _SearchDtAccounts; } set { _SearchDtAccounts = value; OnPropertyChanged(); } }
        private DataTable _SearchDtAccounts = new DataTable();
        public string AccountsSearchText { get { return _AccountsSearchText; } set { _AccountsSearchText = value; OnPropertyChanged(); } }
        private string _AccountsSearchText;


        private int? DealKindId;
        private int? DocTypeId;

        private int CurrentOpenInvoice;

        private int AccStatusChosen;

        private string isPurchaseOrSale;

        public event PropertyChangedEventHandler PropertyChanged;

        public InvoiceEditWindow()
        {
            InitializeComponent();
        }

        public InvoiceEditWindow(int invoiceId, int accStatusChosen, string PurchaseOrSale) : this()
        {
            isPurchaseOrSale = PurchaseOrSale;

            using (SqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                SqlCommand = new SqlCommand($"Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, " +
                    $"Kontragenti.Name, Kontragenti.EIK, Kontragenti.DDSNumber, Fakturi.DO, " +
                    $"Fakturi.DDS, Fakturi.FullValue, Fakturi.image, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, Fakturi.DealKindId, Fakturi.DocTypeId From Fakturi " +
                    $"LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id WHERE Fakturi.Id = {invoiceId}", SqlConnection);
                SqlDataAdapter = new SqlDataAdapter(SqlCommand);
                VisualisationDt = new DataTable();
                SqlDataAdapter.Fill(VisualisationDt);

                if (VisualisationDt.Rows.Count >= 0)
                {
                    if (VisualisationDt.Rows[0].Field<string>("Name") != null)
                    {
                        KontTextBox.Text = VisualisationDt.Rows[0].Field<string>("Name").ToString();
                    }
                    else
                    {
                        KontTextBox.Text = "";
                    }
                    if (VisualisationDt.Rows[0].Field<string>("EIK") != null)
                    {
                        EikTextBox.Text = VisualisationDt.Rows[0].Field<string>("EIK").ToString();
                    }
                    else
                    {
                        EikTextBox.Text = "";
                    }
                    if (VisualisationDt.Rows[0].Field<string>("DDSNumber") != null)
                    {
                        DDSNumberTextBox.Text = VisualisationDt.Rows[0].Field<string>("DDSNumber").ToString();
                    }
                    else
                    {
                        DDSNumberTextBox.Text = "";
                    }
                    if (VisualisationDt.Rows[0].Field<DateTime?>("Date") != null)
                    {
                        DocDateTextBox.Text = VisualisationDt.Rows[0].Field<DateTime>("Date").ToString("dd,MM,yyyy", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        DocDateTextBox.Text = "";
                    }
                    if (VisualisationDt.Rows[0].Field<Int64?>("Number") != null)
                    {
                        DocNumTextBox.Text = VisualisationDt.Rows[0].Field<Int64?>("Number").ToString();
                    }
                    else
                    {
                        DocNumTextBox.Text = "";
                    }
                    if (VisualisationDt.Rows[0].Field<decimal?>("DO") != null)
                    {
                        DOTextBox.Text = VisualisationDt.Rows[0].Field<decimal?>("DO").ToString();
                    }
                    else
                    {
                        DOTextBox.Text = "";
                    }
                    if (VisualisationDt.Rows[0].Field<decimal?>("DDS") != null)
                    {
                        DDSTextBox.Text = VisualisationDt.Rows[0].Field<decimal?>("DDS").ToString();
                    }
                    else
                    {
                        DDSTextBox.Text = "";
                    }
                    if (VisualisationDt.Rows[0].Field<decimal?>("FullValue") != null)
                    {
                        FullValueTextBox.Text = VisualisationDt.Rows[0].Field<decimal>("FullValue").ToString();
                    }
                    else
                    {
                        FullValueTextBox.Text = "";
                    }
                    /////////////////////////////////////////////////////////////////////////////////////////////////
                    if (VisualisationDt.Rows[0].Field<DateTime?>("AccDate") != null)
                    {
                        DocAccDateTextBox.Text = VisualisationDt.Rows[0].Field<DateTime>("AccDate").ToString("dd,MM,yyyy", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        DocAccDateTextBox.Text = "";
                    }
                    if (VisualisationDt.Rows[0].Field<string>("DealName") != null)
                    {
                        DealKindTb.Text = VisualisationDt.Rows[0].Field<string>("DealName").ToString();
                        isPayedCash.IsChecked = true;
                        //HARD CODED / NEEDS LOGIC
                    }
                    if (VisualisationDt.Rows[0].Field<string>("TypeName") != null)
                    {
                        DocTypeTextBox.Text = VisualisationDt.Rows[0].Field<string>("TypeName").ToString();
                    }
                    else
                    {
                        DocTypeTextBox.Text = "";
                    }
                    if (VisualisationDt.Rows[0].Field<int?>("Account") != null)
                    {
                        AccNumTextBox.Text = VisualisationDt.Rows[0].Field<int>("Account").ToString();
                    }
                    else
                    {
                        AccNumTextBox.Text = "";
                    }
                    if (VisualisationDt.Rows[0].Field<int?>("InCashAccount") != null)
                    {
                        InCashAccountTextBox.Text = VisualisationDt.Rows[0].Field<int>("InCashAccount").ToString();
                    }
                    else
                    {
                        InCashAccountTextBox.Text = "";
                    }
                    if (VisualisationDt.Rows[0].Field<string>("Note") != null)
                    {
                        NoteTextBox.Text = VisualisationDt.Rows[0].Field<string>("Note").ToString();
                    }
                    else
                    {
                        NoteTextBox.Text = "";
                    }
                    if (VisualisationDt.Rows[0].Field<int?>("DealKindId") != null)
                    {
                        DealKindId = VisualisationDt.Rows[0].Field<int?>("DealKindId");
                    }
                    else
                    {
                        DealKindId = null;
                    }
                    if (VisualisationDt.Rows[0].Field<int?>("DocTypeId") != null)
                    {
                        DocTypeId = VisualisationDt.Rows[0].Field<int?>("DocTypeId");
                    }
                    else
                    {
                        DocTypeId = null;
                    }
                    ////////////////////////////////////////////////////////////////////////////////////////////////
                    if (VisualisationDt.Rows[0].Field<Byte[]>("image") != null)
                    {
                        using (var ms = new System.IO.MemoryStream(VisualisationDt.Rows[0].Field<Byte[]>("image")))
                        {
                            var image = new BitmapImage();
                            image.BeginInit();
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.StreamSource = ms;
                            image.EndInit();

                            BitImage = image;
                        }
                    }
                    CurrentOpenInvoice = invoiceId;
                    isEikPopupOpen = false;
                    isDocTypePopupOpen = false;
                    isDealKindPopupOpen = false;
                }
            }
            AccStatusChosen = accStatusChosen;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateCurrent();
            LoadNextOrPrevious(1);
        }

        private void RightBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadNextOrPrevious(1);
        }

        private void LeftBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadNextOrPrevious(2);
        }

        private void UpdateCurrent()
        {
            using (SqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                SqlConnection.Open();
                //Update Current - Kontragent Part
                int? KontIdFromSearchTable = null;
                if (EikTextBox.Text != "")
                {

                    SqlCommand = new SqlCommand("Select Kontragenti.Id from Kontragenti where EIK like '%" + EikTextBox.Text + "%'", SqlConnection);
                    SqlDataAdapter = new SqlDataAdapter(SqlCommand);
                    DataTable KontByIdTb = new DataTable("KontById");
                    SqlDataAdapter.Fill(KontByIdTb);
                    if (KontByIdTb.Rows.Count == 0)
                    {
                        SqlCommand = new SqlCommand("INSERT into Kontragenti (Name, EIK, DDSNumber) VALUES (@KontragentName, @EIK, @DDSNumber)", SqlConnection);

                        SqlCommand.Parameters.AddWithValue("@KontragentName", KontTextBox.Text);
                        SqlCommand.Parameters.AddWithValue("@EIK", EikTextBox.Text);
                        SqlCommand.Parameters.AddWithValue("@DDSNumber", DDSNumberTextBox.Text);
                        SqlCommand.ExecuteNonQuery();
                        KontByIdTb.Clear();
                        SqlDataAdapter.Fill(KontByIdTb);

                    }

                    if (KontByIdTb.Rows.Count > 0 && KontByIdTb.Rows.Count < 2)
                    {
                        KontIdFromSearchTable = KontByIdTb.Rows[0].Field<int>("Id");
                    }
                }
                else
                {
                    MessageBox.Show("Нещо се обърка");
                }

                //Update Current - Document Part
                SqlCommand = new SqlCommand($"select * from Fakturi where Fakturi.Id = {CurrentOpenInvoice} ", SqlConnection);
                SqlDataAdapter = new SqlDataAdapter(SqlCommand);
                FakturiUpdateDt = new DataTable("Fakturi");
                SqlDataAdapter.Fill(FakturiUpdateDt);

                 /* 0 - Id
                    1 - DocPayableReceivableId
                    2 - KontragentiId
                    3 - AccDate
                    4 - Date
                    5 - Number
                    6 - DO
                    7 - DDS
                    8 - FullValue
                    9 - DealKindId
                    10 - DocTypeId
                    11 - Account
                    12 - InCashAccount
                    13 - Note
                    14 - Image
                    15 - AccountingStatusId */

                FakturiUpdateDt.Rows[0][2] = KontIdFromSearchTable; 
                FakturiUpdateDt.Rows[0][4] = Convert.ToDateTime(DocDateTextBox.Text); 
                FakturiUpdateDt.Rows[0][5] = Convert.ToInt64(DocNumTextBox.Text); 
                FakturiUpdateDt.Rows[0][6] = Convert.ToDecimal(DOTextBox.Text.Replace(".",",")); 
                FakturiUpdateDt.Rows[0][7] = Convert.ToDecimal(DDSTextBox.Text.Replace(".", ",")); 
                FakturiUpdateDt.Rows[0][8] = Convert.ToDecimal(FullValueTextBox.Text.Replace(".", ",")); 
                FakturiUpdateDt.Rows[0][15] = 3;
                FakturiUpdateDt.Rows[0][3] = Convert.ToDateTime(DocDateTextBox.Text);
                ///////////////////////////////////////////////////////////////////////////////// HARD CODED / NEEDS LOGIC 
                FakturiUpdateDt.Rows[0][9] = DealKindId;
                FakturiUpdateDt.Rows[0][10] = DocTypeId;
                /////////////////////////////////////////////////////////////////////////////////
                FakturiUpdateDt.Rows[0][11] = Convert.ToInt32(AccNumTextBox.Text);
                FakturiUpdateDt.Rows[0][12] = Convert.ToInt32(InCashAccountTextBox.Text);
                FakturiUpdateDt.Rows[0][13] = NoteTextBox.Text; 
                

                SqlCommandBuilder builder = new SqlCommandBuilder(SqlDataAdapter);
                SqlDataAdapter.UpdateCommand = builder.GetUpdateCommand();
                SqlDataAdapter.Update(FakturiUpdateDt);
            }
        }
        private void LoadNextOrPrevious(int NextOrPrevious)
        {
            // For Next Invoice NextOrPrevious = 1 / For Previous Invoice NextOrPrevious = 2 
            using (SqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {

                //Load Next
                VisualisationDt.Clear();
                int nextInvoice = 0;
                switch (NextOrPrevious)
                {
                    case 1:
                        nextInvoice = CurrentOpenInvoice + 1;
                        break;
                    case 2:
                        nextInvoice = CurrentOpenInvoice - 1;
                        break;
                }
                int StopCheck = 0;

                switch (isPurchaseOrSale) 
                {
                    case "Purchase":
                        switch (AccStatusChosen)
                        {
                            case 0:
                                SqlCommand = new SqlCommand($"select Fakturi.Id from Fakturi where Fakturi.Id = {nextInvoice} and PurchaseOrSale = 'Purchase'", SqlConnection);
                                break;
                            case 1:
                                SqlCommand = new SqlCommand($"select Fakturi.Id from Fakturi where Fakturi.Id = {nextInvoice} and (AccountingStatusId = 1 and PurchaseOrSale = 'Purchase')", SqlConnection);
                                break;
                            case 2:
                                SqlCommand = new SqlCommand($"select Fakturi.Id from Fakturi where Fakturi.Id = {nextInvoice} and (AccountingStatusId = 2 and PurchaseOrSale = 'Purchase') ", SqlConnection);
                                break;
                            case 3:
                                SqlCommand = new SqlCommand($"select Fakturi.Id from Fakturi where Fakturi.Id = {nextInvoice} and (AccountingStatusId = 3 and PurchaseOrSale = 'Purchase') ", SqlConnection);
                                break;
                            case 4:
                                SqlCommand = new SqlCommand($"select Fakturi.Id from Fakturi where Fakturi.Id = {nextInvoice} and (AccountingStatusId = 4 and PurchaseOrSale = 'Purchase') ", SqlConnection);
                                break;

                        }
                        break;
                    case "Sale":
                        switch (AccStatusChosen)
                        {
                            case 0:
                                SqlCommand = new SqlCommand($"select Fakturi.Id from Fakturi where Fakturi.Id = {nextInvoice} and PurchaseOrSale = 'Sale' ", SqlConnection);
                                break;
                            case 1:
                                SqlCommand = new SqlCommand($"select Fakturi.Id from Fakturi where Fakturi.Id = {nextInvoice} and (AccountingStatusId = 1 and PurchaseOrSale = 'Sale') ", SqlConnection);
                                break;
                            case 2:
                                SqlCommand = new SqlCommand($"select Fakturi.Id from Fakturi where Fakturi.Id = {nextInvoice} and (AccountingStatusId = 2 and PurchaseOrSale = 'Sale') ", SqlConnection);
                                break;
                            case 3:
                                SqlCommand = new SqlCommand($"select Fakturi.Id from Fakturi where Fakturi.Id = {nextInvoice} and (AccountingStatusId = 3 and PurchaseOrSale = 'Sale') ", SqlConnection);
                                break;
                            case 4:
                                SqlCommand = new SqlCommand($"select Fakturi.Id from Fakturi where Fakturi.Id = {nextInvoice} and (AccountingStatusId = 4 and PurchaseOrSale = 'Sale') ", SqlConnection);
                                break;

                        }
                        break;
                }

                

                SqlDataAdapter = new SqlDataAdapter(SqlCommand);
                VisualisationDt = new DataTable();
                SqlDataAdapter.Fill(VisualisationDt);
                if (VisualisationDt.Rows.Count < 1)
                {
                    do
                    {

                        switch (NextOrPrevious)
                        {
                            case 1:
                                nextInvoice++;
                                break;
                            case 2:
                                nextInvoice--;
                                break;
                        }

                        StopCheck++;

                        switch (isPurchaseOrSale)
                        {
                            case "Purchase":
                                switch (AccStatusChosen)
                                {
                                    case 0:
                                        SqlCommand = new SqlCommand($"select Fakturi.Id from Fakturi where Fakturi.Id = {nextInvoice} and PurchaseOrSale = 'Purchase'", SqlConnection);
                                        break;
                                    case 1:
                                        SqlCommand = new SqlCommand($"select Fakturi.Id from Fakturi where Fakturi.Id = {nextInvoice} and (AccountingStatusId = 1 and PurchaseOrSale = 'Purchase')", SqlConnection);
                                        break;
                                    case 2:
                                        SqlCommand = new SqlCommand($"select Fakturi.Id from Fakturi where Fakturi.Id = {nextInvoice} and (AccountingStatusId = 2 and PurchaseOrSale = 'Purchase') ", SqlConnection);
                                        break;
                                    case 3:
                                        SqlCommand = new SqlCommand($"select Fakturi.Id from Fakturi where Fakturi.Id = {nextInvoice} and (AccountingStatusId = 3 and PurchaseOrSale = 'Purchase') ", SqlConnection);
                                        break;
                                    case 4:
                                        SqlCommand = new SqlCommand($"select Fakturi.Id from Fakturi where Fakturi.Id = {nextInvoice} and (AccountingStatusId = 4 and PurchaseOrSale = 'Purchase') ", SqlConnection);
                                        break;

                                }
                                break;
                            case "Sale":
                                switch (AccStatusChosen)
                                {
                                    case 0:
                                        SqlCommand = new SqlCommand($"select Fakturi.Id from Fakturi where Fakturi.Id = {nextInvoice} and PurchaseOrSale = 'Sale' ", SqlConnection);
                                        break;
                                    case 1:
                                        SqlCommand = new SqlCommand($"select Fakturi.Id from Fakturi where Fakturi.Id = {nextInvoice} and (AccountingStatusId = 1 and PurchaseOrSale = 'Sale') ", SqlConnection);
                                        break;
                                    case 2:
                                        SqlCommand = new SqlCommand($"select Fakturi.Id from Fakturi where Fakturi.Id = {nextInvoice} and (AccountingStatusId = 2 and PurchaseOrSale = 'Sale') ", SqlConnection);
                                        break;
                                    case 3:
                                        SqlCommand = new SqlCommand($"select Fakturi.Id from Fakturi where Fakturi.Id = {nextInvoice} and (AccountingStatusId = 3 and PurchaseOrSale = 'Sale') ", SqlConnection);
                                        break;
                                    case 4:
                                        SqlCommand = new SqlCommand($"select Fakturi.Id from Fakturi where Fakturi.Id = {nextInvoice} and (AccountingStatusId = 4 and PurchaseOrSale = 'Sale') ", SqlConnection);
                                        break;

                                }
                                break;
                        }

                        

                        SqlDataAdapter = new SqlDataAdapter(SqlCommand);
                        VisualisationDt = new DataTable();
                        SqlDataAdapter.Fill(VisualisationDt);

                    } while (VisualisationDt.Rows.Count < 1 && StopCheck <= 999);
                }

                if (StopCheck == 1000)
                {
                    StopCheck = 0;
                    switch (NextOrPrevious)
                    {
                        case 1:
                            MessageBox.Show("Няма други следващи фактури", "Force Tools", MessageBoxButton.OK, MessageBoxImage.Information);
                            break;
                        case 2:
                            MessageBox.Show("Няма други предишни фактури", "Force Tools", MessageBoxButton.OK, MessageBoxImage.Information);
                            break;
                    }
                    return;
                }
                else
                {
                    VisualisationDt.Clear();
                    VisualisationDt.Rows.Clear();
                    SqlCommand = new SqlCommand($"Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, " +
                    $"Kontragenti.Name, Kontragenti.EIK, Kontragenti.DDSNumber, Fakturi.DO, " +
                    $"Fakturi.DDS, Fakturi.FullValue, Fakturi.image, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, Fakturi.DealKindId, Fakturi.DocTypeId From Fakturi " +
                    $"LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id WHERE Fakturi.Id = {nextInvoice}", SqlConnection);
                    SqlDataAdapter = new SqlDataAdapter(SqlCommand);
                    VisualisationDt = new DataTable();
                    SqlDataAdapter.Fill(VisualisationDt);

                    if (VisualisationDt.Rows[0].Field<string>("Name") != null)
                    {
                        KontTextBox.Text = VisualisationDt.Rows[0].Field<string>("Name").ToString();
                    }
                    else 
                    {
                        KontTextBox.Text = "";
                    }
                    if (VisualisationDt.Rows[0].Field<string>("EIK") != null)
                    {
                        EikTextBox.Text = VisualisationDt.Rows[0].Field<string>("EIK").ToString();
                    }
                    else
                    {
                        EikTextBox.Text = "";
                    }
                    if (VisualisationDt.Rows[0].Field<string>("DDSNumber") != null)
                    {
                        DDSNumberTextBox.Text = VisualisationDt.Rows[0].Field<string>("DDSNumber").ToString();
                    }
                    else
                    {
                        DDSNumberTextBox.Text = "";
                    }
                    if (VisualisationDt.Rows[0].Field<DateTime?>("Date") != null)
                    {
                        DocDateTextBox.Text = VisualisationDt.Rows[0].Field<DateTime>("Date").ToString("dd,MM,yyyy", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        DocDateTextBox.Text = "";
                    }
                    if (VisualisationDt.Rows[0].Field<Int64?>("Number") != null)
                    {
                        DocNumTextBox.Text = VisualisationDt.Rows[0].Field<Int64?>("Number").ToString();
                    }
                    else
                    {
                        DocNumTextBox.Text = "";
                    }   
                    if (VisualisationDt.Rows[0].Field<decimal?>("DO") != null)
                    {
                        DOTextBox.Text = VisualisationDt.Rows[0].Field<decimal?>("DO").ToString();
                    }
                    else
                    {
                        DOTextBox.Text = "";
                    }
                    if (VisualisationDt.Rows[0].Field<decimal?>("DDS") != null)
                    {
                        DDSTextBox.Text = VisualisationDt.Rows[0].Field<decimal?>("DDS").ToString();
                    }
                    else
                    {
                        DDSTextBox.Text = "";
                    }
                    if (VisualisationDt.Rows[0].Field<decimal?>("FullValue") != null)
                    {
                        FullValueTextBox.Text = VisualisationDt.Rows[0].Field<decimal>("FullValue").ToString();
                    }
                    else
                    {
                        FullValueTextBox.Text = "";
                    }
                    /////////////////////////////////////////////////////////////////////////////////////////////////
                    if (VisualisationDt.Rows[0].Field<DateTime?>("AccDate") != null)
                    {
                        DocAccDateTextBox.Text = VisualisationDt.Rows[0].Field<DateTime>("AccDate").ToString("dd,MM,yyyy", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        DocAccDateTextBox.Text = "";
                    }
                    if (VisualisationDt.Rows[0].Field<string>("DealName") != null)
                    {
                        DealKindTb.Text = VisualisationDt.Rows[0].Field<string>("DealName").ToString();
                        isPayedCash.IsChecked = true;
                        //HARD CODED / NEEDS LOGIC
                    }
                    if (VisualisationDt.Rows[0].Field<string>("TypeName") != null)
                    {
                        DocTypeTextBox.Text = VisualisationDt.Rows[0].Field<string>("TypeName").ToString();
                    }
                    else
                    {
                        DocTypeTextBox.Text = "";
                    }
                    if (VisualisationDt.Rows[0].Field<int?>("Account") != null)
                    {
                        AccNumTextBox.Text = VisualisationDt.Rows[0].Field<int>("Account").ToString();
                    }
                    else
                    {
                        AccNumTextBox.Text = "";
                    }
                    if (VisualisationDt.Rows[0].Field<int?>("InCashAccount") != null)
                    {
                        InCashAccountTextBox.Text = VisualisationDt.Rows[0].Field<int>("InCashAccount").ToString();
                    }
                    else
                    {
                        InCashAccountTextBox.Text = "";
                    }
                    if (VisualisationDt.Rows[0].Field<string>("Note") != null)
                    {
                        NoteTextBox.Text = VisualisationDt.Rows[0].Field<string>("Note").ToString();
                    }
                    else
                    {
                        NoteTextBox.Text = "";
                    }
                    if (VisualisationDt.Rows[0].Field<int?>("DealKindId") != null)
                    {
                        DealKindId = VisualisationDt.Rows[0].Field<int?>("DealKindId");
                    }
                    else
                    {
                        DealKindId = null;
                    }
                    if (VisualisationDt.Rows[0].Field<int?>("DocTypeId") != null)
                    {
                        DocTypeId = VisualisationDt.Rows[0].Field<int?>("DocTypeId");
                    }
                    else
                    {
                        DocTypeId = null;
                    }
                    ////////////////////////////////////////////////////////////////////////////////////////////////


                    if (VisualisationDt.Rows[0].Field<Byte[]>("image") != null)
                    {
                        using (var ms = new System.IO.MemoryStream(VisualisationDt.Rows[0].Field<Byte[]>("image")))
                        {
                            var image = new BitmapImage();
                            image.BeginInit();
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.StreamSource = ms;
                            image.EndInit();

                            BitImage = image;
                        }
                    }
                    CurrentOpenInvoice = nextInvoice;
                    StopCheck = 0;
                    isEikPopupOpen = false;
                    isDocTypePopupOpen = false;
                    isDealKindPopupOpen = false;
                }
            }
        }
        //Helper methods
        #region Helpers
        public static void SelectRowByIndex(DataGrid dataGrid, int rowIndex)
        {
            try
            {
                if (!dataGrid.SelectionUnit.Equals(DataGridSelectionUnit.FullRow))
                    throw new ArgumentException("The SelectionUnit of the DataGrid must be set to FullRow.");

                if (rowIndex < 0 || rowIndex > (dataGrid.Items.Count - 1))
                    throw new ArgumentException(string.Format("{0} is an invalid row index.", rowIndex));

                /* set the SelectedItem property */
                object item = dataGrid.Items[rowIndex]; // = Product X
                dataGrid.SelectedItem = item;

                DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
                if (row == null)
                {
                    /* bring the data item (Product object) into view
                     * in case it has been virtualized away */
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
                    /* if the row has been virtualized away, call its ApplyTemplate() method
                     * to build its visual tree in order for the DataGridCellsPresenter
                     * and the DataGridCells to be created */
                    rowContainer.ApplyTemplate();
                    presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                }
                if (presenter != null)
                {
                    DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    if (cell == null)
                    {
                        /* bring the column into view
                         * in case it has been virtualized away */
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
        #endregion
        //Kontragent Events
        private void EikTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            isEikPopupOpen = true;
            using (SqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                EikSearchText = EikTextBox.Text;
                SqlCommand = new SqlCommand($"SELECT * FROM Kontragenti WHERE EIK like '%{EikSearchText}%'", SqlConnection);
                SqlDataAdapter = new SqlDataAdapter(SqlCommand);
                SearchDtEik = new DataTable("Search");
                SearchDtEik.Clear();
                SqlDataAdapter.Fill(SearchDtEik);

                if (SearchDtEik.Rows.Count == 0)
                {
                    isEikPopupOpen = false;
                }

                SelectRowByIndex(PopUpDgEik, 0);
            }
        }
        private void PopUpDgEik_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Down && e.Key != Key.Up && e.Key != Key.Enter)
            {
                EikTextBox.Focus();
            }
            if (e.Key == Key.Enter)
            {
                DataGrid dg = (DataGrid)sender;
                DataRowView dataRow = (DataRowView)dg.SelectedItem;
                if (dataRow != null)
                {
                    string KontName = dataRow.Row.ItemArray[1].ToString();
                    string KontEIK = dataRow.Row.ItemArray[2].ToString();
                    string KontDDSNumber = dataRow.Row.ItemArray[3].ToString();

                    KontTextBox.Text = KontName;
                    EikTextBox.Text = KontEIK;
                    DDSNumberTextBox.Text = KontDDSNumber;

                    isEikPopupOpen = false;
                    EikTextBox.Focus();
                }
            }
        }
        private void DDSNumberTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            isEikPopupOpen = false;
        }
        //DocType Events
        private void DocTypeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            isDocTypePopupOpen = true;
            using (SqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                DocTypeSearchText = DocTypeTextBox.Text;
                SqlCommand = new SqlCommand($"SELECT * FROM DocumentTypes WHERE TypeName like N'%{DocTypeSearchText}%'", SqlConnection);
                SqlDataAdapter = new SqlDataAdapter(SqlCommand);
                SearchDtDocType = new DataTable("SearchDocType");
                SearchDtDocType.Clear();
                SqlDataAdapter.Fill(SearchDtDocType);

                if (SearchDtDocType.Rows.Count == 0)
                {
                    isDocTypePopupOpen = false;
                }

                SelectRowByIndex(PopUpDgDocType, 0);
            }
        }
        private void PopUpDgDocType_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Down && e.Key != Key.Up && e.Key != Key.Enter)
            {
                DocTypeTextBox.Focus();
            }
            if (e.Key == Key.Enter)
            {
                DataGrid dg = (DataGrid)sender;
                DataRowView dataRow = (DataRowView)dg.SelectedItem;
                if (dataRow != null)
                {
                    int? DocId = Convert.ToInt32(dataRow.Row.ItemArray[0]);
                    string DocTypeName = dataRow.Row.ItemArray[1].ToString();

                    DocTypeTextBox.Text = DocTypeName;
                    DocTypeId = DocId;

                    isDocTypePopupOpen = false;
                    DocTypeTextBox.Focus();
                }
            }
        }
        //DealKind Events
        private void PopUpDgDealKind_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Down && e.Key != Key.Up && e.Key != Key.Enter)
            {
                DealKindTb.Focus();
            }
            if (e.Key == Key.Enter)
            {
                DataGrid dg = (DataGrid)sender;
                DataRowView dataRow = (DataRowView)dg.SelectedItem;
                if (dataRow != null)
                {
                    int? DealId = Convert.ToInt32(dataRow.Row.ItemArray[0]);
                    string DealName = dataRow.Row.ItemArray[2].ToString();

                    DealKindTb.Text = DealName;
                    DealKindId = DealId;

                    isDealKindPopupOpen = false;
                    DealKindTb.Focus();
                }
            }
        }
        private void DealKindTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            isDealKindPopupOpen = true;
            using (SqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                DealKindSearchText = DealKindTb.Text;
                SqlCommand = new SqlCommand($"SELECT * FROM KindOfDeals WHERE DealName like N'%{DealKindSearchText}%' or Percentage like N'%{DealKindSearchText}%'", SqlConnection);
                SqlDataAdapter = new SqlDataAdapter(SqlCommand);
                SearchDtDealKind = new DataTable("SearchDealKind");
                SearchDtDealKind.Clear();
                SqlDataAdapter.Fill(SearchDtDealKind);

                if (SearchDtDealKind.Rows.Count == 0)
                {
                    isDealKindPopupOpen = false;
                }

                SelectRowByIndex(PopUpDgDealKind, 0);
            }
        }
        //Account Events - abit different than the others (creating the new datatable in the private variable instead of in the function)
        private void AccNumTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            isAccountsPopupOpen = true;
            using (SqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                AccountsSearchText = AccNumTextBox.Text;
                SqlCommand = new SqlCommand($"SELECT * FROM Accounts WHERE Account like N'%{AccountsSearchText}%' or AccountName like N'%{AccountsSearchText}%'", SqlConnection);
                SqlDataAdapter = new SqlDataAdapter(SqlCommand);
                SearchDtAccounts = new DataTable("SearchAccountsDt");
                SearchDtAccounts.Clear();
                SqlDataAdapter.Fill(SearchDtAccounts);

                if (SearchDtAccounts.Rows.Count == 0)
                {
                    isAccountsPopupOpen = false;
                }

                SelectRowByIndex(PopUpDgAccNum, 0);
            }
        }
        private void PopUpDgAccNum_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Down && e.Key != Key.Up && e.Key != Key.Enter)
            {
                AccNumTextBox.Focus();
            }
            if (e.Key == Key.Enter)
            {
                DataGrid dg = (DataGrid)sender;
                DataRowView dataRow = (DataRowView)dg.SelectedItem;
                if (dataRow != null)
                {
                    AccNumTextBox.Text = dataRow.Row.ItemArray[0].ToString();

                    isAccountsPopupOpen = false;
                    AccNumTextBox.Focus();
                }
            }
        }
        //Payed in cash Events
        private void isPayedCash_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (isPayedCash.IsChecked == false)
                {
                    isPayedCash.IsChecked = true;
                }
                else
                {
                    isPayedCash.IsChecked = false;
                }
            }
        }
        //Prop Changed Event
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }
}
