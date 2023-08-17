using System.Data;
using Microsoft.Office.Interop.Excel;
using Microsoft.SqlServer.Management.HadrModel;
using DataTable = System.Data.DataTable;

namespace ForceTools
{
    public class ExcelReader
    {
        public DataTable GetDataTable(string filePath)
        {
            DataTable excelDataTable = new DataTable();
            Application ExcelApp = new Application();
            Workbook workbook = ExcelApp.Workbooks.Open(filePath);
            Worksheet worksheet = workbook.Worksheets[1];
            Range range = worksheet.UsedRange;

            object[,] valueArray = (object[,])range.get_Value(XlRangeValueDataType.xlRangeValueDefault);
            object[] singleValue = new object[valueArray.GetLength(1)];

            for (int i = 1; i <= range.Columns.Count; i++)
            {
                string columnName = (string)valueArray[1, i];
                columnName = columnName.Replace(".", "");
                columnName = columnName.Replace("/", "-");
                excelDataTable.Columns.Add(columnName);
            }

            for (int i = 2; i <= valueArray.GetLength(0); i++)
            {
                for (int j = 0; j < valueArray.GetLength(1); j++)
                {
                    if (valueArray[i, j + 1] != null)
                    {
                        singleValue[j] = valueArray[i, j + 1].ToString();
                    }
                    else
                    {
                        singleValue[j] = valueArray[i, j + 1];
                    }
                }
                excelDataTable.LoadDataRow(singleValue, LoadOption.PreserveChanges);
            }
            workbook.Close();
            ExcelApp.Quit();
            return excelDataTable;
        }
    }
}
