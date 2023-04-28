using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PDFiumSharp;
using PDFiumSharp.Types;

namespace ForceTools.ViewModels
{
    public static class PdfConverter
    {
        public enum Scale
        {
            Low = 1,
            High,
            VeryHigh
        }

        public enum CompressionLevel : long
        {
            High = 25L,
            Medium = 50L,
            Low = 90L,
            None = 100L
        }

        private static PdfReader Reader { get; set; }

        private static PDFiumBitmap pDFiumBitmap { get; set; } // Added by me

        public static List<System.Drawing.Image> GetImages(string file, Scale scale, List<int> pagenumbers = null)
        {
            if (File.Exists(file) && Path.GetExtension(file).ToLower() == ".pdf")
            {
                pagenumbers = pagenumbers ?? new List<int>();
                if (!File.Exists(file))
                {
                    return new List<System.Drawing.Image>();
                }

                Reader = new PdfReader(file);
                return ProcessPdfToMemory(scale, pagenumbers);
            }

            return new List<System.Drawing.Image>();
        }

        public static List<System.Drawing.Image> GetImages(byte[] file, Scale scale, List<int> pagenumbers = null)
        {
            pagenumbers = pagenumbers ?? new List<int>();
            if (file == null)
            {
                return new List<System.Drawing.Image>();
            }

            Reader = new PdfReader(file);
            return ProcessPdfToMemory(scale, pagenumbers);
        }

        public static void WriteImages(string file, string outputFolder, Scale scale, CompressionLevel compression, List<int> pagenumbers = null)
        {
            if (File.Exists(file) && Path.GetExtension(file).ToLower() == ".pdf" && Directory.Exists(outputFolder) && File.Exists(file))
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                PdfReader.AllowOpenWithFullPermissions = true;
                Reader = new PdfReader(file);
                ProcessPDF2Filesystem(outputFolder, scale, compression, fileNameWithoutExtension, pagenumbers);
                Reader.Close();     // Added by me 
            }
        }

        public static void WriteImages(byte[] file, string outputFolder, Scale scale, CompressionLevel compression, string filename = "pdfpic", List<int> pagenumbers = null)
        {
            if (file != null)
            {
                PdfReader.AllowOpenWithFullPermissions = true;
                Reader = new PdfReader(file);
                ProcessPDF2Filesystem(outputFolder, scale, compression, filename, pagenumbers);
            }
        }

        public static List<byte[]> ExtractJpeg(string file)
        {
            List<byte[]> list = new List<byte[]>();
            if (File.Exists(file) && Path.GetExtension(file).ToLower() == ".pdf")
            {
                PdfReader pdfReader = new PdfReader(file);
                int numberOfPages = pdfReader.NumberOfPages;
                for (int i = 1; i <= numberOfPages; i++)
                {
                    PdfDictionary pageN = pdfReader.GetPageN(i);
                    PdfDictionary pdfDictionary = PdfReader.GetPdfObject(pageN.Get(PdfName.Resources)) as PdfDictionary;
                    PdfDictionary pdfDictionary2 = PdfReader.GetPdfObject(pdfDictionary.Get(PdfName.Xobject)) as PdfDictionary;
                    if (pdfDictionary2 == null)
                    {
                        continue;
                    }

                    ICollection keys = pdfDictionary2.Keys;
                    if (keys.Count == 0)
                    {
                        continue;
                    }

                    PdfName key = keys.OfType<PdfName>().FirstOrDefault();
                    PdfObject pdfObject = pdfDictionary2.Get(key);
                    if (pdfObject.IsIndirect())
                    {
                        PdfDictionary pdfDictionary3 = PdfReader.GetPdfObject(pdfObject) as PdfDictionary;
                        PdfName obj = PdfReader.GetPdfObject(pdfDictionary3?.Get(PdfName.Subtype)) as PdfName;
                        if (PdfName.Image.Equals(obj))
                        {
                            int number = (pdfObject as PrIndirectReference).Number;
                            PrStream stream = pdfReader.GetPdfObject(number) as PrStream;
                            byte[] streamBytesRaw = PdfReader.GetStreamBytesRaw(stream);
                            list.Add(streamBytesRaw);
                        }
                    }
                }

                return list;
            }

            return list;
        }

        public static void ExtractJpeg(string file, string outputfolder)
        {
            if (File.Exists(file) && Path.GetExtension(file).ToLower() == ".pdf")
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                List<byte[]> list = ExtractJpeg(file);
                for (int i = 0; i < list.Count; i++)
                {
                    string path = fileNameWithoutExtension + "_" + $"{i:0000}.jpg";
                    string path2 = Path.Combine(outputfolder, path);
                    File.WriteAllBytes(path2, list[i]);
                }
            }
        }

        private static List<System.Drawing.Image> ProcessPdfToMemory(Scale scale, List<int> pagenumbers)
        {
            List<System.Drawing.Image> list = new List<System.Drawing.Image>();
            for (int i = 1; i <= Reader.NumberOfPages; i++)
            {
                if ((pagenumbers.Any() && pagenumbers.Contains(i)) || !pagenumbers.Any())
                {
                    Stream stream = ExtractPdfPageStream(i);
                    list.Add(GetPdfImage(((MemoryStream)stream).ToArray(), scale));
                }
            }

            Reader.Close();
            return list;
        }

        private static void ProcessPDF2Filesystem(string outputFolder, Scale scale, CompressionLevel compression, string defaultname = "pdfpic", List<int> pagenumbers = null)
        {
            ImageCodecInfo encoder = GetEncoder(ImageFormat.Jpeg);
            Encoder quality = Encoder.Quality;
            EncoderParameters encoderParameters = new EncoderParameters(1);
            EncoderParameter encoderParameter = new EncoderParameter(quality, GetCompression(compression));
            encoderParameters.Param[0] = encoderParameter;
            for (int i = 1; i <= Reader.NumberOfPages; i++)
            {
                if (pagenumbers == null || (pagenumbers.Any() && pagenumbers.Contains(i)))
                {
                    using (var stream = ExtractPdfPageStream(i))  //Changed by me 
                    {
                        using (System.Drawing.Image image = GetPdfImage(((MemoryStream)stream).ToArray(), scale))  //Changed by me 
                        {
                            image.Save($"{outputFolder}\\{defaultname}_{i}.jpg", encoder, encoderParameters);
                            image.Dispose();            // Added by me 
                            pDFiumBitmap.Dispose();     // Added by me
                        }
                    }
                }
            }
            Reader.Close();
        }
        //Re-written by me 
        private static long GetCompression(CompressionLevel compression)
        {
            if (compression == CompressionLevel.High)
            {
                return 25L;
            }
            else if (compression == CompressionLevel.Medium)
            {
                return 50L;
            }
            else if (compression == CompressionLevel.Low)
            {
                return 90L;
            }
            else if (compression == CompressionLevel.None)
            {
                return 100L;
            }
            else
            {
                return 100L;
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] imageDecoders = ImageCodecInfo.GetImageDecoders();
            ImageCodecInfo[] array = imageDecoders;
            foreach (ImageCodecInfo imageCodecInfo in array)
            {
                if (imageCodecInfo.FormatID == format.Guid)
                {
                    return imageCodecInfo;
                }
            }
            return null;
        }

        private static System.Drawing.Image GetPdfImage(byte[] pdf, Scale resolution)
        {

            var pdfDocument = new PDFiumSharp.PdfDocument(pdf);

            PDFiumSharp.PdfPage pdfPage = pdfDocument.Pages[0];

            //PDFiumBitmap pDFiumBitmap = new PDFiumBitmap((int)pdfPage.Size.Width * (int)resolution, (int)pdfPage.Size.Height * (int)resolution, hasAlpha: false);
            pDFiumBitmap = new PDFiumBitmap((int)pdfPage.Size.Width * (int)resolution, (int)pdfPage.Size.Height * (int)resolution, hasAlpha: false); 
            //Changed by me 

            pDFiumBitmap.Fill(new FPDF_COLOR(byte.MaxValue, byte.MaxValue, byte.MaxValue));

            pdfPage.Render(pDFiumBitmap);

            System.Drawing.Image result = System.Drawing.Image.FromStream(pDFiumBitmap.AsBmpStream());

            pdfDocument.Close();
            pdfPage.Dispose();          // Added by me

            return result;
        }

        private static Stream ExtractPdfPageStream(int pagenumber)
        {
            Stream stream = new MemoryStream();
            Document document = new Document(Reader.GetPageSizeWithRotation(pagenumber));
            PdfCopy pdfCopy = new PdfCopy(document, stream);
            document.Open();
            PdfImportedPage importedPage = pdfCopy.GetImportedPage(Reader, pagenumber);
            pdfCopy.AddPage(importedPage);
            document.Close();
            pdfCopy.Close();            // Added by me 
            return stream;
        }
    }
}

