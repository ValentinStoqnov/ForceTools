using ForceTools.WPF_Pages;
using ForceTools.WPF_Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public int MenuColumnWidth { get { return _MenuClumnWidth; } set { _MenuClumnWidth = value; OnPropertyChanged(); } }
        public string FirmNameTxt { get { return _FirmNameTxt; } set { _FirmNameTxt = value; OnPropertyChanged(); } }
        private int _MenuClumnWidth = 150;
        private string _FirmNameTxt;

        private UserPermissions UserPermissions;
        private List<Button> buttonsList;
        private BrushConverter brushConverter = new BrushConverter();

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            SetInitialPageAndValues();
        }

        public MainWindow(string CurrentUser, UserPermissions userPermissions) : this()
        {
            CurrentUserLbl.Content = CurrentUser;
            UserPermissions = userPermissions;

            buttonsList = new List<Button>()
            {
                PurchasesButton,
                SalesButton,
                KontragentiButton,
                Firms,
                Settings
            }; 
        }

        private void SetInitialPageAndValues() 
        {
            Firms.Background = (Brush)brushConverter.ConvertFrom("#FFFF6900");
            ContentFrame.Content = new FirmPackets().Content;
            FirmNameTxt = "Изберете пакет";
            PurchasesButton.IsEnabled = false;
            SalesButton.IsEnabled = false;
            Settings.IsEnabled = false;
            KontragentiButton.IsEnabled = false;
        }
        private void SetButtonBackgroundColorWhenClicked(Button clickedButton) 
        {
            foreach (Button button in buttonsList) 
            {
                if (button == clickedButton)
                {
                    button.Background = (Brush)brushConverter.ConvertFrom("#FFFF6900");
                }
                else 
                {
                    button.Background = (Brush)brushConverter.ConvertFrom("#0078D4");
                }
            }
        }

        private void PurchasesButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Content = new Accounting_Controlls(OperationType.Purchase).Content;
            SetButtonBackgroundColorWhenClicked(sender as Button);
        }
        private void SalesButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Content = new Accounting_Controlls(OperationType.Sale).Content; 
            SetButtonBackgroundColorWhenClicked(sender as Button);
        }
        private void Firms_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Content = new FirmPackets(FirmNameTxt).Content;
            SetButtonBackgroundColorWhenClicked(sender as Button);
        }
        private void KontragentiButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Content = new KontragentiPage().Content;
            SetButtonBackgroundColorWhenClicked(sender as Button);
        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Content = new SettingsPage(UserPermissions).Content;
            SetButtonBackgroundColorWhenClicked(sender as Button);
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
        private void UsersBtn_Click(object sender, RoutedEventArgs e)
        {
            UiNavigationHelper.OpenLoginWindow();
            this.Close();
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
