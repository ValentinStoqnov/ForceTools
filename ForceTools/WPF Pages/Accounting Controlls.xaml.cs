using System.Windows;
using System.Windows.Controls;


namespace ForceTools
{
    public partial class Accounting_Controlls : Page
    {
        public int NewCount { get; set; }
        public int UnAcCount { get; set; }
        public int RdyExpCount { get; set; }
        public int ExportedCount { get; set; }
        public int AllCount { get; set; }

        private OperationType operationType;

        public Accounting_Controlls()
        {
            InitializeComponent();
        }
        public Accounting_Controlls(OperationType operationType) : this()
        {
            this.operationType = operationType;
            GetInvoicesCount();
            SetPurchaseOrSaleLabel();
        }

        private void SetPurchaseOrSaleLabel()
        {
            switch (operationType)
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
            NewCount = InvoiceCounter.GetNewInvoicesCount(operationType);
            UnAcCount = InvoiceCounter.GetUnAccountedInvoicesCount(operationType);
            RdyExpCount = InvoiceCounter.GetReadyToBeExportedInvoicesCount(operationType);
            ExportedCount = InvoiceCounter.GetExportedInvoicesCount(operationType);
            AllCount = InvoiceCounter.GetAllInvoicesCount(operationType);
        }

        private void AllInvoicesButton_Click(object sender, RoutedEventArgs e)
        {
            UiNavigationHelper.MainWindow.ContentFrame.Content = new InvoiceGridPage(DocumentStatuses.AccountedDocuments, operationType);
        }
        private void HeldInvoicesButton_Click(object sender, RoutedEventArgs e)
        {
            UiNavigationHelper.MainWindow.ContentFrame.Content = new InvoiceGridPage(DocumentStatuses.HeldDocuments, operationType);
        }
        private void UnAccountedInvoicesBtn_Click(object sender, RoutedEventArgs e)
        {
            UiNavigationHelper.MainWindow.ContentFrame.Content = new InvoiceGridPage(DocumentStatuses.UnAccountedDocuments, operationType);
        }
        private void RdyToBeExportedBtn_Click(object sender, RoutedEventArgs e)
        {
            UiNavigationHelper.MainWindow.ContentFrame.Content = new InvoiceGridPage(DocumentStatuses.ReadyToBeExportedDocuments, operationType);
        }
        private void ExportedInvoicesBtn_Click(object sender, RoutedEventArgs e)
        {
            UiNavigationHelper.MainWindow.ContentFrame.Content = new InvoiceGridPage(DocumentStatuses.ExportedDocuments, operationType);
        }
        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            ExcelExporter.ExtractFromDataTable(InvoiceExtractionDataRetriever.GetDataTableForExcelFullExport(operationType));
        }
    }
}

