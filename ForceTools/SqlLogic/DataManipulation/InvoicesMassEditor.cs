using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ForceTools
{
    public class InvoicesMassEditor
    {
        private static SqlConnection sqlConnection;
        private static SqlCommand sqlCommand = new SqlCommand();
        private static SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
        private static SqlCommandBuilder builder;
        private static DataTable UpdateDt = new DataTable();

        public static void DeleteInvoicesFromFakturiTable(List<string> invoicesToBeDeletedList)
        {
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                sqlConnection.Open();
                sqlCommand.Connection = sqlConnection;

                foreach (string InvoiceId in invoicesToBeDeletedList)
                {
                    sqlCommand.CommandText = $"DELETE FROM Fakturi WHERE id = {InvoiceId}";
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }
        public static void ChangeInvoicesDates(List<string> invoicesToBeAffectedList, string newDate)
        {
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                sqlConnection.Open();
                foreach (string InvoiceId in invoicesToBeAffectedList)
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandText = $"select * from Fakturi where id = {InvoiceId}";
                    sqlDataAdapter.Fill(UpdateDt);
                    UpdateDt.Rows[0][3] = newDate;
                    builder = new SqlCommandBuilder(sqlDataAdapter);
                    sqlDataAdapter.UpdateCommand = builder.GetUpdateCommand();
                    sqlDataAdapter.Update(UpdateDt);
                    UpdateDt.Clear();
                }
            }
        }
        public static void ChangeInvoicesStatuses(List<string> invoicesToBeAffectedList, int selectedIndex)
        {
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                sqlConnection.Open();
                foreach (string InvoiceId in invoicesToBeAffectedList)
                {
                    sqlCommand.CommandText = $"select * from Fakturi where id = {InvoiceId}";
                    sqlCommand.Connection = sqlConnection;
                    sqlDataAdapter.Fill(UpdateDt);

                    switch (selectedIndex)
                    {
                        case 0:
                            UpdateDt.Rows[0][15] = 1;
                            break;
                        case 1:
                            UpdateDt.Rows[0][15] = 2;
                            break;
                        case 2:
                            UpdateDt.Rows[0][15] = 3;
                            break;
                        case 3:
                            UpdateDt.Rows[0][15] = 4;
                            break;
                        case 4:
                            UpdateDt.Rows[0][15] = 5;
                            break;
                    }
                    builder = new SqlCommandBuilder(sqlDataAdapter);
                    sqlDataAdapter.UpdateCommand = builder.GetUpdateCommand();
                    sqlDataAdapter.Update(UpdateDt);
                    UpdateDt.Clear();
                }
            }
        }
        public static void ChangeInvoicesAccounts(List<string> invoicesToBeAffectedList, string Account)
        {
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                sqlConnection.Open();
                foreach (string InvoiceId in invoicesToBeAffectedList)
                {
                    sqlCommand.CommandText = $"select * from Fakturi where id = {InvoiceId}";
                    sqlCommand.Connection = sqlConnection;
                    sqlDataAdapter.Fill(UpdateDt);

                    UpdateDt.Rows[0][11] = Account;

                    builder = new SqlCommandBuilder(sqlDataAdapter);
                    sqlDataAdapter.UpdateCommand = builder.GetUpdateCommand();
                    sqlDataAdapter.Update(UpdateDt);
                    UpdateDt.Clear();
                }
            }
        }
        public static void ChangeInvoicesNotes(List<string> invoicesToBeAffectedList, string newNote) 
        {
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                sqlConnection.Open();
                foreach (string InvoiceId in invoicesToBeAffectedList)
                {
                    sqlCommand.CommandText = $"select * from Fakturi where id = {InvoiceId}";
                    sqlCommand.Connection = sqlConnection;
                    sqlDataAdapter.Fill(UpdateDt);

                    UpdateDt.Rows[0][13] = newNote;

                    builder = new SqlCommandBuilder(sqlDataAdapter);
                    sqlDataAdapter.UpdateCommand = builder.GetUpdateCommand();
                    sqlDataAdapter.Update(UpdateDt);
                    UpdateDt.Clear();
                }
            }
        } 
    } 
}


