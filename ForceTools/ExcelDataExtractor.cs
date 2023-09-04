using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Controls;

namespace ForceTools
{
    public class ExcelDataExtractor
    {
        public string DocumentNumber { get; set; }
        public string Date { get; set; }
        public string Kontragent { get; set; }
        public string Eik { get; set; }
        public string DdsNumber { get; set; }
        public List<string> DanuchnaOsnovaList { get; set; } = new List<string>();
        public List<string> DdsList { get; set; } = new List<string>();
        public string FullValue { get; set; }
        public string DocType { get; set; }
        public string InCashAccount { get; set; }

        public Kontragent FullKontragent { get; set; }
        public Invoice FullInvoice { get; set; }

        public ExcelDataExtractor(int currentRow, List<ComboBox> comboBoxList, DataTable excelDataTable)
        {
            GetRowDataFromUnsortedDataTable(currentRow, comboBoxList, excelDataTable);
        }
        public ExcelDataExtractor(int currentRow, DataTable finalEditDataTable)
        {
            FullKontragent = GetKontragentFromSortedDataTable(currentRow,finalEditDataTable);
            FullInvoice = GetInvoiceFromSortedDataTable(currentRow, finalEditDataTable);
        }

        private void GetRowDataFromUnsortedDataTable(int currentRow, List<ComboBox> comboBoxList, DataTable excelDataTable)
        {
            for (int i = 0; i < comboBoxList.Count; i++)
            {
                string dataFromRow = excelDataTable.Rows[currentRow][i].ToString(); //[]Row[]Column
                if (comboBoxList[i].SelectedIndex == 1) DocumentNumber = dataFromRow;
                if (comboBoxList[i].SelectedIndex == 2) Date = dataFromRow;
                if (comboBoxList[i].SelectedIndex == 3) Kontragent = dataFromRow;
                if (comboBoxList[i].SelectedIndex == 4) Eik = dataFromRow;
                if (comboBoxList[i].SelectedIndex == 5) DdsNumber = dataFromRow;
                if (comboBoxList[i].SelectedIndex == 6) DanuchnaOsnovaList.Add(dataFromRow.Replace(",", "."));
                if (comboBoxList[i].SelectedIndex == 7) DdsList.Add(dataFromRow.Replace(",", "."));
                if (comboBoxList[i].SelectedIndex == 8) FullValue = dataFromRow.Replace(",", ".");
                if (comboBoxList[i].SelectedIndex == 9) { Eik = dataFromRow; DdsNumber = dataFromRow; }
                if (comboBoxList[i].SelectedIndex == 10) DocType = dataFromRow;
                if (comboBoxList[i].SelectedIndex == 11) InCashAccount = dataFromRow.ToLower();
            }
        }
        private Invoice GetInvoiceFromSortedDataTable(int currentRow, DataTable dataTable)
        {
            Invoice newInvoice = new Invoice();
            newInvoice.Date = Convert.ToDateTime(dataTable.Rows[currentRow][0]);
            newInvoice.Number = Convert.ToInt64(dataTable.Rows[currentRow][1]);
            newInvoice.DO = Convert.ToDecimal(dataTable.Rows[currentRow][5]);
            newInvoice.DDS = Convert.ToDecimal(dataTable.Rows[currentRow][6]);
            newInvoice.FullValue = Convert.ToDecimal(dataTable.Rows[currentRow][7]);
            newInvoice.InCashAccount = Convert.ToInt32(dataTable.Rows[currentRow][8]);
            newInvoice.Account = Convert.ToInt32(dataTable.Rows[currentRow][9]);
            newInvoice.Note = Convert.ToString(dataTable.Rows[currentRow][10]);
            newInvoice.DocTypeId = Convert.ToInt32(dataTable.Rows[currentRow][11]);
            newInvoice.DealKindId = Convert.ToInt32(dataTable.Rows[currentRow][12]);
            return newInvoice;
        }
        private Kontragent GetKontragentFromSortedDataTable(int currentRow, DataTable dataTable)
        {
            Kontragent newKontragent = new Kontragent();
            newKontragent.Name = dataTable.Rows[currentRow][2].ToString();
            newKontragent.EIK= dataTable.Rows[currentRow][3].ToString();
            newKontragent.DdsNumber = dataTable.Rows[currentRow][4].ToString();
            return newKontragent;
        }
    }
}

