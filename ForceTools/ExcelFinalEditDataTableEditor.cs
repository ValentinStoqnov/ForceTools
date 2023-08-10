using System;
using System.Data;

namespace ForceTools
{
    public class ExcelFinalEditDataTableEditor
    {
        private int SelectedPosition;
        private int InsertPosition;
        private DataTable finalEditDataTable; 

        public ExcelFinalEditDataTableEditor(int selectedPosition, int insertPosition, DataTable FinalEditDataTable)
        {
            SelectedPosition = selectedPosition;
            InsertPosition = insertPosition;
            finalEditDataTable = FinalEditDataTable;
        }
        public Invoice GetInvoiceForSplitting()
        {
            DataRow DataRowToSplit = finalEditDataTable.DefaultView.ToTable().Rows[SelectedPosition];
            Invoice invoice = new Invoice();
            invoice.Number = Convert.ToInt64(DataRowToSplit["Number"].ToString());
            invoice.Date = Convert.ToDateTime(DataRowToSplit["Date"].ToString());
            invoice.DO = Convert.ToDecimal(DataRowToSplit["DO"].ToString());
            invoice.DDS = Convert.ToDecimal(DataRowToSplit["DDS"].ToString());
            invoice.FullValue = Convert.ToDecimal(DataRowToSplit["FullValue"].ToString());
            if(DataRowToSplit["InCashAccount"].ToString() != string.Empty) invoice.InCashAccount = Convert.ToInt32(DataRowToSplit["InCashAccount"].ToString());
            invoice.Account = Convert.ToInt32(DataRowToSplit["Account"].ToString());
            invoice.Note = DataRowToSplit["Note"].ToString();
            invoice.DealKindId = Convert.ToInt32(DataRowToSplit["DealKind"].ToString());
            invoice.DocTypeId = Convert.ToInt32(DataRowToSplit["DocType"].ToString());
            return invoice;
        }
        public Kontragent GetKontragentForSplitting()
        {
            Kontragent kontragent = new Kontragent();
            DataRow DataRowToSplit = finalEditDataTable.DefaultView.ToTable().Rows[SelectedPosition];
            kontragent.Name = DataRowToSplit["Name"].ToString();
            kontragent.EIK = DataRowToSplit["EIK"].ToString();
            kontragent.DdsNumber = DataRowToSplit["DDSNumber"].ToString();
            return kontragent;
        }
        public DataTable SplitRowsAndReturnTable(Invoice invoiceToBeSplit,Kontragent newKontragent,Invoice newInvoice)
        {
            DataRow NewDataRow = finalEditDataTable.NewRow();
            finalEditDataTable.Rows.InsertAt(NewDataRow, InsertPosition);
            UpdateSplitRowData(invoiceToBeSplit);
            UpdateNewRowData(newKontragent, newInvoice);
            return finalEditDataTable;
        }
        private void UpdateSplitRowData(Invoice invoiceToBeSplit) 
        {
            finalEditDataTable.Rows[SelectedPosition][5] = invoiceToBeSplit.DO;
            finalEditDataTable.Rows[SelectedPosition][6] = invoiceToBeSplit.DDS;
            finalEditDataTable.Rows[SelectedPosition][7] = invoiceToBeSplit.FullValue;
            finalEditDataTable.Rows[SelectedPosition][8] = invoiceToBeSplit.InCashAccount;
            finalEditDataTable.Rows[SelectedPosition][9] = invoiceToBeSplit.Account;
            finalEditDataTable.Rows[SelectedPosition][10] = invoiceToBeSplit.Note;
            finalEditDataTable.Rows[SelectedPosition][11] = invoiceToBeSplit.DocTypeId;
            finalEditDataTable.Rows[SelectedPosition][12] = invoiceToBeSplit.DealKindId;
        }
        private void UpdateNewRowData(Kontragent newKontragent, Invoice newInvoice)
        {
            finalEditDataTable.Rows[InsertPosition][0] = newInvoice.Date;
            finalEditDataTable.Rows[InsertPosition][1] = newInvoice.Number;
            finalEditDataTable.Rows[InsertPosition][2] = newKontragent.Name;
            finalEditDataTable.Rows[InsertPosition][3] = newKontragent.EIK;
            finalEditDataTable.Rows[InsertPosition][4] = newKontragent.DdsNumber;
            finalEditDataTable.Rows[InsertPosition][5] = newInvoice.DO;
            finalEditDataTable.Rows[InsertPosition][6] = newInvoice.DDS;
            finalEditDataTable.Rows[InsertPosition][7] = newInvoice.FullValue;
            finalEditDataTable.Rows[InsertPosition][8] = newInvoice.InCashAccount;
            finalEditDataTable.Rows[InsertPosition][9] = newInvoice.Account;
            finalEditDataTable.Rows[InsertPosition][10] = newInvoice.Note;
            finalEditDataTable.Rows[InsertPosition][11] = newInvoice.DocTypeId;
            finalEditDataTable.Rows[InsertPosition][12] = newInvoice.DealKindId;
        }
    }
}
