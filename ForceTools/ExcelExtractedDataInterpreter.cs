using System;

namespace ForceTools
{
    public class ExcelExtractedDataInterpreter : IExtractedDataInterpreter
    {
        public decimal DanOsn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string DDSNumber { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int DealKindId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int DocTypeId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime DocumentDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string EIK { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public decimal FullValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public byte[] ImageInBytes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public long InvoiceNumber { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string KontragentName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Note { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int DefaultPurchaseAccount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int DefaultSaleAccount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int DefaultCashRegAccount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string DefaultNote { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ExcelExtractedDataInterpreter(OperationType operationType)
        {
            ExcelDataExtractor dataExtractor = new ExcelDataExtractor();
        }
    }
}
