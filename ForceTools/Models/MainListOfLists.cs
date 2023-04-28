using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ForceTools.Models
{
    public class MainListOfLists
    {
        private ObservableCollection<InvoiceImageList> _MainList;

        public virtual ObservableCollection<InvoiceImageList> MainList
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

        public void Add(InvoiceImageList icl)
        {
            MainList.Add(icl);
        }
    }
}
