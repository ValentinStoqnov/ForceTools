using ForceTools.Models;
using ForceTools.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ForceTools.WPF_Pages
{
    /// <summary>
    /// Interaction logic for FirmPackets.xaml
    /// </summary>
    public partial class FirmPackets : Page
    {

        public ObservableCollection<Databases> DataBaseCol { get; set; } = new ObservableCollection<Databases>();
        public string NewDbTextBlock { get; set; }
        string dbSelectedImg = @"\Assets\databaseSelected.png"; 
        string dbDefaultImg = @"\Assets\database.png"; 

        public BitmapImage DefaultImage;
        public BitmapImage SelectedImage;

        public FirmPackets()
        {
            InitializeComponent();
            #region Getting the Databases in the Sql server 

            BitmapImage Defaultbitmap = new BitmapImage();
            Defaultbitmap.BeginInit();
            Defaultbitmap.UriSource = new Uri(dbDefaultImg,UriKind.RelativeOrAbsolute);
            Defaultbitmap.EndInit();
            DefaultImage = Defaultbitmap;

            BitmapImage Selectedbitmap = new BitmapImage();
            Selectedbitmap.BeginInit();
            Selectedbitmap.UriSource = new Uri(dbSelectedImg,UriKind.RelativeOrAbsolute);
            Selectedbitmap.EndInit();
            SelectedImage = Selectedbitmap;

            GetAndFillDbList();

            
            #endregion
        }

        public FirmPackets(string FirmnameTxt) : this()
        {
            #region Viewing the Selected Database
            foreach (Databases dbs in DataBaseCol) 
            {
                if (dbs.dbName.ToString() == FirmnameTxt) 
                {
                    dbs.BitImage = SelectedImage;
                }
            }
            #endregion
        }

        private void GetAndFillDbList() 
        {
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultSqlConnection"].ConnectionString))
            {
                con.Open();
                DataTable databases = con.GetSchema("Databases");
                DataBaseCol.Clear();
                foreach (DataRow database in databases.Rows)
                {
                    String databaseName = database.Field<String>("database_name");
                    string dbID = database.Field<short>("dbid").ToString();
                    //DateTime creationDate = database.Field<DateTime>("create_date");

                    if (databaseName != "master" && databaseName != "tempdb" && databaseName != "model" && databaseName != "msdb" && databaseName != "Users")
                    {
                        Databases data = new Databases(databaseName, dbID, DefaultImage);
                        DataBaseCol.Add(data);
                    }
                }
                con.Close();
            }
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string DbName = "";
            string SelectedPacket;

            //Connecting to the chosen database
            var item = ((FrameworkElement)e.OriginalSource).DataContext as Databases;
            if (item != null)
            {
                DbName = item.dbName;
                SelectedPacket = item.dbId;
            }
            else
            {
                return;
            }

            SqlHelper.ConnectToDifferentDatabase(DbName);

            //Sending the Connected Database name to MainWindow
            MainWindow mw = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive) as MainWindow;
            mw.FirmNameTxt = DbName;
            mw.PurchasesButton.IsEnabled = true;
            mw.SalesButton.IsEnabled = true;
            mw.Settings.IsEnabled = true;
            mw.KontragentiButton.IsEnabled = true;

            //Changing the icon of the chosen database
            Databases dbs = PacketsLv.SelectedItem as Databases;
            dbs.BitImage = SelectedImage;

            //Seting default icons for not-chosen databases
            foreach (Databases dbss in PacketsLv.Items) 
            {
                if (dbss.dbId != SelectedPacket) 
                {
                    dbss.BitImage = DefaultImage;
                }
            }

        }

        private void CreateNewDb_Click(object sender, RoutedEventArgs e)
        {
            NewDbPopup.IsOpen = true;
        }

        private void PopupCreateBtn_Click(object sender, RoutedEventArgs e)
        {
            NewDbPopup.IsOpen = false;
            SqlHelper.CreateNewDatabase(NewDbTextBlock, ConfigurationManager.ConnectionStrings["DefaultSqlConnection"].ConnectionString);
            GetAndFillDbList();
        }
        private void PopupCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            NewDbPopup.IsOpen = false;
        }

        #region Helper Methods
        private static T FindAnchestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);


            }
            while (current != null);
            return null;
        }

        public static Visual GetDescendantByName(Visual element, string name)
        {
            if (element == null) return null;

            if (element is FrameworkElement
                && (element as FrameworkElement).Name == name) return element;

            Visual result = null;

            if (element is FrameworkElement)
                (element as FrameworkElement).ApplyTemplate();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                result = GetDescendantByName(visual, name);
                if (result != null)
                    break;
            }

            return result;
        }


        #endregion

        private void DeleteDb_Click(object sender, RoutedEventArgs e)
        {
            Databases dbs = PacketsLv.SelectedItem as Databases;
            var mb = MessageBox.Show($"Сигурни ли сте че искате да изтриете {dbs.dbName}", "Изтриване на пакет", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (mb == MessageBoxResult.Yes)
            {
                SqlHelper.DeleteDatabase(dbs.dbName, ConfigurationManager.ConnectionStrings["DefaultSqlConnection"].ConnectionString);

                MainWindow mw = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive) as MainWindow;

                if (mw.FirmNameTxt == dbs.dbName) 
                {
                    mw.FirmNameTxt = "Изберете пакет";
                    mw.PurchasesButton.IsEnabled = false;
                    mw.SalesButton.IsEnabled = false;
                    mw.Settings.IsEnabled = false;
                    mw.KontragentiButton.IsEnabled = false;
                }
                DataBaseCol.Remove(dbs);
            } 
        }
    }
}