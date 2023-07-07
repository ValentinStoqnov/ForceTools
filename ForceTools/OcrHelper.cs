using System.IO;
using Tesseract;

namespace ForceTools
{
    public class OcrHelper
    {
        public static void RunOcr(string imageFilePath) 
        {
            DoOcrLeft(imageFilePath);
            DoOcrRight(imageFilePath);
            DoOcrFull(imageFilePath);
        }
        #region NewOcrTechnique
        private static void DoOcrLeft(string imageFilePath)
        {

            if (!Directory.Exists(FileSystemHelper.OcrTempFolder))
            {
                Directory.CreateDirectory(FileSystemHelper.OcrTempFolder);
            }


            using (var ocrEngine = new TesseractEngine(FileSystemHelper.TessTrainedDataFolder, "bul", EngineMode.TesseractAndLstm))
            {
                ocrEngine.SetVariable("user_defined_dpi", "300"); //set dpi for supressing warning
                using (var img = Pix.LoadFromFile(imageFilePath))
                {
                    //LeftOcr
                    using (var page = ocrEngine.Process(img, Rect.FromCoords(0, 0, img.Width / 2, img.Height / 3)))
                    {
                        var ocrText = page.GetText();

                        File.WriteAllText(FileSystemHelper.LeftOcrTxtFilePath, ocrText);

                    }
                }
            }
        }
        private static void DoOcrRight(string imageFilePath)
        {
            using (var ocrEngine = new TesseractEngine(FileSystemHelper.TessTrainedDataFolder, "bul", EngineMode.TesseractAndLstm))
            {
                ocrEngine.SetVariable("user_defined_dpi", "300"); //set dpi for supressing warning
                using (var img = Pix.LoadFromFile(imageFilePath))
                {
                    //RightOcr
                    using (var page = ocrEngine.Process(img, Rect.FromCoords(img.Width / 2, 0, img.Width, img.Height / 3)))
                    {
                        var ocrText = page.GetText();

                        File.WriteAllText(FileSystemHelper.RightOcrTxtFilePath, ocrText);

                    }
                }
            }
        }
        private static void DoOcrFull(string imageFilePath)
        {

            using (var ocrEngine = new TesseractEngine(FileSystemHelper.TessTrainedDataFolder, "bul", EngineMode.TesseractAndLstm))
            {
                ocrEngine.SetVariable("user_defined_dpi", "300"); //set dpi for supressing warning
                using (var img = Pix.LoadFromFile(imageFilePath))
                {
                    //FullOcr
                    using (var page = ocrEngine.Process(img, Rect.FromCoords(0, 0, img.Width, img.Height)))
                    {
                        var ocrText = page.GetText();

                        File.WriteAllText(FileSystemHelper.FullOcrTxtFilePath, ocrText);

                    }
                }
            }
        }
        #endregion

        #region OldOcrTechnique
        //private void DoOcr(string sourceForOcrFilePath)
        //{

        //    if (!Directory.Exists(OcrTempFolder))
        //    {
        //        Directory.CreateDirectory(OcrTempFolder);
        //    }


        //    using (var ocrEngine = new TesseractEngine(TessTrainedDataFolder, "bul", EngineMode.TesseractAndLstm))
        //    {
        //        ocrEngine.SetVariable("user_defined_dpi", "300"); //set dpi for supressing warning
        //        using (var img = Pix.LoadFromFile(sourceForOcrFilePath))
        //        {
        //            //LeftOcr
        //            using (var page = ocrEngine.Process(img, Tesseract.Rect.FromCoords(0, 0, img.Width / 2, img.Height / 3)))
        //            {
        //                var ocrText = page.GetText();

        //                File.WriteAllText(LeftOcrTxtFilePath, ocrText);

        //            }
        //            //RightOcr
        //            using (var page = ocrEngine.Process(img, Tesseract.Rect.FromCoords(img.Width / 2, 0, img.Width, img.Height / 3)))
        //            {
        //                var ocrText = page.GetText();

        //                File.WriteAllText(RightOcrTxtFilePath, ocrText);

        //            }
        //            //FullOcr
        //            using (var page = ocrEngine.Process(img, Tesseract.Rect.FromCoords(0, 0, img.Width, img.Height)))
        //            {
        //                var ocrText = page.GetText();

        //                File.WriteAllText(FullOcrTxtFilePath, ocrText);

        //            }
        //        }
        //    }
        //}
        #endregion
    }
}
