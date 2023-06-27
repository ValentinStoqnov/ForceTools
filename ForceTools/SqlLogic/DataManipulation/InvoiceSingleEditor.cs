using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Remoting.Contexts;

namespace ForceTools
{
    public class InvoiceSingleEditor
    {
        private static SqlConnection sqlConnection;
        private static SqlCommand sqlCommand;
        private static SqlDataAdapter sqlDataAdapter;

        public static int GetKontragentIdAndUpdateTable(string eikText, string kontText, string ddsNumberText)
        {
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                sqlConnection.Open();
                sqlCommand = new SqlCommand("Select Kontragenti.Id from Kontragenti where EIK like '%" + eikText + "%'", sqlConnection);
                sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                DataTable KontByIdTb = new DataTable("KontById");
                sqlDataAdapter.Fill(KontByIdTb);
                if (KontByIdTb.Rows.Count == 0)
                {
                    InsertNewKontragentInTable(eikText, kontText, ddsNumberText);
                    KontByIdTb.Clear();
                    sqlDataAdapter.Fill(KontByIdTb);
                }
                return KontByIdTb.Rows[0].Field<int>("Id");
            }
        }
        public static void InsertNewKontragentInTable(string kontText, string eikText, string ddsNumberText)
        {
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                sqlConnection.Open();
                sqlCommand = new SqlCommand("INSERT into Kontragenti (Name, EIK, DDSNumber) VALUES (@KontragentName, @EIK, @DDSNumber)", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@KontragentName", kontText);
                sqlCommand.Parameters.AddWithValue("@EIK", eikText);
                sqlCommand.Parameters.AddWithValue("@DDSNumber", ddsNumberText);
                sqlCommand.ExecuteNonQuery();
            }
        }
        public static void UpdateInvoiceDataFields(int InvoiceId, string kontText, string eikText, string ddsNumberText, string docDateText, string docNumberText, string doText, string ddsText, string fullValueText, int? dealKindId, int? docTypeId, string AccNumText, string inCashAccountText, string noteText)
        {
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                sqlConnection.Open();
                sqlCommand = new SqlCommand($"select * from Fakturi where Fakturi.Id = {InvoiceId} ", sqlConnection);
                sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                DataTable FakturiUpdateDt = new DataTable("Fakturi");
                sqlDataAdapter.Fill(FakturiUpdateDt);

                /* 0 - Id
                   1 - DocPayableReceivableId
                   2 - KontragentiId
                   3 - AccDate
                   4 - Date
                   5 - Number
                   6 - DO
                   7 - DDS
                   8 - FullValue
                   9 - DealKindId
                   10 - DocTypeId
                   11 - Account
                   12 - InCashAccount
                   13 - Note
                   14 - Image
                   15 - AccountingStatusId */

                FakturiUpdateDt.Rows[0][2] = GetKontragentIdAndUpdateTable(kontText, eikText, ddsNumberText);
                FakturiUpdateDt.Rows[0][4] = Convert.ToDateTime(docDateText);
                FakturiUpdateDt.Rows[0][5] = Convert.ToInt64(docNumberText);
                FakturiUpdateDt.Rows[0][6] = Convert.ToDecimal(doText.Replace(".", ","));
                FakturiUpdateDt.Rows[0][7] = Convert.ToDecimal(ddsText.Replace(".", ","));
                FakturiUpdateDt.Rows[0][8] = Convert.ToDecimal(fullValueText.Replace(".", ","));
                FakturiUpdateDt.Rows[0][15] = 3;
                FakturiUpdateDt.Rows[0][3] = Convert.ToDateTime(docDateText);
                ///////////////////////////////////////////////////////////////////////////////// HARD CODED / NEEDS LOGIC 
                FakturiUpdateDt.Rows[0][9] = dealKindId;
                FakturiUpdateDt.Rows[0][10] = docTypeId;
                FakturiUpdateDt.Rows[0][11] = Convert.ToInt32(AccNumText);
                FakturiUpdateDt.Rows[0][12] = Convert.ToInt32(inCashAccountText);
                FakturiUpdateDt.Rows[0][13] = noteText;

                SqlCommandBuilder builder = new SqlCommandBuilder(sqlDataAdapter);
                sqlDataAdapter.UpdateCommand = builder.GetUpdateCommand();
                sqlDataAdapter.Update(FakturiUpdateDt);
            }
        }
    }
}
