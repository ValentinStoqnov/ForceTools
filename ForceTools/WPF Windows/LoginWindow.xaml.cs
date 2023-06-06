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
    public partial class LoginWindow : Window
    {
        public string Account { get; set; }
        public string Password { get; set; }

        private bool isFirstTimeLogin = false;

        public LoginWindow()
        {
            InitializeComponent();
            //SqlConnectionsHandler.RemoveSavedServersFromConfig();
            PasswordTb.Focus();
            //Account = "sa";
            //Password = "1";

            FileSystemHelper.CheckAndCreateDatabaseFolder();
            FileSystemHelper.CheckAndCreateTempFolder();
            SetUpWindowControls();
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
                if (isFirstTimeLogin == true) SqlHelper.FirstTimeLoginSettingsSetter(Account);
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

    }
}
