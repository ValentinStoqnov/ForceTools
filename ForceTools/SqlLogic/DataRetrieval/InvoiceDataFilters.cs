using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ForceTools
{
    public static class InvoiceDataFilters
    {
        private static SqlConnection sqlConnection;
        private static SqlCommand sqlCommand = new SqlCommand();
        private static SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
        private static DataTable filteredInvoicesDatatable = new DataTable();

        public static DataTable GetInvoiceDataTableByStatusAndOperation(OperationType operationType, DocumentStatuses documentStatuses)
        {
            filteredInvoicesDatatable.Clear();
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                sqlConnection.Open();
                sqlCommand.CommandText = GetFilterCommandText(operationType, documentStatuses);
                sqlCommand.Connection = sqlConnection;
                sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(filteredInvoicesDatatable);
            }
            return filteredInvoicesDatatable;
        }
        public static DataTable GetInvoiceDataTableByStatusAndOperationAndSearchText(string searchText, OperationType operationType, DocumentStatuses documentStatuses) 
        {
            filteredInvoicesDatatable.Clear();
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                sqlConnection.Open();
                sqlCommand.CommandText = GetFilterCommandBySearchText(searchText,operationType, documentStatuses);
                sqlCommand.Connection = sqlConnection;
                sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(filteredInvoicesDatatable);
            }
            return filteredInvoicesDatatable;
        }

        private static string GetFilterCommandText(OperationType operationType, DocumentStatuses documentStatuses)
        {
            string filterCommandText = "";

            if (documentStatuses == DocumentStatuses.AccountedDocuments)
            {
                filterCommandText = $"Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, Kontragenti.Name,Kontragenti.EIK, Kontragenti.DDSNumber,Fakturi.DO, Fakturi.DDS, Fakturi.FullValue, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id JOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE PurchaseOrSale = '{operationType.ToString()}'";
            }
            else
            {
                filterCommandText = $"Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, Kontragenti.Name,Kontragenti.EIK, Kontragenti.DDSNumber,Fakturi.DO, Fakturi.DDS, Fakturi.FullValue, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id JOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE (AccountingStatusId = {(int)documentStatuses} and PurchaseOrSale = '{operationType.ToString()}')";
            }
            return filterCommandText;
        }
        private static string GetFilterCommandBySearchText(string searchText, OperationType operationType, DocumentStatuses documentStatuses) 
        {
            string filterCommandText = "";

            if (documentStatuses == DocumentStatuses.AccountedDocuments)
            {
                filterCommandText = $"Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, Kontragenti.Name,Kontragenti.EIK, Kontragenti.DDSNumber,Fakturi.DO, Fakturi.DDS, Fakturi.FullValue, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id JOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE PurchaseOrSale = '{operationType.ToString()}' and (Number like N'%{searchText}%' Or AccDate like N'%{searchText}%' Or Date like N'%{searchText}%' Or Name like N'%{searchText}%' Or EIK like N'%{searchText}%' Or DDSNumber like N'%{searchText}%' Or DO like N'%{searchText}%' Or DDS like N'%{searchText}%' Or FullValue like N'%{searchText}%' Or Account like N'%{searchText}%' Or InCashAccount like N'%{searchText}%' Or Note like N'%{searchText}%' Or TypeName like N'%{searchText}%' Or DealName like N'%{searchText}%')";
            }
            else
            {
                filterCommandText = $"Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, Kontragenti.Name,Kontragenti.EIK, Kontragenti.DDSNumber,Fakturi.DO, Fakturi.DDS, Fakturi.FullValue, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, AccountingStatuses.AccountingStatus From Fakturi LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id JOIN AccountingStatuses on Fakturi.AccountingStatusId = AccountingStatuses.Id WHERE (AccountingStatusId = {(int)documentStatuses} and PurchaseOrSale = '{operationType.ToString()}') and (Number like N'%{searchText}%' Or AccDate like N'%{searchText}%' Or Date like N'%{searchText}%' Or Name like N'%{searchText}%' Or EIK like N'%{searchText}%' Or DDSNumber like N'%{searchText}%' Or DO like N'%{searchText}%' Or DDS like N'%{searchText}%' Or FullValue like N'%{searchText}%' Or Account like N'%{searchText}%' Or InCashAccount like N'%{searchText}%' Or Note like N'%{searchText}%' Or TypeName like N'%{searchText}%' Or DealName like N'%{searchText}%')";
            }
            return filterCommandText;
        }
    }
}
