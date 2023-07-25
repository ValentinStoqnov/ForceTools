using System.Windows;
using System.Windows.Controls;


namespace ForceTools
{
    /// <summary>
    /// Interaction logic for Accounting_Controlls.xaml
    /// </summary>
    public partial class Accounting_Controlls : Page
    {
        public int NewCount { get; set; }
        public int UnAcCount { get; set; }
        public int RdyExpCount { get; set; }
        public int ExportedCount { get; set; }
        public int AllCount { get; set; }

        private OperationType OperationType;

        public Accounting_Controlls()
        {
            InitializeComponent();
        }
        public Accounting_Controlls(OperationType operationType) : this()
        {
            OperationType = operationType;
            GetInvoicesCount();
            SetPurchaseOrSaleLabel();
        }

        private void SetPurchaseOrSaleLabel()
        {
            switch (OperationType)
            {
                case OperationType.Purchase:
                    labelPurchasesSales.Content = "Покупки";
                    break;
                case OperationType.Sale:
                    labelPurchasesSales.Content = "Продажби";
                    break;
            }
        }
        private void GetInvoicesCount()
        {
            NewCount = InvoiceCounter.GetNewInvoicesCount(OperationType);
            UnAcCount = InvoiceCounter.GetUnAccountedInvoicesCount(OperationType);
            RdyExpCount = InvoiceCounter.GetReadyToBeExportedInvoicesCount(OperationType);
            ExportedCount = InvoiceCounter.GetExportedInvoicesCount(OperationType);
            AllCount = InvoiceCounter.GetAllInvoicesCount(OperationType);
        }

        private void AllInvoicesButton_Click(object sender, RoutedEventArgs e)
        {
            UiNavigationHelper.MainWindow.ContentFrame.Content = new InvoiceGridPage(DocumentStatuses.AccountedDocuments, OperationType);
        }
        private void HeldInvoicesButton_Click(object sender, RoutedEventArgs e)
        {
            UiNavigationHelper.MainWindow.ContentFrame.Content = new InvoiceGridPage(DocumentStatuses.HeldDocuments, OperationType);
        }
        private void UnAccountedInvoicesBtn_Click(object sender, RoutedEventArgs e)
        {
            UiNavigationHelper.MainWindow.ContentFrame.Content = new InvoiceGridPage(DocumentStatuses.UnAccountedDocuments, OperationType);
        }
        private void RdyToBeExportedBtn_Click(object sender, RoutedEventArgs e)
        {
            UiNavigationHelper.MainWindow.ContentFrame.Content = new InvoiceGridPage(DocumentStatuses.ReadyToBeExportedDocuments, OperationType);
        }
        private void ExportedInvoicesBtn_Click(object sender, RoutedEventArgs e)
        {
            UiNavigationHelper.MainWindow.ContentFrame.Content = new InvoiceGridPage(DocumentStatuses.ExportedDocuments, OperationType);
        }
        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            ExcelExporter.ExtractFromDataTable(InvoiceExtractionDataRetriever.GetDataTableForExcelFullExport());
        }
    }
}

