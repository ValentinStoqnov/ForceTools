using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ForceTools
{
    public static class InvoiceExtractionDataRetriever
    {
        private static SqlConnection sqlConnection;
        private static SqlCommand sqlCommand = new SqlCommand();
        private static SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
        private static DataTable operationsDatatable = new DataTable();

        public static DataTable GetDataTableForExcelFullExport(OperationType operationType) 
        {
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                operationsDatatable.Clear();
                sqlConnection.Open();
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandText = $"Select DocumentTypes.TypeName as 'Тип Документ', \r\nFakturi.Number as Номер, Fakturi.Date as Дата, \r\nKontragenti.Name as Контрагент, Kontragenti.EIK as ЕИК, \r\nKontragenti.DDSNumber as  'ДДС Номер' , Fakturi.DO as 'Данъчна основа' , \r\nFakturi.DDS as ДДС , Fakturi.FullValue as Общо ,Fakturi.InCashAccount as 'В брой',\r\nFakturi.Account as Сметка, Fakturi.Note as Бележка, DocumentTypes.Id as 'Код Тип Документ', \r\nKindOfDeals.Id as 'Код Тип Сделка', AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On \r\nFakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id \r\nJOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE AccountingStatusId = 3 and Fakturi.PurchaseOrSale = '{operationType.ToString()}'";
                sqlDataAdapter.Fill(operationsDatatable);
            }
            sqlConnection.Close();
            return operationsDatatable;
        }
    }
}
