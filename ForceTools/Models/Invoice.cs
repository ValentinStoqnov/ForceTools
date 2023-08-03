using System;

namespace ForceTools
{
    public struct Invoice
    {
        public int KontragentId;
        public DateTime Date;
        public DateTime AccDate;
        public long Number;
        public decimal DO;
        public decimal DDS;
        public decimal FullValue;
        public int AccountingStatusId;
        public byte[] ImageInBytes;
        public int DealKindId;
        public int DocTypeId;
        public int InCashAccount;
        public int Account;
        public string Note;
        public string PurchaseOrSale; 
    }
}
