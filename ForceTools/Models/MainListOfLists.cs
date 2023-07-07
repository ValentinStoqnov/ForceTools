using System.Collections.ObjectModel;

namespace ForceTools.Models
{
    public class MainListOfLists : ObservableCollection<InvoiceImageList>
    {
        private ObservableCollection<InvoiceImageList> _MainList;

        public ObservableCollection<InvoiceImageList> MainList
        {
            get { return _MainList; }
            set
            {
                _MainList = value;
            }
        }
        public MainListOfLists()
        {
            MainList = new ObservableCollection<InvoiceImageList>();
        }
    }
}
