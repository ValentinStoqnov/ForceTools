using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ForceTools
{
    public static class SavedTypesRetriever
    {
        private static DataTable GetTypesDataTable(string sqlCommandText)
        {
            SqlConnection sqlConnection;
            SqlCommand sqlCommand;
            SqlDataAdapter sqlDataAdapter;
            DataTable typesDataTable = new DataTable();

            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                sqlCommand = new SqlCommand(sqlCommandText, sqlConnection);
                sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(typesDataTable);
            }

            return typesDataTable;
        }
        public static DataTable GetDocTypesDataTable()
        {
            DataTable docTypesDataTable = GetTypesDataTable("select * from DocumentTypes");
            return docTypesDataTable;
        }
        public static DataTable GetKindOfDealsDataTable()
        {
            DataTable kindOfDealsDataTable = GetTypesDataTable("select * from KindOfDeals");
            return kindOfDealsDataTable;
        }
    }
}
