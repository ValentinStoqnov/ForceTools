using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;

namespace ForceTools
{
    public static class FileSystemHelper
    {
        public static string DataFolderPath = AppDomain.CurrentDomain.BaseDirectory + "\\Database";
        public static string TempFolderPath = AppDomain.CurrentDomain.BaseDirectory + "\\Temp";
        public static string dbSelectedImg = @"\Assets\databaseSelected.png";
        public static string dbDefaultImg = @"\Assets\database.png";
        public static string ExcelPlaceHolderImage = @".\Assets\ExcelPlaceholderImage.png"; // NEED TO FIX PATH LATER
        public static string TessTrainedDataFolder = @"TrainedData";
        public static string OcrTempFolder = TempFolderPath + "\\Ocr\\";
        public static string RightOcrTxtFilePath = OcrTempFolder + "RightOcrTempText.txt";
        public static string LeftOcrTxtFilePath = OcrTempFolder + "LeftOcrTempText.txt";
        public static string FullOcrTxtFilePath = OcrTempFolder + "FullOcrTempText.txt";
        

        public static void CheckAndCreateDatabaseFolder()
        {
            if (!Directory.Exists(FileSystemHelper.DataFolderPath))
            {
                Directory.CreateDirectory(FileSystemHelper.DataFolderPath);
            }
        }

        public static void CheckAndCreateTempFolder()
        {
            if (!Directory.Exists(FileSystemHelper.TempFolderPath))
            {
                Directory.CreateDirectory(FileSystemHelper.TempFolderPath);
            }
            if (!Directory.Exists(FileSystemHelper.OcrTempFolder))
            {
                Directory.CreateDirectory(FileSystemHelper.OcrTempFolder);
            }
        }

        public static string OpenFileDialogAndGetExcelFilePath()
        {
            var OFD = new OpenFileDialog();
            OFD.Multiselect = false;
            OFD.Filter = "Excel Файлове|*.xls;*.xlsx";
            OFD.ShowDialog();
            return OFD.FileName;
        }

        public static string[] OpenFileDialogAndGetPdfFilePaths()
        {
            var OFD = new OpenFileDialog();
            OFD.Multiselect = true;
            OFD.Filter = "PDF Файлове|*.PDF";
            OFD.ShowDialog();
            return OFD.FileNames;
        }
        public static string[] GetAllJpgFilePathsInTempFolder()
        {
            List<string> imagesInTempFolder = new List<string>();
            DirectoryInfo TempFolder = new DirectoryInfo(TempFolderPath);
            foreach (FileInfo finfo in TempFolder.GetFiles())
            {
                if (".jpg".Contains(finfo.Extension.ToLower()))
                    imagesInTempFolder.Add(finfo.FullName);
            }
            return imagesInTempFolder.ToArray();
        }
        public static void DelteFilesFromList(List<string> filesToBeDeleted)
        {
            foreach (string s in filesToBeDeleted)
            { 
                File.Delete(s);
            }
        }
        public static void ClearTempFolder() 
        {
            var TempFolder = new DirectoryInfo(TempFolderPath);
            foreach (FileInfo finfo in TempFolder.GetFiles())
            {
                if (".jpg".Contains(finfo.Extension.ToLower()))
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    File.Delete(finfo.FullName);
                }
            }
        }
    }
}
