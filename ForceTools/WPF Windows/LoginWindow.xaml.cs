using ForceTools.ViewModels;
using iTextSharp.text;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ForceTools.WPF_Windows
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window, INotifyPropertyChanged
    {
        private string _Account;
        private string _Password;
        public string Account { get { return _Account; } set { _Account = value; OnPropertyChanged(); } }
        public string Password { get { return _Password; } set { _Password = value; OnPropertyChanged(); } }

        private string DataFolderPath = AppDomain.CurrentDomain.BaseDirectory + "\\Database";
        private string TempFolderPath = AppDomain.CurrentDomain.BaseDirectory + "\\Temp";

        private bool IsFirstTimeLogin = false;

        public LoginWindow()
        {
            InitializeComponent();
            //DebugConnection();
            PasswordTb.Focus();
            //Account = "sa";
            //Password = "1";

            CheckForDatabaseFolder();
            CheckForTempFolder();
            CheckConfigFileConnection();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CheckForDatabaseFolder()
        {
            if (!Directory.Exists(DataFolderPath))
            {
                Directory.CreateDirectory(DataFolderPath);
            }
        }

        private void CheckForTempFolder()
        {
            if (!Directory.Exists(TempFolderPath))
            {
                Directory.CreateDirectory(TempFolderPath);
            }
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Account == null || Account == "") 
            {
                MessageBox.Show("Потребителското име не може да бъде празно !", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (Password == null || Password == "")
            {
                MessageBox.Show("Паролата не може да бъде празна !", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (ServerPickingTc.Visibility == Visibility.Visible)
            {
                SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder();
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.ConnectionStrings.ConnectionStrings;

                switch (ServerPickingTc.SelectedIndex)
                {
                    case 0:
                        scsb.DataSource = ServerConectionCb.SelectedItem.ToString();
                        scsb.UserID = Account;
                        scsb.Password = Password;
                        scsb.ConnectTimeout = 30;
                        scsb.ApplicationIntent = ApplicationIntent.ReadWrite;
                        settings["DefaultSqlConnection"].ConnectionString = scsb.ToString();
                        settings["Server"].ConnectionString = ServerConectionCb.SelectedItem.ToString();
                        configFile.Save(ConfigurationSaveMode.Modified);
                        ConfigurationManager.RefreshSection(configFile.ConnectionStrings.SectionInformation.Name);
                        Properties.Settings.Default.Reload();
                        break;
                    case 1:
                        scsb.DataSource = $"{IpTb.Text},{PortTb.Text}";
                        scsb.UserID = Account;
                        scsb.Password = Password;
                        settings["DefaultSqlConnection"].ConnectionString = scsb.ToString();
                        settings["Server"].ConnectionString = $"{IpTb.Text},{PortTb.Text}";
                        configFile.Save(ConfigurationSaveMode.Modified);
                        ConfigurationManager.RefreshSection(configFile.ConnectionStrings.SectionInformation.Name);
                        Properties.Settings.Default.Reload();
                        break;
                }

            }
            else
            {
                SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder();
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.ConnectionStrings.ConnectionStrings;

                scsb.DataSource = settings["Server"].ConnectionString;
                scsb.UserID = Account;
                scsb.Password = Password;
                settings["DefaultSqlConnection"].ConnectionString = scsb.ToString();
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.ConnectionStrings.SectionInformation.Name);
                Properties.Settings.Default.Reload();
            }
            CheckServerAndConnect();
        }

        private async void CheckConfigFileConnection()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.ConnectionStrings.ConnectionStrings;
            if (settings["Server"].ToString() == string.Empty)
            {
                LoginBtn.IsEnabled = false;
                LoadingAnimationMe.Source = new Uri($@"{AppDomain.CurrentDomain.BaseDirectory}\LoadingGif.gif");
                SearchLoadingSp.Visibility = Visibility.Visible;
                await Task.Run(() => GetServers());
                SearchLoadingSp.Visibility = Visibility.Collapsed;
                ServerPickingTc.Visibility = Visibility.Visible;
                LoginBtn.IsEnabled = true;
                IsFirstTimeLogin = true;
            }
            else
            {
                ServerPickingTc.Visibility = Visibility.Collapsed;
                SearchLoadingSp.Visibility = Visibility.Collapsed;
                IsFirstTimeLogin = false;
            }
        }

        private void DebugConnection()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.ConnectionStrings.ConnectionStrings;
            settings["DefaultSqlConnection"].ConnectionString = String.Empty;
            settings["Server"].ConnectionString = String.Empty;
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.ConnectionStrings.SectionInformation.Name);
            Properties.Settings.Default.Reload();
        }

        private void ResetDefaultConString() 
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.ConnectionStrings.ConnectionStrings;
            settings["DefaultSqlConnection"].ConnectionString = String.Empty;
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.ConnectionStrings.SectionInformation.Name);
            Properties.Settings.Default.Reload();
        }

        private void GetServers()
        {
            DataTable AvailableServersDT = SqlDataSourceEnumerator.Instance.GetDataSources();

            //Fill the Server Connection Combo Box with available SQL servers
            foreach (DataRow row in AvailableServersDT.Rows)
            {
                if (row.Field<string>("ServerName") != null)
                {
                    string ServerStrings = row.Field<string>("ServerName") + @"\" + row.Field<string>("InstanceName");
                    ServerConectionCb.Dispatcher.Invoke(new Action(() =>
                    {
                        ServerConectionCb.Items.Add(ServerStrings);
                    }));

                }
            }

            //Select the first Server in ServerCon Combo Box if not empty 
            if (ServerConectionCb.Items.Count > 0)
            {
                ServerConectionCb.Dispatcher.Invoke(new Action(() =>
                {
                    ServerConectionCb.SelectedIndex = 0;
                }));
            }
        }

        private void CheckServerAndConnect()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultSqlConnection"].ConnectionString);
            try
            {
                con.Open();

            }
            catch (System.Exception ex)
            {
                if (ServerPickingTc.Visibility == Visibility.Visible)
                {
                    MessageBox.Show("Съвъра не същестува, няма достъп до него или грешно потребителско име или парола.","",MessageBoxButton.OK,MessageBoxImage.Error);
                }
                else 
                {
                    MessageBox.Show("Грешно потребителско име или парола.","", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Console.WriteLine(ex.ToString(), "ForceTools", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    MainWindow mw;
                    switch (IsFirstTimeLogin)
                    {
                        case true:
                            SqlHelper.FirstTimeLoginSettingsSetter(Account);
                            mw = new MainWindow(Account, CheckIfUserIsAdmin());
                            mw.Show();
                            this.Close();
                            break;
                        case false:
                            mw = new MainWindow(Account, CheckIfUserIsAdmin());
                            mw.Show();
                            this.Close();
                            break;
                    }
                }
                else
                {
                    ResetDefaultConString(); //Have to change this with logic if the server is correct but the login is not only reset the login in thte con string
                }
            }
        }

        private string CheckIfUserIsAdmin()
        {
            if (Account == "sa")
            {
                return "admin";
            }
            else
            {
                using (SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultSqlConnection"].ConnectionString))
                {
                    string command = $"select r.name as Role, m.name as Principal\r\n\r\nfrom\r\n\r\n    master.sys.server_role_members rm\r\n\r\n    inner join\r\n\r\n    master.sys.server_principals r on r.principal_id = rm.role_principal_id and r.type = 'R'\r\n\r\n    inner join\r\n\r\n    master.sys.server_principals m on m.principal_id = rm.member_principal_id\r\n\r\nwhere m.name = '{Account}' and r.name = 'ForceToolsAdmin'";

                    using (SqlCommand sqlcmd = new SqlCommand(command, Con))
                    {
                        Con.Open();
                        using (SqlDataAdapter Adapter = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable checkIfAdmin = new DataTable();
                            Adapter.Fill(checkIfAdmin);

                            if (checkIfAdmin.Rows.Count > 0)
                            {
                                return "admin";
                            }
                            else
                            {
                                return "operator";
                            }
                        }
                    }
                }
            }
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            LoadingAnimationMe.Position = new TimeSpan(0, 0, 1);
        }

    } 
}
