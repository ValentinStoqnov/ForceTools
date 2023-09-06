using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace ForceTools
{
    public class ExcelExtractedDataInterpreter : IExtractedDataInterpreter
    {
        private readonly OperationType _operationType;
        private Invoice newInvoice = new Invoice();

        public Kontragent Kontragent { get; set; }
        public Invoice Invoice { get; set; }
        public InvoiceDefaultValues DefaultValues { get; set; }

        public ExcelExtractedDataInterpreter(OperationType operationType, int currentRow, List<ComboBox> comboBoxList, DataTable excelDataTable)
        {
            _operationType = operationType;
            ExcelDataExtractor dataExtractor = new ExcelDataExtractor(currentRow, comboBoxList, excelDataTable);
            Kontragent newKontragent = KontragentEditor.GetKontragent(InterperetEik(dataExtractor));

            SetDefaultValuesFromSqlTable();
            if (newKontragent.EIK == null)
            {
                newKontragent.Name = dataExtractor.Kontragent;
                newKontragent.EIK = InterperetEik(dataExtractor);
                newKontragent.DdsNumber = InterperetDdsNumber(dataExtractor);
            }

            newInvoice.Date = InterperetDocumentDate(dataExtractor);
            newInvoice.Number = InterperetInvoiceNumber(dataExtractor);
            newInvoice.FullValue = InterperetFullValue(dataExtractor);
            newInvoice.DO = InterperetDanuchnaOsnova(dataExtractor);
            newInvoice.DDS = InterpretDanukDobavenaStoinost(dataExtractor);
            newInvoice.DocTypeId = InterperetDocumentType(dataExtractor);
            newInvoice.DealKindId = GetDealKindId();
            newInvoice.Note = GetNote(_operationType, newKontragent);
            newInvoice.InCashAccount = InterpreterInCashAccount(dataExtractor);
            newInvoice.Account = GetAccount(_operationType, newKontragent);
            newInvoice.ImageInBytes = GetImageFromBytes();
            DoFinalConversions();

            Kontragent = newKontragent;
            Invoice = newInvoice;
        }
        

        public ExcelExtractedDataInterpreter(OperationType operationType, int currentRow, DataTable dataTable)
        {
            _operationType = operationType;
            ExcelDataExtractor dataExtractor = new ExcelDataExtractor(currentRow, dataTable);
            Kontragent newKontragent = dataExtractor.FullKontragent;
            newInvoice = dataExtractor.FullInvoice;
            newInvoice.ImageInBytes = GetImageFromBytes();
            Kontragent = newKontragent;
            Invoice = newInvoice;
            CheckIfInvoiceHasErrors(Invoice);
        }

        private void CheckIfInvoiceHasErrors(Invoice newInvoice)
        {
            if (newInvoice.DO == 0) throw new ArgumentException();
            if (newInvoice.FullValue == 0) throw new ArgumentException();
            if (newInvoice.DealKindId == 0) throw new ArgumentException();
        }
        private int InterpreterInCashAccount(ExcelDataExtractor dataExtractor)
        {
            if (dataExtractor.InCashAccount == null) return DefaultValues.DefaultCashRegAccount;
            if (dataExtractor.InCashAccount.Contains("банк")) return 0;
            if (dataExtractor.InCashAccount.Contains("брой")) return DefaultValues.DefaultCashRegAccount;
            if (dataExtractor.InCashAccount.Contains("платежно")) return 0;
            if (dataExtractor.InCashAccount == "1") return DefaultValues.DefaultCashRegAccount;

            return DefaultValues.DefaultCashRegAccount;
        }
        public object[] GetInterpreterDataRow()
        {
            object[] invoiceData = new object[] {Invoice.Date, Invoice.Number, Kontragent.Name,Kontragent.EIK,Kontragent.DdsNumber,Invoice.DO,Invoice.DDS,
                Invoice.FullValue,Invoice.InCashAccount,Invoice.Account,Invoice.Note,Invoice.DocTypeId,Invoice.DealKindId};
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
            decimal.TryParse(dataExtractor.FullValue, NumberStyles.Number, CultureInfo.GetCultureInfo("bg-BG"), out decimal fullValue);
            if (fullValue == 0) fullValue = InterperetDanuchnaOsnova(dataExtractor) + InterpretDanukDobavenaStoinost(dataExtractor);
            fullValue = Math.Round(fullValue, 2);
            return fullValue;
        }
        private decimal InterpretDanukDobavenaStoinost(ExcelDataExtractor dataExtractor)
        {
            decimal dds = 0;
            foreach (string ddsString in dataExtractor.DdsList)
            {
                var DecimalParseBool = decimal.TryParse(ddsString, NumberStyles.Number, CultureInfo.GetCultureInfo("bg-BG"), out decimal ddsParsed);
                if (DecimalParseBool == true && ddsParsed != 0) dds = Math.Round(ddsParsed, 2);
            }
            if (dds == 0 && newInvoice.FullValue != 0 && newInvoice.DO != 0) dds = newInvoice.FullValue - newInvoice.DO;
            return dds;
        }
        private decimal InterperetDanuchnaOsnova(ExcelDataExtractor dataExtractor)
        {
            decimal danuchnaOsnova = 0;
            if (dataExtractor.DanuchnaOsnovaList.Count > 1)
            {
                foreach (string danuchnaOsnovaString in dataExtractor.DanuchnaOsnovaList)
                {
                    decimal.TryParse(danuchnaOsnovaString, NumberStyles.Number, CultureInfo.GetCultureInfo("bg-BG"), out decimal danuchnaOsnovaFromMultiColumns);
                    if (danuchnaOsnovaFromMultiColumns != 0) danuchnaOsnova = danuchnaOsnovaFromMultiColumns;
                }
            }
            else decimal.TryParse(dataExtractor.DanuchnaOsnovaList[0], NumberStyles.AllowDecimalPoint, CultureInfo.GetCultureInfo("bg-BG"), out danuchnaOsnova);

            if (danuchnaOsnova == 0 && Convert.ToDecimal(dataExtractor.FullValue, CultureInfo.GetCultureInfo("bg-BG")) > 0 && InterpretDanukDobavenaStoinost(dataExtractor) > 0)
                danuchnaOsnova = Convert.ToDecimal(dataExtractor.FullValue, CultureInfo.GetCultureInfo("bg-BG")) - InterpretDanukDobavenaStoinost(dataExtractor);
            if (danuchnaOsnova == 0 && Convert.ToDecimal(dataExtractor.FullValue, CultureInfo.GetCultureInfo("bg-BG")) < 0 && InterpretDanukDobavenaStoinost(dataExtractor) < 0)
                danuchnaOsnova = Convert.ToDecimal(dataExtractor.FullValue, CultureInfo.GetCultureInfo("bg-BG")) - InterpretDanukDobavenaStoinost(dataExtractor);
            danuchnaOsnova = Math.Round(danuchnaOsnova, 2);
            return danuchnaOsnova;
        }
        private DateTime InterperetDocumentDate(ExcelDataExtractor dataExtractor)
        {
            DateTime.TryParse(dataExtractor.Date, out DateTime dateTime);
            return dateTime;
        }
        private int InterperetDocumentType(ExcelDataExtractor dataExtractor)
        {
            string extractedDocTypeString = string.Empty;
            if (dataExtractor.DocType != null) extractedDocTypeString = dataExtractor.DocType.ToLower();
            if (extractedDocTypeString != string.Empty)
            {
                if (extractedDocTypeString.Contains("кредитно")) return 3;
            }
            if (newInvoice.DO < 0 || newInvoice.FullValue < 0) return 3;

            return 1;
        }
        private byte[] GetImageFromBytes()
        {
            byte[] ImageInBytes = File.ReadAllBytes(FileSystemHelper.ExcelPlaceHolderImage);
            return ImageInBytes;
        }
        private string GetNote(OperationType operationType, Kontragent newKontragent)
        {
            string Note = "";
            if (newInvoice.DocTypeId == 3) Note = "КИ";
            else
            {
                switch (operationType)
                {
                    case OperationType.Purchase:
                        if (newKontragent.LastPurchaseNote != null) Note = newKontragent.LastPurchaseNote;
                        else Note = DefaultValues.DefaultPurchaseNote;
                        break;
                    case OperationType.Sale:
                        if (newKontragent.LastSaleNote != null) Note = newKontragent.LastSaleNote;
                        else Note = DefaultValues.DefaultSaleNote;
                        break;
                }
            }
            return Note;
        }
        private int? GetAccount(OperationType operationType, Kontragent newKontragent)
        {
            int? Account = 0;
            switch (operationType)
            {
                case OperationType.Purchase:
                    if (newKontragent.LastPurchaseAccount != null) Account = newKontragent.LastPurchaseAccount;
                    else Account = DefaultValues.DefaultPurchaseAccount;
                    break;
                case OperationType.Sale:
                    if (newKontragent.LastSaleAccount != null) Account = newKontragent.LastSaleAccount;
                    else Account = DefaultValues.DefaultSaleAccount;
                    break;
            }
            return Account;
        }
        private int GetDealKindId()
        {
            int DealKindIdInt = 0;
            switch (_operationType)
            {
                case OperationType.Purchase:
                    if (newInvoice.DO != 0 && newInvoice.FullValue != 0)
                    {
                        if (newInvoice.DO == newInvoice.FullValue)
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
                    if (newInvoice.DO != 0 && newInvoice.FullValue != 0)
                    {
                        if (newInvoice.DO == newInvoice.FullValue)
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
            if (newInvoice.DocTypeId == 3)
            {
                if (newInvoice.DO > 0) newInvoice.DO = -newInvoice.DO;
                if (newInvoice.FullValue > 0) newInvoice.FullValue = -newInvoice.FullValue;
                if (newInvoice.DDS > 0) newInvoice.DDS = -newInvoice.DDS;
            }
            if (newInvoice.DDS != 0 && newInvoice.FullValue != 0)
            {
                if (newInvoice.FullValue - newInvoice.DDS != newInvoice.DO) newInvoice.DO = newInvoice.FullValue - newInvoice.DDS;
            }
        }
    }
}
