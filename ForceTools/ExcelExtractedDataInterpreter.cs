using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Controls;

namespace ForceTools
{
    public class ExcelExtractedDataInterpreter : IExtractedDataInterpreter
    {
        private readonly OperationType _operationType;

        public int DefaultPurchaseAccount { get; set; }
        public int DefaultSaleAccount { get; set; }
        public int DefaultCashRegAccount { get; set; }
        public string DefaultNote { get; set; }

        public string KontragentName { get; set; }
        public string EIK { get; set; }
        public string DDSNumber { get; set; }
        public long InvoiceNumber { get; set; }
        public decimal FullValue { get; set; }
        public DateTime DocumentDate { get; set; }
        public decimal DanOsn { get; set; }
        public int DocTypeId { get; set; }
        public int DealKindId { get; set; }
        public string Note { get; set; }
        public byte[] ImageInBytes { get; set; }

        public ExcelExtractedDataInterpreter(OperationType operationType, int currentRow, List<ComboBox> comboBoxList, DataTable excelDataTable)
        {
            ExcelDataExtractor dataExtractor = new ExcelDataExtractor(currentRow, comboBoxList, excelDataTable);
            _operationType = operationType;
            SetDefaultValuesFromSqlTable();
            KontragentName = dataExtractor.Kontragent;
            EIK = InterperetEik(dataExtractor);
            DDSNumber = InterperetDdsNumber(dataExtractor);
            InvoiceNumber = InterperetInvoiceNumber(dataExtractor);
            FullValue = InterperetFullValue(dataExtractor);
            DocumentDate = InterperetDocumentDate(dataExtractor);
            DanOsn = InterperetDanuchnaOsnova(dataExtractor);
            DocTypeId = InterperetDocumentType(dataExtractor);
            DealKindId = GetDealKindId();
            Note = GetNote();
            ImageInBytes = GetImageFromBytes();
            DoFinalConversions();
        }
        private string InterperetDdsNumber(ExcelDataExtractor dataExtractor)
        {
            string DdsNumberString = dataExtractor.DdsNumber;
            if (DdsNumberString.Length > 11) DdsNumberString = DdsNumberString.Substring(0, 10);
            return DdsNumberString;
        }

        private string InterperetEik(ExcelDataExtractor dataExtractor)
        {
            string EikString = dataExtractor.Eik;
            if (EikString.Contains("BG")) EikString = EikString.Substring(2, 9);
            if (EikString.Length > 10) EikString = EikString.Substring(0, 10);
            return EikString;
        }

        private void SetDefaultValuesFromSqlTable()
        {
            DataTable DefaultValuesTable = InvoiceDataFilters.GetDefaultValuesDataTable();
            DefaultPurchaseAccount = Convert.ToInt32(DefaultValuesTable.Rows[0][2]);
            DefaultSaleAccount = Convert.ToInt32(DefaultValuesTable.Rows[1][2]);
            DefaultCashRegAccount = Convert.ToInt32(DefaultValuesTable.Rows[2][2]);
            DefaultNote = Convert.ToString(DefaultValuesTable.Rows[3][2]);
        }

        private long InterperetInvoiceNumber(ExcelDataExtractor dataExtractor)
        {
            long.TryParse(dataExtractor.DocumentNumber, out long invoiceNumber);
            return invoiceNumber;
        }
        private decimal InterperetFullValue(ExcelDataExtractor dataExtractor)
        {
            decimal.TryParse(dataExtractor.FullValue, out decimal fullValue);
            if(fullValue <= 0) fullValue = Convert.ToDecimal(dataExtractor.DanuchnaOsnova) + Convert.ToDecimal(dataExtractor.Dds.Replace(".",","));
            return fullValue;
        }
        private DateTime InterperetDocumentDate(ExcelDataExtractor dataExtractor)
        {
            DateTime.TryParse(dataExtractor.Date, out DateTime dateTime);
            return dateTime;
        }
        private decimal InterperetDanuchnaOsnova(ExcelDataExtractor dataExtractor)
        {
            decimal.TryParse(dataExtractor.DanuchnaOsnova, out decimal danuchnaOsnova);
            if (danuchnaOsnova == 0 && Convert.ToDecimal(dataExtractor.FullValue) > 0 && Convert.ToDecimal(dataExtractor.Dds) > 0)
                danuchnaOsnova = Convert.ToDecimal(dataExtractor.FullValue) - Convert.ToDecimal(dataExtractor.Dds);
            return danuchnaOsnova;
        }
        private int InterperetDocumentType(ExcelDataExtractor dataExtractor)
        {
            // THIS NEEDS LOGIC
            int docTypeId = 1;
            return docTypeId;
        }
        private byte[] GetImageFromBytes()
        {
            byte[] ImageInBytes = File.ReadAllBytes(FileSystemHelper.ExcelPlaceHolderImage);
            return ImageInBytes;
        }
        private string GetNote()
        {
            string Note;
            if (DocTypeId == 3)
            {
                Note = "КИ";
            }
            else
            {
                Note = DefaultNote;
            }
            return Note;
        }
        private int GetDealKindId()
        {
            int DealKindIdInt = 0;
            switch (_operationType)
            {
                case OperationType.Purchase:
                    if (DanOsn != 0 && FullValue != 0)
                    {
                        if (DanOsn == FullValue)
                        {
                            DealKindIdInt = 12; /////////////////////////CHANGE THIS 
                        }
                        else
                        {
                            DealKindIdInt = 12;
                        }
                    }
                    break;
                case OperationType.Sale:
                    if (DanOsn != 0 && FullValue != 0)
                    {
                        if (DanOsn == FullValue)
                        {
                            DealKindIdInt = 25;
                        }
                        else
                        {
                            DealKindIdInt = 21;
                        }
                    }
                    break;
            }
            return DealKindIdInt;
        }
        private void DoFinalConversions()
        {
            //Converting Do,DDS,FullValue values to -values if the DocType is Kreditno and extracted values are not -values.
            if (DocTypeId == 3)
            {
                if (DanOsn > 0)
                {
                    DanOsn = -DanOsn;
                }
                if (FullValue > 0)
                {
                    FullValue = -FullValue;
                }
            }
        }
    }
}
