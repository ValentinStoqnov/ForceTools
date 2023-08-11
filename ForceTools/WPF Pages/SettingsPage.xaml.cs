using ForceTools.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
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

namespace ForceTools.WPF_Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        DataTable Dt = new DataTable("DefaultValues");
        public DataTable AccountsDataTable { get; set; } = new DataTable();
        public DataTable KindOfDealsDataTable { get; set; } = new DataTable();
        public DataTable UsersDataTable { get; set; } = new DataTable();

        public SettingsPage()
        {
            InitializeComponent();

            GetSavedDefaultValues();
            GetAccountsData();
            GetDealKindsData();
            GetUsersData();
        }

        public SettingsPage(UserPermissions userPermissions) : this()
        {
            GetAdminSettingsAccess(userPermissions);
        }

        private void GetAdminSettingsAccess(UserPermissions userPermissions)
        {
            if (userPermissions == UserPermissions.Admin)
            {
                AdminTab.IsEnabled = true;
            }
            else
            {
                AdminTab.IsEnabled = false;
            }
        }

        private void GetSavedDefaultValues()
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                using (SqlCommand Cmd = new SqlCommand("select * from DefaultValues", sqlConnection))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                    sda.Fill(Dt);

                    DefaultPurchaseTb.Text = Dt.Rows[0][2].ToString();
                    DefaultSaleTb.Text = Dt.Rows[1][2].ToString();
                    DefaultCashRegTb.Text = Dt.Rows[2][2].ToString();
                    DefaultPurchaseNoteTb.Text = Dt.Rows[3][2].ToString();
                    DefaultSaleNoteTb.Text = Dt.Rows[4][2].ToString();
                }
            }
        }

        private void GetAccountsData()
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                using (SqlCommand Cmd = new SqlCommand("select * from Accounts", sqlConnection))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                    sda.Fill(AccountsDataTable);
                }
            }
        }

        private void GetDealKindsData()
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                using (SqlCommand Cmd = new SqlCommand("select * from KindOfDeals", sqlConnection))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                    sda.Fill(KindOfDealsDataTable);
                }
            }
        }

        private void GetUsersData()
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultSqlConnection"].ConnectionString))
            {
                DataTable SeeIfAdminDt = new DataTable();
                sqlConnection.Open();
                SqlCommand sqcmd = new SqlCommand($"select m.name as Principal from master.sys.server_role_members rm inner join master.sys.server_principals r on r.principal_id = rm.role_principal_id and r.type = 'R' inner join master.sys.server_principals m on m.principal_id = rm.member_principal_id where r.name = 'sysadmin' and m.name not like '%NT SERVICE%' and m.name not like '%{Environment.MachineName}%'", sqlConnection);

                SqlDataAdapter adapter = new SqlDataAdapter(sqcmd);
                UsersDataTable.Clear();
                adapter.Fill(UsersDataTable);
                if (UsersDataTable.Columns.Contains("isAdmin") != true)
                {
                    UsersDataTable.Columns.Add("isAdmin");
                }
                foreach (DataRow dr in UsersDataTable.Rows)
                {
                    if (dr[0].ToString() == "sa")
                    {
                        dr[1] = "Главен Админинистратор";
                    }
                    else 
                    {
                        SqlCommand cmd = new SqlCommand($"select m.name as Principal\r\n\r\nfrom\r\n\r\n    master.sys.server_role_members rm\r\n\r\n    inner join\r\n\r\n    master.sys.server_principals r on r.principal_id = rm.role_principal_id and r.type = 'R'\r\n\r\n    inner join\r\n\r\n    master.sys.server_principals m on m.principal_id = rm.member_principal_id\r\n\r\nwhere r.name = 'ForceToolsAdmin' and m.name = '{dr[0].ToString()}'\r\n", sqlConnection);
                        SqlDataAdapter adapter2 = new SqlDataAdapter(cmd);
                        SeeIfAdminDt.Clear();
                        adapter2.Fill(SeeIfAdminDt);

                        if (SeeIfAdminDt.Rows.Count > 0)
                        {
                            dr[1] = "Админинистратор";
                        }
                        else
                        {
                            dr[1] = "Оператор";
                        }
                    }  
                }
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                using (SqlCommand Cmd = new SqlCommand("select * from DefaultValues", sqlConnection))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                    //sda.Fill(Dt);
                    try
                    {
                        Dt.Rows[0][2] = Convert.ToInt32(DefaultPurchaseTb.Text);
                        Dt.Rows[1][2] = Convert.ToInt32(DefaultSaleTb.Text);
                        Dt.Rows[2][2] = Convert.ToInt32(DefaultCashRegTb.Text);
                        Dt.Rows[3][2] = Convert.ToString(DefaultPurchaseNoteTb.Text);
                        Dt.Rows[4][2] = Convert.ToString(DefaultSaleNoteTb.Text);

                        SqlCommandBuilder builder = new SqlCommandBuilder(sda);
                        sda.UpdateCommand = builder.GetUpdateCommand();
                        sda.Update(Dt);
                        SuccessLbl.Visibility = Visibility.Visible;
                    }
                    catch
                    {
                        SuccessLbl.Content = "Неуспешна операция";
                        SuccessLbl.Visibility = Visibility.Visible;
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                }
            }
        }

        private void SearchBarBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SearchBarTb.IsVisible == true && SearchBarLbl.IsVisible == true)
            {
                SearchBarTb.Visibility = Visibility.Collapsed;
                SearchBarLbl.Visibility = Visibility.Collapsed;
            }
            else
            {
                SearchBarTb.Visibility = Visibility.Visible;
                SearchBarLbl.Visibility = Visibility.Visible;
                Keyboard.Focus(SearchBarTb);
            }
        }

        private void SearchBarTb_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void UsersDg_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Delete)
            {
                try
                {
                    DataRowView dataGridRowView = (DataRowView)UsersDg.Items[UsersDg.SelectedIndex];
                    if ((string)dataGridRowView[1] == "sa")
                    {
                        MessageBox.Show("Главният администраторски потребител не може да бъде изтрит.", "Изтриване на потребител", MessageBoxButton.OK, MessageBoxImage.Error);
                        e.Handled = true;
                    }
                }
                catch
                {

                }
            }
        }

        private void CreateNewUserBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowUserCreationVisBorder("Create");
        }

        private void EditUserBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowUserCreationVisBorder("Edit");
        }

        private void DeleteUserBtn_Click(object sender, RoutedEventArgs e)
        {
            DataRowView UserDataRow = (DataRowView)UsersDg.SelectedItem;
            MainWindow mw = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive) as MainWindow;
            if (UserDataRow.Row[0].ToString() == mw.CurrentUserLbl.Content.ToString())
            {
                MessageBox.Show("Не можете да изтриете текущия активен потребител");
                return;
            }
            if (UserDataRow.Row[0].ToString() == "sa")
            {
                MessageBox.Show("Не можете да изтриете главния администраторски потребител");
                return;
            }

            var result = MessageBox.Show($"Сигурни ли сте че искате да изтриете потребител \"{UserDataRow.Row[0].ToString()}\" ?", "Изтриване на потребител", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using (SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultSqlConnection"].ConnectionString))
                {
                    Con.Open();
                    using (SqlCommand cmd = new SqlCommand($"DROP LOGIN {UserDataRow.Row[0].ToString()}", Con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    Con.Close();
                }
            }
            GetUsersData();
        }

        private void ShowUserCreationVisBorder(string CreateOrEdit)
        {
            switch (CreateOrEdit)
            {
                case "Create":
                    UserCreationVisBorder.Visibility = Visibility.Visible;
                    UserAccCreateTb.IsEnabled = true;
                    UserCreationLabel.Content = "Създаване на нов потребител";
                    UserCreationCb.SelectedIndex = 0;
                    break;
                case "Edit":
                    DataRowView UserDataRow = (DataRowView)UsersDg.SelectedItem;
                    MainWindow mw = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive) as MainWindow;
                    if (UserDataRow.Row[0].ToString() == mw.CurrentUserLbl.Content.ToString())
                    {
                        MessageBox.Show("Не можете да редактирате текущия активен потребител");
                        return;
                    }
                    if (UserDataRow.Row[0].ToString() == "sa")
                    {
                        MessageBox.Show("Не можете да редактирате главния администраторски потребител");
                        return;
                    }
                    UserCreationVisBorder.Visibility = Visibility.Visible;
                    UserAccCreateTb.IsEnabled = false;
                    UserAccCreateTb.Text = UserDataRow.Row[0].ToString();
                    UserPasswordCreateTb.Text = "******";
                    UserCreationCb.SelectedIndex = -1;
                    UserCreationLabel.Content = "Редакция на потребител";
                    break;
            }
        }

        private void UserCreationCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            UserCreationVisBorder.Visibility = Visibility.Hidden;
        }

        private void UserCreationSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UserAccCreateTb.Text == "")
            {
                MessageBox.Show("Полето \"Потребител\" не може да бъде празно!");
                return;
            }
            if (UserPasswordCreateTb.Text == "")
            {
                MessageBox.Show("Полето \"Парола\" не може да бъде празно!");
                return;
            }
            switch (UserAccCreateTb.IsEnabled)
            {
                case true:
                    using (SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultSqlConnection"].ConnectionString))
                    {
                        Con.Open();
                        using (SqlCommand Cmd = new SqlCommand($"CREATE LOGIN {UserAccCreateTb.Text} WITH PASSWORD = '{UserPasswordCreateTb.Text}';", Con))
                        {
                            Cmd.ExecuteNonQuery();
                        }
                        using (SqlCommand Cmd = new SqlCommand($"ALTER SERVER ROLE sysadmin ADD MEMBER {UserAccCreateTb.Text};", Con))
                        {
                            Cmd.ExecuteNonQuery();
                        }
                        if (UserCreationCb.SelectedIndex == 1)
                        {
                            using (SqlCommand Cmd = new SqlCommand($"ALTER SERVER ROLE ForceToolsAdmin ADD MEMBER {UserAccCreateTb.Text};", Con))
                            {
                                Cmd.ExecuteNonQuery();
                            }
                        }
                        Con.Close();
                    }
                    break;
                case false:
                    using (SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultSqlConnection"].ConnectionString))
                    {
                        Con.Open();
                        if (UserPasswordCreateTb.Text != "******" || UserPasswordCreateTb.Text != "")
                        {
                            using (SqlCommand Cmd = new SqlCommand($"ALTER LOGIN {UserAccCreateTb.Text} WITH PASSWORD = '{UserPasswordCreateTb.Text}';", Con))
                            {
                                Cmd.ExecuteNonQuery();
                            }
                        }

                        switch (UserCreationCb.SelectedIndex) 
                        {
                            case 0:
                                using (SqlCommand Cmd = new SqlCommand($"ALTER SERVER ROLE ForceToolsAdmin DROP MEMBER {UserAccCreateTb.Text};", Con))
                                {
                                    Cmd.ExecuteNonQuery();
                                }
                                break;
                            case 1:
                                using (SqlCommand Cmd = new SqlCommand($"ALTER SERVER ROLE ForceToolsAdmin ADD MEMBER {UserAccCreateTb.Text};", Con))
                                {
                                    Cmd.ExecuteNonQuery();
                                }
                                break;
                        }
                        Con.Close();
                    }
                    break;
            }
            UserCreationVisBorder.Visibility = Visibility.Hidden;
            GetUsersData();
        }

        private void AccountSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                using (SqlCommand Cmd = new SqlCommand("select * from Accounts", sqlConnection))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                    //sda.Fill(AccountsDataTable);
                    SqlCommandBuilder sqlCommandBuilder = new SqlCommandBuilder(sda);
                    sda.UpdateCommand = sqlCommandBuilder.GetUpdateCommand();
                    sda.Update(AccountsDataTable);
                }
            }
        }
    }
}
