using ForceTools.WPF_Pages;
using ForceTools.WPF_Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace ForceTools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private int _MenuClumnWidth = 150;
        public int MenuColumnWidth { get { return _MenuClumnWidth; } set { _MenuClumnWidth = value; OnPropertyChanged(); } }
        private string _FirmNameTxt;
        public string FirmNameTxt { get { return _FirmNameTxt; } set { _FirmNameTxt = value; OnPropertyChanged(); } }

        private bool IsUserAdmin = false;

        private BrushConverter bc = new BrushConverter();

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            SetInitialPageAndValues();   
        }

        public MainWindow(string CurrentUser, string Access) : this()
        {
            CurrentUserLbl.Content = CurrentUser;

            if (Access == "admin")
            {
                IsUserAdmin = true;
            }
            else 
            {
                IsUserAdmin = false;
            }
        }

        private void SetInitialPageAndValues() 
        {
            Firms.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#FFFF6900");
            FirmPackets Fp = new FirmPackets();
            ContentFrame.Content = Fp.Content;
            FirmNameTxt = "Изберете пакет";
            PurchasesButton.IsEnabled = false;
            SalesButton.IsEnabled = false;
            Settings.IsEnabled = false;
            KontragentiButton.IsEnabled = false;
        }
        
        private void PurchasesButton_Click(object sender, RoutedEventArgs e)
        {
            Accounting_Controlls ac = new Accounting_Controlls("Purchase");
            ContentFrame.Content = ac.Content;

            PurchasesButton.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#FFFF6900");
            SalesButton.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#0078D4");
            Firms.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#0078D4");
            Settings.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#0078D4");
            KontragentiButton.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#0078D4");

        }
        private void SalesButton_Click(object sender, RoutedEventArgs e)
        {
            Accounting_Controlls ac = new Accounting_Controlls("Sale");
            ContentFrame.Content = ac.Content;

            PurchasesButton.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#0078D4");
            SalesButton.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#FFFF6900");
            Firms.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#0078D4");
            Settings.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#0078D4");
            KontragentiButton.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#0078D4");
        }
        private void Firms_Click(object sender, RoutedEventArgs e)
        {
            FirmPackets Fp = new FirmPackets(FirmNameTxt);
            ContentFrame.Content = Fp.Content;

            PurchasesButton.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#0078D4");
            SalesButton.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#0078D4");
            Firms.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#FFFF6900");
            Settings.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#0078D4");
            KontragentiButton.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#0078D4");
        }
        private void KontragentiButton_Click(object sender, RoutedEventArgs e)
        {
            KontragentiPage Kp = new KontragentiPage();
            ContentFrame.Content = Kp.Content;

            PurchasesButton.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#0078D4");
            SalesButton.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#0078D4");
            Firms.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#0078D4");
            Settings.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#0078D4");
            KontragentiButton.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#FFFF6900");
        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsPage Sp = new SettingsPage(IsUserAdmin);
            ContentFrame.Content = Sp.Content;
            
            

            PurchasesButton.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#0078D4");
            SalesButton.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#0078D4");
            Firms.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#0078D4");
            Settings.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#FFFF6900");
            KontragentiButton.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#0078D4");
        }
        private void MenuExpander_Click(object sender, RoutedEventArgs e)
        {
            if (MenuColumnWidth == 150)
            {
                MenuColumnWidth = 57;
            }
            else if (MenuColumnWidth == 57)
            {
                MenuColumnWidth = 150;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UsersBtn_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow LW = new LoginWindow();
            LW.Show();
            this.Close();
        }
    }
}
