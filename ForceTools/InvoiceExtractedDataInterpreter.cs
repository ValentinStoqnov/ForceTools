using System;
using System.Data;
using System.IO;

namespace ForceTools
{
    public class InvoiceExtractedDataInterpreter
    {
        private OperationType _operationType;
        private Kontragent _kontragent;

        public int DefaultPurchaseAccount;
        public int DefaultSaleAccount;
        public int DefaultCashRegAccount;
        public string DefaultNote;

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

        public InvoiceExtractedDataInterpreter(OperationType operationType, string imageFilePath)
        {
            RegexDataExtractor DataExtractor = new RegexDataExtractor();
            _operationType = operationType;
            SetDefaultValuesFromSqlTable();
            _kontragent = InterperetKontragent(DataExtractor);
            KontragentName = _kontragent.Name;
            EIK = _kontragent.EIK;
            DDSNumber = _kontragent.DdsNumber;
            InvoiceNumber = InterperetInvoiceNumber(DataExtractor);
            FullValue = InterperetFullValue(DataExtractor);
            DocumentDate = InterperetDocumentDate(DataExtractor);
            DanOsn = InterperetDanuchnaOsnova(DataExtractor);
            DocTypeId = InterperetDocumentType(DataExtractor);
            DealKindId = GetDealKindId();
            Note = GetNote();
            ImageInBytes = GetImageFromBytes(imageFilePath);
            DoFinalConversions();
        }
        private void SetDefaultValuesFromSqlTable() 
        {
            DataTable DefaultValuesTable = InvoiceDataFilters.GetDefaultValuesDataTable(); 
            DefaultPurchaseAccount = Convert.ToInt32(DefaultValuesTable.Rows[0][2]);
            DefaultSaleAccount = Convert.ToInt32(DefaultValuesTable.Rows[1][2]);
            DefaultCashRegAccount = Convert.ToInt32(DefaultValuesTable.Rows[2][2]);
            DefaultNote = Convert.ToString(DefaultValuesTable.Rows[3][2]);
        }
        private Kontragent InterperetKontragent(RegexDataExtractor dataExtractor)
        {
            DocumentSides sideWhereKontragentIsFound;
            Kontragent kontragent;
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
            long InvoiceNumber;
            string ExtractedInvoiceNumber;
            if (dataExtractor.InvoiceNumberExtract(RegexExtractionMethod.One) != string.Empty)
                ExtractedInvoiceNumber = dataExtractor.InvoiceNumberExtract(RegexExtractionMethod.One);
            else
                ExtractedInvoiceNumber = dataExtractor.InvoiceNumberExtract(RegexExtractionMethod.Two);
            long.TryParse(ExtractedInvoiceNumber, out InvoiceNumber);
            return InvoiceNumber;
        }
        private decimal InterperetFullValue(RegexDataExtractor dataExtractor)
        {
            decimal FullValue;
            decimal.TryParse(dataExtractor.FullValueExtract(), out FullValue);
            return FullValue;
        }
        private DateTime InterperetDocumentDate(RegexDataExtractor dataExtractor)
        {
            DateTime dateTime;
            DateTime.TryParse(dataExtractor.DateExtract(), out dateTime);
            return dateTime;
        }
        private decimal InterperetDanuchnaOsnova(RegexDataExtractor dataExtractor)
        {
            decimal DanuchnaOsnova;
            decimal.TryParse(dataExtractor.DanOsnExtract(), out DanuchnaOsnova);
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
        private string GetNote()
        {
            string Note = "";
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
