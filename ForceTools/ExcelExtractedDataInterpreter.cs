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
        public Kontragent Kontragent { get; set; }
        public Invoice Invoice { get; set; }
        public InvoiceDefaultValues DefaultValues { get; set; }

        public ExcelExtractedDataInterpreter(OperationType operationType, int currentRow, List<ComboBox> comboBoxList, DataTable excelDataTable)
        {
            Kontragent newKontragent = new Kontragent();
            Invoice newInvoice = new Invoice();

            ExcelDataExtractor dataExtractor = new ExcelDataExtractor(currentRow, comboBoxList, excelDataTable);
            _operationType = operationType;
            SetDefaultValuesFromSqlTable();
            newKontragent.Name = dataExtractor.Kontragent;
            newKontragent.EIK = InterperetEik(dataExtractor);
            newKontragent.DdsNumber = InterperetDdsNumber(dataExtractor);
            newInvoice.Date = InterperetDocumentDate(dataExtractor);
            newInvoice.Number = InterperetInvoiceNumber(dataExtractor);
            newInvoice.FullValue = InterperetFullValue(dataExtractor);
            newInvoice.DO = InterperetDanuchnaOsnova(dataExtractor);
            newInvoice.DDS = InterpretDanukDobavenaStoinost(newInvoice);
            newInvoice.DocTypeId = InterperetDocumentType(dataExtractor);
            newInvoice.DealKindId = GetDealKindId();
            newInvoice.Note = GetNote(operationType);
            newInvoice.ImageInBytes = GetImageFromBytes();
            DoFinalConversions(newInvoice);

            Kontragent = newKontragent;
            Invoice = newInvoice;
        }
        public object[] GetInterpretedDataTable()
        {
            object[] invoiceData = new object[] { };
            return invoiceData;
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
            InvoiceDefaultValues defaultValues = new InvoiceDefaultValues();
            DataTable DefaultValuesTable = InvoiceDataFilters.GetDefaultValuesDataTable();
            defaultValues.DefaultPurchaseAccount = Convert.ToInt32(DefaultValuesTable.Rows[0][2]);
            defaultValues.DefaultSaleAccount = Convert.ToInt32(DefaultValuesTable.Rows[1][2]);
            defaultValues.DefaultCashRegAccount = Convert.ToInt32(DefaultValuesTable.Rows[2][2]);
            defaultValues.DefaultPurchaseNote = Convert.ToString(DefaultValuesTable.Rows[3][2]);
            defaultValues.DefaultSaleNote = Convert.ToString(DefaultValuesTable.Rows[4][2]);
            DefaultValues = defaultValues;
        }

        private long InterperetInvoiceNumber(ExcelDataExtractor dataExtractor)
        {
            long.TryParse(dataExtractor.DocumentNumber, out long invoiceNumber);
            return invoiceNumber;
        }
        private decimal InterperetFullValue(ExcelDataExtractor dataExtractor)
        {
            decimal.TryParse(dataExtractor.FullValue, out decimal fullValue);
            if (fullValue <= 0) fullValue = Convert.ToDecimal(dataExtractor.DanuchnaOsnova) + Convert.ToDecimal(dataExtractor.Dds);
            return fullValue;
        }
        private decimal InterpretDanukDobavenaStoinost(Invoice newInvoice)
        {
            return newInvoice.FullValue - newInvoice.DO;
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
            string extractedDocTypeString = dataExtractor.DocType.ToLower();
            if (extractedDocTypeString != string.Empty)
            {
                if (extractedDocTypeString.Contains("кредитно")) return 3;
            }
            if (Invoice.DO < 0 || Invoice.FullValue < 0) return 3;

            return 1;
        }
        private byte[] GetImageFromBytes()
        {
            byte[] ImageInBytes = File.ReadAllBytes(FileSystemHelper.ExcelPlaceHolderImage);
            return ImageInBytes;
        }
        private string GetNote(OperationType operationType)
        {
            string Note = "";
            if (Invoice.DocTypeId == 3)
            {
                Note = "КИ";
            }
            else
            {
                switch (operationType)
                {
                    case OperationType.Purchase:
                        Note = DefaultValues.DefaultPurchaseNote;
                        break;
                    case OperationType.Sale:
                        Note = DefaultValues.DefaultSaleNote;
                        break;
                }
            }
            return Note;
        }
        private int GetDealKindId()
        {
            int DealKindIdInt = 0;
            switch (_operationType)
            {
                case OperationType.Purchase:
                    if (Invoice.DO != 0 && Invoice.FullValue != 0)
                    {
                        if (Invoice.DO == Invoice.FullValue)
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
                    if (Invoice.DO != 0 && Invoice.FullValue != 0)
                    {
                        if (Invoice.DO == Invoice.FullValue)
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
        private void DoFinalConversions(Invoice newInvoice)
        {
            //Converting Do,DDS,FullValue values to -values if the DocType is Kreditno and extracted values are not -values.
            if (Invoice.DocTypeId == 3)
            {
                if (Invoice.DO > 0)
                {
                    newInvoice.DO = -newInvoice.DO;
                }
                if (Invoice.FullValue > 0)
                {
                    newInvoice.FullValue = -newInvoice.FullValue;
                }
            }
        }
    }
}
