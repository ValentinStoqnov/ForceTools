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

        public static int FindNextOrPreviousInvoiceIdByCurrentIdAndStatusAndOperationType(int startId, InvoiceLoadOperators invoiceLoadOperators, OperationType operationType, DocumentStatuses documentStatuses)
        {
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                DataTable CounterDataTable = new DataTable();
                int nextInvoiceId = 0;
                int StopCheck = 0;
                switch (invoiceLoadOperators)
                {
                    case InvoiceLoadOperators.LoadNext:
                        nextInvoiceId = startId + 1;
                        break;
                    case InvoiceLoadOperators.LoadPrevious:
                        nextInvoiceId = startId - 1;
                        break;
                }
                sqlCommand = new SqlCommand(GetNextSingleInvoiceIdFilterCommandByIdAndDocumentStatusAndOperationType(nextInvoiceId, documentStatuses, operationType), sqlConnection);
                sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(CounterDataTable);
                if (CounterDataTable.Rows.Count < 1)
                {
                    do
                    {
                        switch (invoiceLoadOperators)
                        {
                            case InvoiceLoadOperators.LoadNext:
                                nextInvoiceId++;
                                break;
                            case InvoiceLoadOperators.LoadPrevious:
                                nextInvoiceId--;
                                break;
                        }
                        StopCheck++;
                        sqlDataAdapter.Fill(CounterDataTable);

                    } while (CounterDataTable.Rows.Count < 1 && StopCheck <= 999);
                }
                if (StopCheck == 1000) return -1;
                return nextInvoiceId;
            }
        }
        public static DataTable GetInvoiceDataTableByStatusAndOperation(OperationType operationType, DocumentStatuses documentStatuses)
        {
            string filter = GetFilterCommandText(operationType, documentStatuses);
            return GetFilteredDataTable(filter);
        }
        public static DataTable GetInvoiceDataTableByStatusAndOperationAndSearchText(string searchText, OperationType operationType, DocumentStatuses documentStatuses)
        {
            string filter = GetFilterCommandBySearchText(searchText, operationType, documentStatuses);
            return GetFilteredDataTable(filter);
        }
        public static DataTable GetSingleInvoiceDataTableByInvoiceId(int invoiceId)
        {
            string filter = GetSingeInvoiceFilterCommandByInvoiceId(invoiceId);
            return GetFilteredDataTable(filter);
        }
        public static DataTable GetKontragentiFilteredDataTableBySearchText(string searchText) 
        {
            string filter = GetKontragentiFilterCommandBySearchText(searchText);
            return GetFilteredDataTable(filter);
        }
        public static DataTable GetDocumentTypeFilteredDataTableBySearchText(string searchText)
        {
            string filter = GetDocumentTypeFilterCommandBySearchText(searchText);
            return GetFilteredDataTable(filter);
        }
        public static DataTable GetDealKindFilteredDataTableBySearchText(string searchText)
        {
            string filter = GetDealKindFilterCommandBySearchText(searchText);
            return GetFilteredDataTable(filter);
        }
        public static DataTable GetAccountNumberFilteredDataTableBySearchText(string searchText)
        {
            string filter = GetAccountNumberFilterCommandBySearchText(searchText);
            return GetFilteredDataTable(filter);
        }

        private static DataTable GetFilteredDataTable(string filter) 
        {
            DataTable FilteredDataTable = new DataTable();
            using (sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
            {
                sqlCommand = new SqlCommand(filter, sqlConnection);
                sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(FilteredDataTable);
            }
            return FilteredDataTable;
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
        private static string GetSingeInvoiceFilterCommandByInvoiceId(int invoiceId)
        {
            string filterCommandText = $"Select Fakturi.Id, Fakturi.Number, Fakturi.AccDate, Fakturi.Date, " +
                    $"Kontragenti.Name, Kontragenti.EIK, Kontragenti.DDSNumber, Fakturi.DO, " +
                    $"Fakturi.DDS, Fakturi.FullValue, Fakturi.image, Fakturi.Account, Fakturi.InCashAccount, Fakturi.Note, DocumentTypes.TypeName, KindOfDeals.DealName, Fakturi.DealKindId, Fakturi.DocTypeId From Fakturi " +
                    $"LEFT JOIN Kontragenti On Fakturi.KontragentiId = Kontragenti.Id LEFT JOIN DocumentTypes on Fakturi.DocTypeId = DocumentTypes.Id LEFT JOIN KindOfDeals on Fakturi.DealKindId = KindOfDeals.Id WHERE Fakturi.Id = {invoiceId}";
            return filterCommandText;
        }
        private static string GetNextSingleInvoiceIdFilterCommandByIdAndDocumentStatusAndOperationType(int nextInvoiceId, DocumentStatuses documentStatuses, OperationType operationType)
        {
            int AccStatusChosen = (int)documentStatuses;
            if (AccStatusChosen == 0)
            {
                return $"select Fakturi.Id from Fakturi where Fakturi.Id = {nextInvoiceId} and PurchaseOrSale = '{operationType.ToString()}' ";
            }
            else
            {
                return $"select Fakturi.Id from Fakturi where Fakturi.Id = {nextInvoiceId} and (AccountingStatusId = {AccStatusChosen} and PurchaseOrSale = '{operationType.ToString()}') ";
            }
        }
        private static string GetKontragentiFilterCommandBySearchText(string searchText) 
        {
            return $"SELECT * FROM Kontragenti WHERE EIK like '%{searchText}%'";
        }
        private static string GetDocumentTypeFilterCommandBySearchText(string searchText) 
        {
            return $"SELECT * FROM DocumentTypes WHERE TypeName like N'%{searchText}%'";
        }
        private static string GetDealKindFilterCommandBySearchText(string searchText)
        {
            return $"SELECT * FROM KindOfDeals WHERE DealName like N'%{searchText}%' or Percentage like N'%{searchText}%'";
        }
        private static string GetAccountNumberFilterCommandBySearchText(string searchText)
        {
            return $"SELECT * FROM Accounts WHERE Account like N'%{searchText}%' or AccountName like N'%{searchText}%'";
        }

    }
}
