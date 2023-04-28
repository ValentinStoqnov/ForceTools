using ForceTools.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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

namespace ForceTools.WPF_Pages
{
    /// <summary>
    /// Interaction logic for KontragentiPage.xaml
    /// </summary>
    public partial class KontragentiPage : Page
    {

        public DataTable KontragentiDt { get { return _KontragentiDt; } set { _KontragentiDt = value; } }
        private DataTable _KontragentiDt = new DataTable();

        public KontragentiPage()
        {
            InitializeComponent();
            FillKontragentiDt();
        }

        private void FillKontragentiDt()
        {
            using (var SqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                using (var Cmd = new SqlCommand("select * from Kontragenti", SqlCon))
                {
                    using (var SqlDataAdapter = new SqlDataAdapter(Cmd))
                    {
                        SqlDataAdapter.Fill(KontragentiDt);
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
            using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                using (var SqCmd = new SqlCommand($"select * from Kontragenti where Kontragenti.Name like N'%{SearchBarTb.Text}%' or Kontragenti.EIK like N'%{SearchBarTb.Text}%' or Kontragenti.DDSNumber like N'%{SearchBarTb.Text}%' "))
                {
                    SqCmd.Connection = sqlConnection;
                    using (var DataAdapter = new SqlDataAdapter(SqCmd))
                    {
                        KontragentiDt.Clear();
                        DataAdapter.Fill(KontragentiDt);
                    }
                }
            }
        }

        private void KontragentiDg_CurrentCellChanged(object sender, EventArgs e)
        {
            using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                using (var SqCmd = new SqlCommand("select * from Kontragenti", sqlConnection)) 
                {
                    using (var SqlDataAdapter = new SqlDataAdapter(SqCmd)) 
                    {
                        using (var SqlCommandBuilder = new SqlCommandBuilder(SqlDataAdapter)) 
                        {
                            SqlDataAdapter.UpdateCommand = SqlCommandBuilder.GetUpdateCommand();
                            SqlDataAdapter.Update(KontragentiDt);
                        }
                    }
                }      
            }
        }

        private void KontragentiDg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (KontragentiDg.SelectedItems.Count > 0)
                {
                    var Res = MessageBox.Show("Сигурени ли сте че искате да изтриете " + KontragentiDg.SelectedItems.Count + " контрагента?", "Изтриване",
                        MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
                    if (Res == MessageBoxResult.Yes)
                    {
                        MessageBox.Show(KontragentiDg.SelectedItems.Count + " документа бяха изтрити!");
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
        }
    }
}
