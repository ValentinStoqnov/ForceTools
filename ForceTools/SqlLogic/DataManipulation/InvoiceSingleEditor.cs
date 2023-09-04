using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Controls;

namespace ForceTools
{
    public class InvoiceSingleEditor
    {
        private static SqlConnection sqlConnection;
        private static SqlCommand sqlCommand;
        private static SqlDataAdapter sqlDataAdapter;

        public static void UpdateKontragentAndInvoiceDataFields(int InvoiceId, string kontText, string eikText, string ddsNumberText, string docDateText, string docNumberText, string doText, string ddsText, string fullValueText, int? dealKindId, int? docTypeId, string AccNumText, string inCashAccountText, string noteText, OperationType operationType)
        {
            Kontragent kontragent = KontragentEditor.GetOrCreateNewKontragent(kontText, eikText, ddsNumberText);
            if (kontragent.Name != kontText)
            {
                kontragent.Name = kontText;
                KontragentEditor.UpdateKontragentName(kontragent);
            }
            if (operationType == OperationType.Purchase)
            {
                kontragent.LastPurchaseAccount = Convert.ToInt32(AccNumText);
                kontragent.LastPurchaseNote = noteText;
            }
            else
            {
                kontragent.LastSaleAccount = Convert.ToInt32(AccNumText);
                kontragent.LastSaleNote = noteText;
            }
            KontragentEditor.UpdateKontragentAccAndNoteData(kontragent, operationType);
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

                FakturiUpdateDt.Rows[0][2] = kontragent.Id;
                FakturiUpdateDt.Rows[0][4] = Convert.ToDateTime(docDateText);
                FakturiUpdateDt.Rows[0][5] = Convert.ToInt64(docNumberText);
                FakturiUpdateDt.Rows[0][6] = Convert.ToDecimal(doText.Replace(",", "."));
                FakturiUpdateDt.Rows[0][7] = Convert.ToDecimal(ddsText.Replace(",", "."));
                FakturiUpdateDt.Rows[0][8] = Convert.ToDecimal(fullValueText.Replace(",", "."));
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
        public static void InsertNewInvoiceInSqlTableFromPdfUploader(OperationType operationType, string imageFilePath) 
        {
            RegexExtractedDataInterpreter interpreter = new RegexExtractedDataInterpreter(operationType, imageFilePath);
            InsertNewInvoiceInSqlTableFromMassUploaders(interpreter, operationType);
        }
        public static void InsertNewInvoiceInSqlTableFromExcelUploader(OperationType operationType, int currentRow, DataTable excelDataTable) 
        { 
            ExcelExtractedDataInterpreter interpreter = new ExcelExtractedDataInterpreter(operationType, currentRow,excelDataTable);
            InsertNewInvoiceInSqlTableFromMassUploaders(interpreter, operationType);
        }
        private static void InsertNewInvoiceInSqlTableFromMassUploaders<T>(T interpreter,OperationType operationType) where T : IExtractedDataInterpreter
        {
            Kontragent kontragent = KontragentEditor.GetOrCreateNewKontragent(interpreter.Kontragent.Name, interpreter.Kontragent.EIK, interpreter.Kontragent.DdsNumber);
            
            //Adding Data to Fakturi Table
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                sqlConnection.Open();
                sqlCommand = new SqlCommand("INSERT into Fakturi (KontragentiId, Date, Number, DO, DDS, FullValue,AccountingStatusId,image,AccDate, DealKindId, DocTypeId, Account, InCashAccount, Note, PurchaseOrSale) VALUES (@KontragentiId, @Date, @Number, @DO, @DDS, @FullValue,@AccountingStatusId,@image, @AccDate, @DealKindId, @DocTypeId, @Account, @InCashAccount, @Note, @PurchaseOrSale)", sqlConnection);

                sqlCommand.Parameters.AddWithValue("@KontragentiId", kontragent.Id);
                sqlCommand.Parameters.AddWithValue("@Date", interpreter.Invoice.Date);
                sqlCommand.Parameters.AddWithValue("@Number", interpreter.Invoice.Number);
                sqlCommand.Parameters.AddWithValue("@DO", interpreter.Invoice.DO);
                sqlCommand.Parameters.AddWithValue("@DDS", interpreter.Invoice.DDS);
                sqlCommand.Parameters.AddWithValue("@FullValue", interpreter.Invoice.FullValue);
                sqlCommand.Parameters.AddWithValue("@AccountingStatusId", 2);
                sqlCommand.Parameters.AddWithValue("@image", interpreter.Invoice.ImageInBytes);
                sqlCommand.Parameters.AddWithValue("@AccDate", interpreter.Invoice.Date);
                sqlCommand.Parameters.AddWithValue("@DealKindId", interpreter.Invoice.DealKindId);
                sqlCommand.Parameters.AddWithValue("@DocTypeId", interpreter.Invoice.DocTypeId);
                sqlCommand.Parameters.AddWithValue("@InCashAccount", interpreter.Invoice.InCashAccount);
                sqlCommand.Parameters.AddWithValue("@Account", interpreter.Invoice.Account);
                sqlCommand.Parameters.AddWithValue("@Note", interpreter.Invoice.Note);
                //Setting DocType Specific info 
                switch (operationType)
                {
                    case OperationType.Purchase:
                        sqlCommand.Parameters.AddWithValue("@PurchaseOrSale", "Purchase");
                        break;
                    case OperationType.Sale:
                        sqlCommand.Parameters.AddWithValue("@PurchaseOrSale", "Sale");
                        break;
                }
                sqlCommand.ExecuteNonQuery();
            }
        }
    }
}
