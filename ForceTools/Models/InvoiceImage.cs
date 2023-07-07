using ForceTools.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ForceTools.Models
{
    public class InvoiceImage
    {

        private BitmapImage _bitImage;
        private string _imagePath;
        
        public BitmapImage BitImage
        {
            get => _bitImage;
            set
            {
                _bitImage = value;
                
            }
        }
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                
            }

        }

        public InvoiceImage(BitmapImage bitmapImage, string imagePath)
        {
            BitImage = bitmapImage;
            ImagePath = imagePath;
        }

        public InvoiceImage()
        {
        }
    }
}
