using ForceTools.WPF_Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ForceTools
{
    public static class UiNavigationHelper
    {
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
    }
}
