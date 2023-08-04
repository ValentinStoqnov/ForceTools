using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ForceTools
{
    public class KontragentEditor
    {
        private static SqlConnection sqlConnection;
        private static SqlCommand sqlCommand;
        private static SqlDataAdapter sqlDataAdapter;

        public static void UpdateKontragent(Kontragent kontragent)
        {
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                DataTable KontragentTable = new DataTable();
                sqlCommand = new SqlCommand($"Select * from Kontragenti where Kontragenti.Id = {kontragent.Id}", sqlConnection);
                sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(KontragentTable);
                //UNFINISHED METHOD ////////////////////////////////////////////////////
                KontragentTable.Rows[0][1] = kontragent.Name;

                SqlCommandBuilder builder = new SqlCommandBuilder(sqlDataAdapter);
                sqlDataAdapter.UpdateCommand = builder.GetUpdateCommand();
                sqlDataAdapter.Update(KontragentTable);
            }
        }
        public static void UpdateKontragentAccAndNoteData(Kontragent kontragent, OperationType operationType)
        {
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                DataTable LastUsedDataTable = new DataTable();
                sqlCommand = new SqlCommand($"Select * from LastUsedKontragentData where LastUsedKontragentData.Id = {kontragent.LastUsedDataId}", sqlConnection);
                sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(LastUsedDataTable);
                if (operationType == OperationType.Purchase)
                {
                    LastUsedDataTable.Rows[0][1] = kontragent.LastPurchaseAccount;
                    LastUsedDataTable.Rows[0][3] = kontragent.LastPurchaseNote;
                }
                else
                {
                    LastUsedDataTable.Rows[0][2] = kontragent.LastSaleAccount;
                    LastUsedDataTable.Rows[0][4] = kontragent.LastSaleNote;
                }
                SqlCommandBuilder builder = new SqlCommandBuilder(sqlDataAdapter);
                sqlDataAdapter.UpdateCommand = builder.GetUpdateCommand();
                sqlDataAdapter.Update(LastUsedDataTable);
            }
        }
        public static int InsertNewKontragentAccAndNoteDataAndGetId()
        {
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                sqlConnection.Open();
                sqlCommand = new SqlCommand("INSERT INTO LastUsedKontragentData(PurchaseAcc,PurchaseNote,SaleAcc,SaleNote) output INSERTED.ID VALUES(@PurchaseAcc,@SaleAcc,@PurchaseNote,@SaleNote)", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@PurchaseAcc", DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@PurchaseNote", DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@SaleAcc", DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@SaleNote", DBNull.Value);

                return (int)sqlCommand.ExecuteScalar();
            }
        }
        public static void InsertNewKontragentInSqlTable(string kontragentName, string kontragentEik, string kontragentDdsNumber)
        {
            int newAccAndNoteDataId = InsertNewKontragentAccAndNoteDataAndGetId();
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                sqlConnection.Open();
                sqlCommand = new SqlCommand("INSERT into Kontragenti (Name, EIK, DDSNumber,LastUsedDataId) VALUES (@KontragentName, @EIK, @DDSNumber,@LastUsedDataId)", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@KontragentName", kontragentName);
                sqlCommand.Parameters.AddWithValue("@EIK", kontragentEik);
                sqlCommand.Parameters.AddWithValue("@DDSNumber", kontragentDdsNumber);
                sqlCommand.Parameters.AddWithValue("@LastUsedDataId", newAccAndNoteDataId);
                sqlCommand.ExecuteNonQuery();
            }
        }
        public static bool CheckIfKontragentExists(string kontragentEik)
        {
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                sqlConnection.Open();
                sqlCommand = new SqlCommand("Select Kontragenti.Id from Kontragenti where EIK like '%" + kontragentEik + "%'", sqlConnection);
                sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                DataTable KontByIdTb = new DataTable("KontById");
                sqlDataAdapter.Fill(KontByIdTb);
                if (KontByIdTb.Rows.Count == 1)
                {
                    return true;
                }
                return false;
            }
        }
        public static Kontragent GetOrCreateNewKontragent(string kontragentName, string kontragentEik, string kontragentDdsNumber)
        {
            Kontragent Kontragent = new Kontragent();
            if (CheckIfKontragentExists(kontragentEik) == false) InsertNewKontragentInSqlTable(kontragentName, kontragentEik, kontragentDdsNumber);
            Kontragent = GetKontragent(kontragentEik);
            return Kontragent;
            
        }
        public static Kontragent GetKontragent(string kontragentEik)
        {
            Kontragent Kontragent = new Kontragent();
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                sqlConnection.Open();
                sqlCommand = new SqlCommand($"Select * from Kontragenti join LastUsedKontragentData on Kontragenti.LastUsedDataId = LastUsedKontragentData.Id where EIK like '%{kontragentEik}%' ", sqlConnection);
                sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                DataTable KontByIdTb = new DataTable("KontById");
                sqlDataAdapter.Fill(KontByIdTb);
                if (KontByIdTb.Rows.Count < 1) return Kontragent;
                Kontragent.Id = KontByIdTb.Rows[0].Field<int>("Id");
                Kontragent.Name = KontByIdTb.Rows[0].Field<string>("Name");
                Kontragent.EIK = KontByIdTb.Rows[0].Field<string>("EIK");
                Kontragent.DdsNumber = KontByIdTb.Rows[0].Field<string>("DDSNumber");
                Kontragent.LastUsedDataId = KontByIdTb.Rows[0].Field<int>("LastUsedDataId");
                Kontragent.LastPurchaseAccount = KontByIdTb.Rows[0].Field<int?>("PurchaseAcc");
                Kontragent.LastSaleAccount = KontByIdTb.Rows[0].Field<int?>("SaleAcc");
                Kontragent.LastPurchaseNote = KontByIdTb.Rows[0].Field<string>("PurchaseNote");
                Kontragent.LastSaleNote = KontByIdTb.Rows[0].Field<string>("SaleNote");
            }

            return Kontragent;
        }
    }
}
