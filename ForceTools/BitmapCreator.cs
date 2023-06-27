using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace ForceTools
{
    public static class BitmapCreator
    {
        public static BitmapImage DefaultDatabaseBitmapImage() 
        {
            BitmapImage DefaultBitmap = new BitmapImage();
            DefaultBitmap.BeginInit();
            DefaultBitmap.UriSource = new Uri(FileSystemHelper.dbDefaultImg, UriKind.RelativeOrAbsolute);
            DefaultBitmap.EndInit();
            return DefaultBitmap;
        }
        public static BitmapImage SelectedDatabaseBitmapImage() 
        {
            BitmapImage DefaultBitmap = new BitmapImage();
            DefaultBitmap.BeginInit();
            DefaultBitmap.UriSource = new Uri(FileSystemHelper.dbSelectedImg, UriKind.RelativeOrAbsolute);
            DefaultBitmap.EndInit();
            return DefaultBitmap;
        }
        public static BitmapImage InvoiceImageFromMemoryStream(MemoryStream memoryStream) 
        {
            BitmapImage InvoiceImage = new BitmapImage();
            InvoiceImage.BeginInit();
            InvoiceImage.CacheOption = BitmapCacheOption.OnLoad;
            InvoiceImage.StreamSource = memoryStream;
            InvoiceImage.EndInit();
            return InvoiceImage;
        }
    }
}
