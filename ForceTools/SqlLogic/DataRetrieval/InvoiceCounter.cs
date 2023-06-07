using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ForceTools
{
    public static class InvoiceCounter
    {
        private static SqlConnection sqlConnection;
        private static SqlCommand sqlCommand = new SqlCommand();
        private static SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
        private static DataTable CountingDataTable = new DataTable();

        public static int GetNewInvoicesCount(OperationType operationType)
        {
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                sqlConnection.Open();
                sqlCommand.Connection = sqlConnection;
                switch (operationType)
                {
                    case OperationType.Purchase:
                        sqlCommand.CommandText = "select count (*) as Counting from Fakturi where (AccountingStatusId = 1 and PurchaseOrSale = 'Purchase')";
                        break;
                    case OperationType.Sale:
                        sqlCommand.CommandText = "select count (*) as Counting from Fakturi where (AccountingStatusId = 1 and PurchaseOrSale = 'Sale')";
                        break;
                }
                sqlDataAdapter.Fill(CountingDataTable);
            }
            int InvoiceCount = CountingDataTable.Rows[0].Field<int>("Counting");
            CountingDataTable.Clear();
            return InvoiceCount;
        }
        public static int GetUnAccountedInvoicesCount(OperationType operationType)
        {
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                sqlConnection.Open();
                sqlCommand.Connection = sqlConnection;
                switch (operationType)
                {
                    case OperationType.Purchase:
                        sqlCommand.CommandText = "select count (*) as Counting from Fakturi where (AccountingStatusId = 2 and PurchaseOrSale = 'Purchase')";
                        break;
                    case OperationType.Sale:
                        sqlCommand.CommandText = "select count (*) as Counting from Fakturi where (AccountingStatusId = 2 and PurchaseOrSale = 'Sale')";
                        break;
                }
                sqlDataAdapter.Fill(CountingDataTable);
            }
            int InvoiceCount = CountingDataTable.Rows[0].Field<int>("Counting");
            CountingDataTable.Clear();
            return InvoiceCount;
        }
        public static int GetReadyToBeExportedInvoicesCount(OperationType operationType)
        {
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                sqlConnection.Open();
                sqlCommand.Connection = sqlConnection;
                switch (operationType)
                {
                    case OperationType.Purchase:
                        sqlCommand.CommandText = "select count (*) as Counting from Fakturi where (AccountingStatusId = 3 and PurchaseOrSale = 'Purchase')";
                        break;
                    case OperationType.Sale:
                        sqlCommand.CommandText = "select count (*) as Counting from Fakturi where (AccountingStatusId = 3 and PurchaseOrSale = 'Sale')";
                        break;
                }
                sqlDataAdapter.Fill(CountingDataTable);
            }
            int InvoiceCount = CountingDataTable.Rows[0].Field<int>("Counting");
            CountingDataTable.Clear();
            return InvoiceCount;
        }
        public static int GetExportedInvoicesCount(OperationType operationType)
        {
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                sqlConnection.Open();
                sqlCommand.Connection = sqlConnection;
                switch (operationType)
                {
                    case OperationType.Purchase:
                        sqlCommand.CommandText = "select count (*) as Counting from Fakturi where (AccountingStatusId = 4 and PurchaseOrSale = 'Purchase')";
                        break;
                    case OperationType.Sale:
                        sqlCommand.CommandText = "select count (*) as Counting from Fakturi where (AccountingStatusId = 4 and PurchaseOrSale = 'Sale')";
                        break;
                }
                sqlDataAdapter.Fill(CountingDataTable);
            }
            int InvoiceCount = CountingDataTable.Rows[0].Field<int>("Counting");
            CountingDataTable.Clear();
            return InvoiceCount;
        }
        public static int GetAllInvoicesCount(OperationType operationType)
        {
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                sqlConnection.Open();
                sqlCommand.Connection = sqlConnection;
                switch (operationType)
                {
                    case OperationType.Purchase:
                        sqlCommand.CommandText = "select count (*) as Counting from Fakturi where PurchaseOrSale = 'Purchase'";
                        break;
                    case OperationType.Sale:
                        sqlCommand.CommandText = "select count (*) as Counting from Fakturi where PurchaseOrSale = 'Sale'";
                        break;
                }
                sqlDataAdapter.Fill(CountingDataTable);
            }
            int InvoiceCount = CountingDataTable.Rows[0].Field<int>("Counting");
            CountingDataTable.Clear();
            return InvoiceCount;
        }
    }
}
