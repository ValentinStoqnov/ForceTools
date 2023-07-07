using ForceTools.WPF_Windows;
using System.Linq;
using System.Windows;

namespace ForceTools
{
    public static class UiNavigationHelper
    {
        public static MainWindow MainWindow { get { return GetCurrentMainWindow(); } }
        public static MainWindow GetCurrentMainWindow()
        {
            return Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive) as MainWindow;
        }
        public static void OpenMainWindow(string account) 
        {
            MainWindow mw = new MainWindow(account, SqlConnectionsHandler.CheckUserPermissions(account));
            mw.Show();
        }
        public static void OpenLoginWindow() 
        {
            LoginWindow LW = new LoginWindow();
            LW.Show();
        }
        public static void OpenNICWWindow(OperationType operationType) 
        {
            NICW InvCreWin = new NICW(operationType);
            InvCreWin.ShowDialog();
        }
        public static void OpenPdfUploaderWindow(OperationType operationType) 
        {
            PdfUploaderWindow PUW = new PdfUploaderWindow(operationType);
            PUW.ShowDialog();
        }
        public static void OpenInvoiceEditWindow(int InvoiceId, OperationType operationType, DocumentStatuses documentStatuses) 
        {
            InvoiceEditWindow IEW = new InvoiceEditWindow(InvoiceId, documentStatuses, operationType);
            IEW.ShowDialog();
        }
    }
}
