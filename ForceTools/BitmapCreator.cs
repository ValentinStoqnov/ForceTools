using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
        public static BitmapImage InvoiceImageFromFilePathLowQuality(string filePath) 
        {
            BitmapImage InvoiceImage = new BitmapImage();
            InvoiceImage.BeginInit();
            InvoiceImage.CacheOption = BitmapCacheOption.OnLoad;
            InvoiceImage.DecodePixelWidth = 400;
            InvoiceImage.UriSource = new Uri(filePath, UriKind.Absolute);
            InvoiceImage.EndInit();
            InvoiceImage.Freeze();
            return InvoiceImage;
        }
        public static BitmapImage InvoiceImageFromFilePathHighQuality(string filePath)
        {
            BitmapImage InvoiceImage = new BitmapImage();
            InvoiceImage.BeginInit();
            InvoiceImage.CacheOption = BitmapCacheOption.OnLoad;
            InvoiceImage.UriSource = new Uri(filePath, UriKind.Absolute);
            InvoiceImage.EndInit();
            InvoiceImage.Freeze();
            return InvoiceImage;
        }
        public static void CombineBitmapsFromList(List<Bitmap> bitmapList) 
        {
            var width = 0;
            var height = 0;
            float DpiHorizontal = 0; 
            float DpiVertical = 0; 
            foreach (var image in bitmapList)
            {
                width = image.Width;
                height += image.Height;
                DpiHorizontal = image.HorizontalResolution; 
                DpiVertical = image.VerticalResolution; 
            }
            var bitmap = new Bitmap(width, height);
            bitmap.SetResolution(DpiHorizontal, DpiVertical);
            var g = Graphics.FromImage(bitmap);
            var localWidth = 0;
            var localHeight = 0;
            foreach (var image in bitmapList)
            {
                g.DrawImage(image, localWidth, localHeight);
                localHeight += image.Height + 50;
            }

            string[] files = Directory.GetFiles(FileSystemHelper.TempFolderPath, "Combined*.jpg");
            string baseName = Path.Combine(FileSystemHelper.TempFolderPath, "Combined");
            string filename;
            int i = 0;
            do
            {
                filename = baseName + ++i + ".jpg";
            } while (files.Contains(filename));

            bitmap.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
        }
    }
}
