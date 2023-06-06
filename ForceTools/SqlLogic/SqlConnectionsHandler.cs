using Microsoft.Office.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ForceTools
{
    public static class SqlConnectionsHandler
    {
        public static async Task<List<string>> GetListOfAvailableServersAsync()
        {
            DataTable AvailableServersDT = await Task.Run(() => SqlDataSourceEnumerator.Instance.GetDataSources());
            List<string> ServersList = new List<string>();

            foreach (DataRow row in AvailableServersDT.Rows)
            {
                if (row.Field<string>("ServerName") != null)
                {
                    string ServerStrings = row.Field<string>("ServerName") + @"\" + row.Field<string>("InstanceName");
                    ServersList.Add(ServerStrings);
                }

            }
            return ServersList;
        }
        public static List<string> GetListOfAvailableServers()
        {
            DataTable AvailableServersDT = SqlDataSourceEnumerator.Instance.GetDataSources();
            List<string> ServersList = new List<string>();

            foreach (DataRow row in AvailableServersDT.Rows)
            {
                if (row.Field<string>("ServerName") != null)
                {
                    string ServerStrings = row.Field<string>("ServerName") + @"\" + row.Field<string>("InstanceName");
                    ServersList.Add(ServerStrings);
                }

            }
            return ServersList;
        }
        public static bool DoesConfigFileHaveSavedServers()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.ConnectionStrings.ConnectionStrings;
            if (settings["Server"].ToString() == string.Empty)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static void RemoveSavedServersFromConfig()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.ConnectionStrings.ConnectionStrings;
            settings["DefaultSqlConnection"].ConnectionString = string.Empty;
            settings["Server"].ConnectionString = string.Empty;
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.ConnectionStrings.SectionInformation.Name);
            Properties.Settings.Default.Reload();
        }
        public static void SaveServerCredentialsToConfigFile(string dataSource, string account, string password)
        {
            SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder();
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.ConnectionStrings.ConnectionStrings;

            scsb.DataSource = dataSource;
            scsb.UserID = account;
            scsb.Password = password;
            scsb.ConnectTimeout = 30;
            scsb.ApplicationIntent = ApplicationIntent.ReadWrite;
            settings["DefaultSqlConnection"].ConnectionString = scsb.ToString();
            settings["Server"].ConnectionString = dataSource;
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.ConnectionStrings.SectionInformation.Name);
            Properties.Settings.Default.Reload();
        }
        public static void AlterUserIdAndPasswordInConfigFile(string account, string password)
        {
            SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder();
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.ConnectionStrings.ConnectionStrings;

            scsb.DataSource = settings["Server"].ConnectionString;
            scsb.UserID = account;
            scsb.Password = password;
            settings["DefaultSqlConnection"].ConnectionString = scsb.ToString();
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.ConnectionStrings.SectionInformation.Name);
            Properties.Settings.Default.Reload();
        }
        public static UserPermissions CheckUserPermissions(string account)
        {
            if (account == "sa")
            {
                return UserPermissions.Admin;
            }
            else
            {
                using (SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultSqlConnection"].ConnectionString))
                {
                    string command = $"select r.name as Role, m.name as Principal\r\n\r\nfrom\r\n\r\n    master.sys.server_role_members rm\r\n\r\n    inner join\r\n\r\n    master.sys.server_principals r on r.principal_id = rm.role_principal_id and r.type = 'R'\r\n\r\n    inner join\r\n\r\n    master.sys.server_principals m on m.principal_id = rm.member_principal_id\r\n\r\nwhere m.name = '{account}' and r.name = 'ForceToolsAdmin'";

                    using (SqlCommand sqlcmd = new SqlCommand(command, Con))
                    {
                        Con.Open();
                        using (SqlDataAdapter Adapter = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable checkIfAdmin = new DataTable();
                            Adapter.Fill(checkIfAdmin);

                            if (checkIfAdmin.Rows.Count > 0)
                            {
                                return UserPermissions.Admin;
                            }
                            else
                            {
                                return UserPermissions.Operator;
                            }
                        }
                    }
                }
            }
        }
        public static bool CheckIfServerConnectionIsValid() 
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultSqlConnection"].ConnectionString);
            bool userCanConnect = false;
            try
            {
                con.Open();
                if (con.State == ConnectionState.Open) userCanConnect = true;
            }
            catch (System.Exception ex)
            { 
                Console.WriteLine(ex.ToString(), "ForceTools", MessageBoxButton.OK, MessageBoxImage.Information);
                userCanConnect = false;
            }
            return userCanConnect;
        }
    }
}
