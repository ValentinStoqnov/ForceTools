using Microsoft.Office.Interop.Excel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System;
using DataTable = System.Data.DataTable;

namespace ForceTools
{
    public static class ExcelExporter
    {
        public static void ExtractFromDataTable(DataTable dataToExtract) 
        {
            // Opening Excel and Filling WorkSheet from datatable
            var ExcelApplication = new Application();
            ExcelApplication.Visible = true;
            ExcelApplication.Workbooks.Add();
            Worksheet workSheet = (Worksheet)ExcelApplication.ActiveSheet;
            workSheet.Name = "Sheet1";

            try
            {
                if (dataToExtract == null || dataToExtract.Columns.Count == 0)
                    throw new Exception("ExportToExcel: Null or empty input table!\n");


                // Getting and filling columns
                for (var i = 0; i < dataToExtract.Columns.Count; i++)
                {
                    workSheet.Cells[1, i + 1] = dataToExtract.Columns[i].ColumnName;
                }

                // Getting and filling rows
                for (var i = 0; i < dataToExtract.Rows.Count; i++)
                {

                    for (var j = 0; j < dataToExtract.Columns.Count; j++)
                    {
                        //Filling Excel rows with raw data from datatable if its not Data column
                        if (j != 2)
                        {
                            workSheet.Cells[i + 2, j + 1] = dataToExtract.Rows[i][j];
                        }
                        //Filling with Converted Data if its Data column
                        else
                        {
                            workSheet.Cells[i + 2, j + 1] = dataToExtract.Rows[i].Field<DateTime>("Дата").ToString("dd,MM,yyyy", CultureInfo.InvariantCulture).Replace(",", ".");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ExportToExcel: \n" + ex.Message);
            }
            finally
            {
                // Formating Columns
                for (var i = 1; i < dataToExtract.Columns.Count + 1; i++)
                {
                    ((Range)workSheet.Columns[i]).ColumnWidth = 12;
                    ExcelApplication.Cells[1, i].Interior.Color = ColorTranslator.FromHtml("#0078D4");
                    ExcelApplication.Cells[1, i].Font.Color = XlRgbColor.rgbWhite;
                }
                ((Range)workSheet.Columns[3]).AutoFit();
                ((Range)workSheet.Columns[9]).AutoFit();
                dataToExtract.Clear();
            }
        }
    }
}
