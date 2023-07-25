using System;

namespace ForceTools
{
    public interface IExtractedDataInterpreter
    {
        decimal DanOsn { get; set; }
        string DDSNumber { get; set; }
        int DealKindId { get; set; }
        int DocTypeId { get; set; }
        DateTime DocumentDate { get; set; }
        string EIK { get; set; }
        decimal FullValue { get; set; }
        byte[] ImageInBytes { get; set; }
        long InvoiceNumber { get; set; }
        string KontragentName { get; set; }
        string Note { get; set; }
        int DefaultPurchaseAccount { get; set; }
        int DefaultSaleAccount { get; set; }
        int DefaultCashRegAccount { get; set; }
        string DefaultNote { get; set; }
    }
}