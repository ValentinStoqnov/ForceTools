using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ForceTools
{
    public static class InvoiceExtractionDataRetriever
    {
        private static SqlConnection sqlConnection;
        private static SqlCommand sqlCommand;
        private static SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
        private static DataTable operationsDatatable = new DataTable();

        public static DataTable GetDataTableForExcelFullExport() 
        {
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                operationsDatatable.Clear();
                sqlConnection.Open();
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandText = "Select DocumentTypes.TypeName as 'Тип Документ', Fakturi.Number as Номер, Fakturi.Date as Дата, Kontragenti.Name as Контрагент, Kontragenti.EIK as ЕИК, Kontragenti.DDSNumber as  'ДДС Номер' , Fakturi.DO as 'Данъчна основа' , Fakturi.DDS as ДДС , Fakturi.FullValue as Общо ,Fakturi.InCashAccount as 'В брой', Fakturi.Account as Сметка, Fakturi.Note as Бележка, DocumentTypes.Id as 'Код Тип Документ', KindOfDeals.Id as 'Код Тип Сделка', AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id JOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE AccountingStatusId = 3";
                sqlDataAdapter.Fill(operationsDatatable);
            }
            sqlConnection.Close();
            return operationsDatatable;
        }
    }
}
