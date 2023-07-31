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
        public string DanuchnaOsnova { get; set; }
        public string Dds { get; set; }
        public string FullValue { get; set; }

        public ExcelDataExtractor(int currentRow, List<ComboBox> comboBoxList, DataTable excelDataTable)
        {
            GetRowData(currentRow, comboBoxList, excelDataTable);
        }

        private void GetRowData(int currentRow, List<ComboBox> comboBoxList, DataTable excelDataTable)
        {
            for (int i = 0; i < comboBoxList.Count; i++)
            {
                string dataFromRow = excelDataTable.Rows[currentRow][i].ToString(); //[]Row[]Column
                if (comboBoxList[i].SelectedIndex == 1) DocumentNumber = dataFromRow;
                if (comboBoxList[i].SelectedIndex == 2) Date = dataFromRow;
                if (comboBoxList[i].SelectedIndex == 3) Kontragent = dataFromRow;
                if (comboBoxList[i].SelectedIndex == 4) Eik = dataFromRow;
                if (comboBoxList[i].SelectedIndex == 5) DdsNumber = dataFromRow;
                if (comboBoxList[i].SelectedIndex == 6) DanuchnaOsnova = dataFromRow.Replace(".",",");
                if (comboBoxList[i].SelectedIndex == 7) Dds = dataFromRow.Replace(".", ",");
                if (comboBoxList[i].SelectedIndex == 8) FullValue = dataFromRow.Replace(".", ",");
                if (comboBoxList[i].SelectedIndex == 9) { Eik = dataFromRow; DdsNumber = dataFromRow; } 
            }
        }
    }
}
        
