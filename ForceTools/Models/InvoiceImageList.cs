using System.Collections.ObjectModel;


namespace ForceTools.Models
{

    public class InvoiceImageList
    {

        private ObservableCollection<InvoiceImage> _listOfImages;

        public virtual ObservableCollection<InvoiceImage> ListOfImages
        {
            get { return _listOfImages; }
            set 
            {
                _listOfImages = value;
                
            }
        }
        public InvoiceImageList()
        {
            ListOfImages = new ObservableCollection<InvoiceImage>();
        }

        public InvoiceImageList(InvoiceImage invoiceImage) : this()
        {
            ListOfImages.Add(invoiceImage);
        }

        public void Add(InvoiceImage ic) 
        {
            ListOfImages.Add(ic);
        }
    }
}
