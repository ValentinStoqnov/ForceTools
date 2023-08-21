using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace ForceTools.WPF_Windows
{
    public partial class LoginWindow : Window
    {
        public string Account { get; set; }
        public string Password { get; set; }

        private bool isFirstTimeLogin = false;

        public LoginWindow()
        {
            InitializeComponent();
            PasswordTb.Focus();
            //Account = "sa";
            //Password = "1";
            FileSystemHelper.CheckAndCreateDatabaseFolder();
            FileSystemHelper.CheckAndCreateTempFolder();
            SetUpWindowControls();
        }

        private void CheckForUpdates()
        {

            Version localVersion = new Version(Assembly.GetExecutingAssembly().GetName().Version.ToString());
            VersionLabel.Content = $"v: {localVersion}";

            try
            {
                WebClient webClient = new WebClient();
                Version onlineVersion = new Version(webClient.DownloadString("https://raw.githubusercontent.com/ValentinStoqnov/ForceToolsUpdateFiles/main/Version"));
                int isVersionNewerResult = onlineVersion.CompareTo(localVersion);
                if (isVersionNewerResult == 1)
                {
                    var messageBoxResult = MessageBox.Show($"Налична е нова версия на ForceTools: {onlineVersion}, искате ли да обновите програмата?","Нова версия",MessageBoxButton.YesNo,MessageBoxImage.Question);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        this.Close();
                        Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"/ForceToolsUpdater.exe");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking for updates: {ex}");
            }
        }

        private async Task GetServers()
        {
            //Fills combo box with available servers
            ServerConectionCb.ItemsSource = await SqlConnectionsHandler.GetListOfAvailableServersAsync();

            //Select the first Server in the Combo Box if not empty
            if (ServerConectionCb.Items.Count > 0) ServerConectionCb.SelectedIndex = 0;
        }
        private async void SetUpWindowControls()
        {
            if (SqlConnectionsHandler.DoesConfigFileHaveSavedServers() == false)
            {
                isFirstTimeLogin = true;
                LoginBtn.IsEnabled = false;
                LoadingAnimationMe.Source = new Uri($@"{AppDomain.CurrentDomain.BaseDirectory}\LoadingGif.gif");
                SearchLoadingSp.Visibility = Visibility.Visible;
                await GetServers();
                SearchLoadingSp.Visibility = Visibility.Collapsed;
                ServerPickingTc.Visibility = Visibility.Visible;
                LoginBtn.IsEnabled = true;
            }
            else
            {
                isFirstTimeLogin = false;
                ServerPickingTc.Visibility = Visibility.Collapsed;
                SearchLoadingSp.Visibility = Visibility.Collapsed;
            }
        }
        private void ConnectToServerOrReset()
        {
            if (SqlConnectionsHandler.CheckIfServerConnectionIsValid() == true)
            {
                if (isFirstTimeLogin == true) SqlDatabaseHandler.FirstTimeLoginSettingsSetter(Account);
                UiNavigationHelper.OpenMainWindow(Account);
                this.Close();
            }
            else
            {
                if (isFirstTimeLogin == true)
                {
                    MessageBox.Show("Съвъра не същестува, няма достъп до него или грешно потребителско име или парола.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                    SqlConnectionsHandler.RemoveSavedServersFromConfig();
                }
                else
                {
                    MessageBox.Show("Грешно потребителско име или парола.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
            if (isFirstTimeLogin == true)
            {
                switch (ServerPickingTc.SelectedIndex)
                {
                    case 0:
                        string DataSource = ServerConectionCb.SelectedItem.ToString();
                        SqlConnectionsHandler.SaveServerCredentialsToConfigFile(DataSource, Account, Password);
                        break;
                    case 1:
                        SqlConnectionsHandler.SaveServerCredentialsToConfigFile($"{IpTb.Text},{PortTb.Text}", Account, Password);
                        break;
                }
            }
            else
            {
                SqlConnectionsHandler.AlterUserIdAndPasswordInConfigFile(Account, Password);
            }
            ConnectToServerOrReset();
        }
        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            LoadingAnimationMe.Position = new TimeSpan(0, 0, 1);
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            CheckForUpdates();
        }
    }
}
