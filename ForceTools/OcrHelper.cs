using System.IO;
using Tesseract;

namespace ForceTools
{
    public class OcrHelper
    {
        public static void RunOcr(string imageFilePath)
        {
            using (var ocrEngine = new TesseractEngine(FileSystemHelper.TessTrainedDataFolder, "bul", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(imageFilePath))
                {
                    DoOcrFull(ocrEngine, img);
                    DoOcrRight(ocrEngine, img);
                    DoOcrLeft(ocrEngine, img);
                }
            }
        }
        private static void DoOcrFull(TesseractEngine ocrEngine, Pix img)
        {
            using (var page = ocrEngine.Process(img, Rect.FromCoords(0, 0, img.Width, img.Height)))
            {
                var ocrText = page.GetWordStrBoxText(0);
                File.WriteAllText(FileSystemHelper.FullOcrTxtFilePath, ocrText);
            }
        }
        private static void DoOcrRight(TesseractEngine ocrEngine, Pix img)
        {
            using (var page = ocrEngine.Process(img, Rect.FromCoords(img.Width / 2, 0, img.Width, img.Height / 3)))
            {
                var ocrText = page.GetText();
                File.WriteAllText(FileSystemHelper.RightOcrTxtFilePath, ocrText);
            }
        }
        private static void DoOcrLeft(TesseractEngine ocrEngine, Pix img)
        {
            using (var page = ocrEngine.Process(img, Rect.FromCoords(0, 0, img.Width / 2, img.Height / 3)))
            {
                var ocrText = page.GetText();
                File.WriteAllText(FileSystemHelper.LeftOcrTxtFilePath, ocrText);
            }
        }
    }
}
