using ForceTools.Models;
using ForceTools.ViewModels;
using System.Collections.Generic;
using System.Drawing;
using Tesseract;

namespace ForceTools
{
    public class InvoiceImageListsManipulations
    {
        public MainListOfLists CreateMainListOfInvoiceImageLists() 
        {
            string[] invoicePdfFilePaths = FileSystemHelper.OpenFileDialogAndGetPdfFilePaths();
            PdfConverter.WriteMultipleImages(invoicePdfFilePaths, FileSystemHelper.TempFolderPath, PdfConverter.Scale.VeryHigh, PdfConverter.CompressionLevel.None);
            MainListOfLists mainListOfLists = new MainListOfLists();
            string[] invoiceJpgFilePaths = FileSystemHelper.GetAllJpgFilePathsInTempFolder();
            foreach (string invoiceJpgPath in invoiceJpgFilePaths)
            {
                mainListOfLists.Add(new InvoiceImageList(new InvoiceImage(BitmapCreator.InvoiceImageFromFilePathLowQuality(invoiceJpgPath), invoiceJpgPath)));
            }
            return mainListOfLists;
        }
        public static void CombineInvoiceFromMainListOfLists(MainListOfLists mainList) 
        {
            var ImagesToBeDeletedList = new List<string>();
            foreach (InvoiceImageList iml in mainList)
            {
                if (iml.ListOfImages.Count > 1) 
                {
                    var BitmapList = new List<Bitmap>();
                    foreach (InvoiceImage imageClass in iml.ListOfImages)
                    {
                        var bitmap = new Bitmap(imageClass.ImagePath);
                        BitmapList.Add(bitmap);
                        ImagesToBeDeletedList.Add(imageClass.ImagePath);
                    }
                    BitmapCreator.CombineBitmapsFromList(BitmapList);
                }
            }
            FileSystemHelper.DelteFilesFromList(ImagesToBeDeletedList);
        }
    }
}
