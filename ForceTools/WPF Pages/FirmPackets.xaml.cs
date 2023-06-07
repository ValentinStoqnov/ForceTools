using ForceTools.Models;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ForceTools.WPF_Pages
{
    /// <summary>
    /// Interaction logic for FirmPackets.xaml
    /// </summary>
    public partial class FirmPackets : Page, INotifyPropertyChanged
    {
        private ObservableCollection<Databases> _DataBaseCol = new ObservableCollection<Databases>();
        public ObservableCollection<Databases> DataBaseCol { get { return _DataBaseCol; } set { _DataBaseCol = value; OnPropertyChanged(); } }
        public string NewDbTextBlock { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public FirmPackets()
        {
            InitializeComponent();
            RefreshDatabaseCollectionData();
        }
        public FirmPackets(string FirmnameTxt) : this()
        {
            SetSelectedDatabaseIconOnStart(FirmnameTxt);
        }

        private void SetSelectedDatabaseIconOnStart(string SelectedDatabase)
        {
            foreach (Databases databases in DataBaseCol)
            {
                if (databases.dbName.ToString() == SelectedDatabase)
                {
                    databases.BitImage = BitmapCreator.SelectedDatabaseBitmapImage();
                }
            }
        }
        private void SetSelectedDatabaseIconOnDatabaseChange(Databases database) 
        {
            foreach (Databases dbss in PacketsLv.Items)
            {
                if (dbss.dbId != database.dbId)
                {
                    dbss.BitImage = BitmapCreator.DefaultDatabaseBitmapImage();
                }
                else
                {
                    dbss.BitImage = BitmapCreator.SelectedDatabaseBitmapImage();
                }
            }
        }
        private void RefreshDatabaseCollectionData()
        {
            DataBaseCol = SqlDatabaseHandler.GetUserCreatedDatabasesObservableCollection();
        }
        private void SendSelectedDatabaseInfoToMainwindow(string databaseName) 
        {
            UiNavigationHelper.MainWindow.FirmNameTxt = databaseName;
            UiNavigationHelper.MainWindow.PurchasesButton.IsEnabled = true;
            UiNavigationHelper.MainWindow.SalesButton.IsEnabled = true;
            UiNavigationHelper.MainWindow.Settings.IsEnabled = true;
            UiNavigationHelper.MainWindow.KontragentiButton.IsEnabled = true;
        }
        private void ResetSelectedDatabaseInfoForMainwindow() 
        {
            UiNavigationHelper.MainWindow.FirmNameTxt = "Изберете пакет";
            UiNavigationHelper.MainWindow.PurchasesButton.IsEnabled = false;
            UiNavigationHelper.MainWindow.SalesButton.IsEnabled = false;
            UiNavigationHelper.MainWindow.Settings.IsEnabled = false;
            UiNavigationHelper.MainWindow.KontragentiButton.IsEnabled = false;
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var database = ((FrameworkElement)e.OriginalSource).DataContext as Databases;
            if (database == null) return;
            SqlConnectionsHandler.ConnectToDifferentDatabase(database.dbName);
            SendSelectedDatabaseInfoToMainwindow(database.dbName);
            SetSelectedDatabaseIconOnDatabaseChange(database);
        }
        private void CreateNewDb_Click(object sender, RoutedEventArgs e)
        {
            NewDbPopup.IsOpen = true;
        }
        private void PopupCreateBtn_Click(object sender, RoutedEventArgs e)
        {
            NewDbPopup.IsOpen = false;
            SqlDatabaseHandler.CreateNewDatabase(NewDbTextBlock, ConfigurationManager.ConnectionStrings["DefaultSqlConnection"].ConnectionString);
            RefreshDatabaseCollectionData();
        }
        private void PopupCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            NewDbPopup.IsOpen = false;
        }
        private void DeleteDb_Click(object sender, RoutedEventArgs e)
        {
            Databases selectedDatabase = PacketsLv.SelectedItem as Databases;
            var mb = MessageBox.Show($"Сигурни ли сте че искате да изтриете {selectedDatabase.dbName}", "Изтриване на пакет", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (mb == MessageBoxResult.Yes)
            {
                if (UiNavigationHelper.MainWindow.FirmNameTxt == selectedDatabase.dbName) ResetSelectedDatabaseInfoForMainwindow();
                SqlDatabaseHandler.DeleteDatabase(selectedDatabase.dbName, ConfigurationManager.ConnectionStrings["DefaultSqlConnection"].ConnectionString);
                RefreshDatabaseCollectionData();
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}