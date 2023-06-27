using System;
using System.Data;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace ForceTools
{
    public class InvoiceDataReader
    {
        private DataTable _dataTable;

        public InvoiceDataReader(int invoiceId)
        {
            _dataTable = InvoiceDataFilters.GetSingleInvoiceDataTableByInvoiceId(invoiceId);
        }

        public string GetKontragentNameField()
        {
            if (_dataTable.Rows[0].Field<string>("Name") != null)
            {
                return _dataTable.Rows[0].Field<string>("Name").ToString();
            }
            else
            {
                return "";
            }
        }
        public string GetEikField()
        {
            if (_dataTable.Rows[0].Field<string>("EIK") != null)
            {
                return _dataTable.Rows[0].Field<string>("EIK").ToString();
            }
            else
            {
                return "";
            }
        }
        public string GetDdsNumberField()
        {
            if (_dataTable.Rows[0].Field<string>("DDSNumber") != null)
            {
                return _dataTable.Rows[0].Field<string>("DDSNumber").ToString();
            }
            else
            {
                return "";
            }
        }
        public string GetDocDateField()
        {
            if (_dataTable.Rows[0].Field<DateTime?>("Date") != null)
            {
                return _dataTable.Rows[0].Field<DateTime>("Date").ToString("dd,MM,yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                return "";
            }
        }
        public string GetDocNumberField()
        {
            if (_dataTable.Rows[0].Field<Int64?>("Number") != null)
            {
                return _dataTable.Rows[0].Field<Int64?>("Number").ToString();
            }
            else
            {
                return "";
            }
        }
        public string GetDoField()
        {
            if (_dataTable.Rows[0].Field<decimal?>("DO") != null)
            {
                return _dataTable.Rows[0].Field<decimal?>("DO").ToString();
            }
            else
            {
                return "";
            }
        }
        public string GetDdsField()
        {
            if (_dataTable.Rows[0].Field<decimal?>("DDS") != null)
            {
                return _dataTable.Rows[0].Field<decimal?>("DDS").ToString();
            }
            else
            {
                return "";
            }
        }
        public string GetFullValueField()
        {
            if (_dataTable.Rows[0].Field<decimal?>("FullValue") != null)
            {
                return _dataTable.Rows[0].Field<decimal>("FullValue").ToString();
            }
            else
            {
                return "";
            }
        }
        public string GetAccDateField()
        {
            if (_dataTable.Rows[0].Field<DateTime?>("AccDate") != null)
            {
                return _dataTable.Rows[0].Field<DateTime>("AccDate").ToString("dd,MM,yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                return "";
            }
        }
        public string GetDealKindField()
        {
            if (_dataTable.Rows[0].Field<string>("DealName") != null)
            {
                return _dataTable.Rows[0].Field<string>("DealName").ToString();
            }
            else
            {
                return string.Empty;
            }
        }
        public string GetDocTypeField() 
        {
            if (_dataTable.Rows[0].Field<string>("TypeName") != null)
            {
                return _dataTable.Rows[0].Field<string>("TypeName").ToString();
            }
            else
            {
                return "";
            }
        }
        public string GetAccountNumberField() 
        {
            if (_dataTable.Rows[0].Field<int?>("Account") != null)
            {
                return _dataTable.Rows[0].Field<int>("Account").ToString();
            }
            else
            {
                return "";
            }
        }
        public string GetInCashAccountField() 
        {
            if (_dataTable.Rows[0].Field<int?>("InCashAccount") != null)
            {
                return _dataTable.Rows[0].Field<int>("InCashAccount").ToString();
            }
            else
            {
                return "";
            }
        }
        public string GetNoteField() 
        {
            if (_dataTable.Rows[0].Field<string>("Note") != null)
            {
                return _dataTable.Rows[0].Field<string>("Note").ToString();
            }
            else
            {
                return "";
            }
        }
        public BitmapImage GetInvoiceImage() 
        {
            if (_dataTable.Rows[0].Field<Byte[]>("image") != null)
            {
                using (var ms = new System.IO.MemoryStream(_dataTable.Rows[0].Field<Byte[]>("image")))
                {
                    return BitmapCreator.InvoiceImageFromMemoryStream(ms);
                }
            }
            else
            {
                return null;
            }
        }
        public int? GetDealKindId() 
        {
            if (_dataTable.Rows[0].Field<int?>("DealKindId") != null)
            {
                return _dataTable.Rows[0].Field<int?>("DealKindId");
            }
            else
            {
                return null;
            }
        }
        public int? GetDocTypeId() 
        {
            if (_dataTable.Rows[0].Field<int?>("DocTypeId") != null)
            {
                return _dataTable.Rows[0].Field<int?>("DocTypeId");
            }
            else
            {
                return null;
            }
        }
    }
}
