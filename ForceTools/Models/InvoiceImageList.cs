using ForceTools.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

using System.Runtime.CompilerServices;


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

        public void Add(InvoiceImage ic) 
        {
            ListOfImages.Add(ic);
        }
    }
}
