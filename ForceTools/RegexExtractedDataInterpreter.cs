using Microsoft.SqlServer.Management.Dmf;
using System;
using System.Data;
using System.IO;

namespace ForceTools
{
    public class RegexExtractedDataInterpreter : IExtractedDataInterpreter
    {
        private readonly OperationType _operationType;
        public Kontragent Kontragent { get; set; }
        public Invoice Invoice { get; set; }
        public InvoiceDefaultValues DefaultValues { get; set; }

        public RegexExtractedDataInterpreter(OperationType operationType, string imageFilePath)
        {
            _operationType = operationType;
            RegexDataExtractor dataExtractor = new RegexDataExtractor();
            Invoice newInvoice = new Invoice();

            SetDefaultValuesFromSqlTable();
            Kontragent = InterperetKontragent(dataExtractor);
            newInvoice.Number = InterperetInvoiceNumber(dataExtractor);
            newInvoice.FullValue = InterperetFullValue(dataExtractor);
            newInvoice.Date = InterperetDocumentDate(dataExtractor);
            newInvoice.DO = InterperetDanuchnaOsnova(dataExtractor);
            newInvoice.DocTypeId = InterperetDocumentType(dataExtractor);
            newInvoice.DealKindId = GetDealKindId();
            newInvoice.Note = GetNote(operationType);
            newInvoice.ImageInBytes = GetImageFromBytes(imageFilePath);
            DoFinalConversions(newInvoice);
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
        private Kontragent InterperetKontragent(RegexDataExtractor dataExtractor)
        {
            DocumentSides sideWhereKontragentIsFound;
            Kontragent kontragent = new Kontragent();
            string KontragentName;
            string EIK;
            string DDS;
            //Logic determening if Kontragent is on the Right or Left side of the document.
            if (dataExtractor.KontragentSideExtract(_operationType, DocumentSides.RightSide) != false)
                sideWhereKontragentIsFound = DocumentSides.RightSide;
            else if (dataExtractor.KontragentSideExtract(_operationType, DocumentSides.LeftSide) != false)
                sideWhereKontragentIsFound = DocumentSides.LeftSide;
            else
            {
                //Returning empty if not found
                kontragent.Name = "";
                kontragent.EIK = "";
                kontragent.DdsNumber = "";
                return kontragent;
            }
            //Extracting Kontragent Name if Kontragent is found
            KontragentName = dataExtractor.KontragentNameExtract(_operationType);
            //Extracting EIK if Kontragent is found
            EIK = dataExtractor.EIKExtract(sideWhereKontragentIsFound, RegexExtractionMethod.One);
            if (EIK == string.Empty) EIK = dataExtractor.EIKExtract(sideWhereKontragentIsFound, RegexExtractionMethod.Two);
            //Extracting DDS if Kontragent is found
            bool isDDSFound = dataExtractor.DDSExtract(sideWhereKontragentIsFound);
            if (EIK != "" && EIK.ToString().Length < 10 && isDDSFound != false)
            {
                DDS = $"BG{EIK}";
            }
            else if (EIK != "")
            {
                DDS = $"{EIK}";
            }
            else
            {
                DDS = "";
            }
            kontragent.Name = KontragentName;
            kontragent.EIK = EIK;
            kontragent.DdsNumber = DDS;
            return kontragent;
        }
        private long InterperetInvoiceNumber(RegexDataExtractor dataExtractor)
        {
            string ExtractedInvoiceNumber;
            if (dataExtractor.InvoiceNumberExtract(RegexExtractionMethod.One) != string.Empty)
                ExtractedInvoiceNumber = dataExtractor.InvoiceNumberExtract(RegexExtractionMethod.One);
            else
                ExtractedInvoiceNumber = dataExtractor.InvoiceNumberExtract(RegexExtractionMethod.Two);
            long.TryParse(ExtractedInvoiceNumber, out long InvoiceNumber);
            return InvoiceNumber;
        }
        private decimal InterperetFullValue(RegexDataExtractor dataExtractor)
        {
            decimal.TryParse(dataExtractor.FullValueExtract(), out decimal FullValue);
            return FullValue;
        }
        private DateTime InterperetDocumentDate(RegexDataExtractor dataExtractor)
        {
            DateTime.TryParse(dataExtractor.DateExtract(), out DateTime dateTime);
            return dateTime;
        }
        private decimal InterperetDanuchnaOsnova(RegexDataExtractor dataExtractor)
        {
            decimal.TryParse(dataExtractor.DanOsnExtract(), out decimal DanuchnaOsnova);
            return DanuchnaOsnova;
        }
        private int InterperetDocumentType(RegexDataExtractor dataExtractor)
        {
            bool isFaktura = dataExtractor.DocTypeExtract().Item1;
            bool isKreditno = dataExtractor.DocTypeExtract().Item2;
            bool isToFaktura = dataExtractor.DocTypeExtract().Item3;
            int DocTypeId;
            if (isFaktura == true && isKreditno == false)
            {
                DocTypeId = 1;
                if (isToFaktura)
                    DocTypeId = 3;
            }
            else if (isFaktura == false && isKreditno == true)
            {
                DocTypeId = 3;
            }
            else if (isFaktura == true && isKreditno == true)
            {
                DocTypeId = 1;
                if (isToFaktura)
                    DocTypeId = 3;
            }
            else
            {
                DocTypeId = 1;
            }
            return DocTypeId;
        }
        private byte[] GetImageFromBytes(string imageFilePath)
        {
            #region Image to Byte Array 
            byte[] ImageInBytes = File.ReadAllBytes(imageFilePath);
            return ImageInBytes;
            #endregion
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
