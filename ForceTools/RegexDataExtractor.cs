using System;
using System.IO;
using System.Text.RegularExpressions;

namespace ForceTools
{
    public class RegexDataExtractor
    {
        private string TextLeftFile;
        private string TextRightFIle;
        private string TextFullFile;

        public RegexDataExtractor()
        {
            TextLeftFile = File.ReadAllText(FileSystemHelper.LeftOcrTxtFilePath);
            TextRightFIle = File.ReadAllText(FileSystemHelper.RightOcrTxtFilePath);
            TextFullFile = File.ReadAllText(FileSystemHelper.FullOcrTxtFilePath);
        }

        public string KontragentNameExtract(OperationType operationType, DocumentSides documentSide, RegexExtractionMethod extractionMethod)
        {
            string KontragentName = "";
            string PathToChosenDocumentSide = "";
            Regex kontragentExtractionPT1 = new Regex("");
            Regex kontragentExtractionPT2 = new Regex("");
            switch (operationType)
            {
                case OperationType.Purchase:
                    kontragentExtractionPT1 = new Regex(@"(?<=Доставчик:?).*", RegexOptions.IgnoreCase);
                    kontragentExtractionPT2 = new Regex(@"(?<=Доставчик:?\n).*", RegexOptions.IgnoreCase);
                    break;
                case OperationType.Sale:
                    kontragentExtractionPT1 = new Regex(@"(?<=Получател:?).*", RegexOptions.IgnoreCase);
                    kontragentExtractionPT2 = new Regex(@"(?<=Получател:?\n).*", RegexOptions.IgnoreCase);
                    break;
            }
            switch (documentSide)
            {
                case DocumentSides.LeftSide:
                    PathToChosenDocumentSide = TextLeftFile;
                    break;
                case DocumentSides.RightSide:
                    PathToChosenDocumentSide = TextRightFIle;
                    break;
            }
            switch (extractionMethod) 
            {
                case RegexExtractionMethod.One:
                    KontragentName = kontragentExtractionPT1.Match(PathToChosenDocumentSide).ToString().Trim();
                    break;
                case RegexExtractionMethod.Two:
                    KontragentName = kontragentExtractionPT2.Match(PathToChosenDocumentSide).ToString().Trim();
                    break;
            }
            return KontragentName;
        }
        public string EIKExtract(DocumentSides documentSide) 
        {
            string PathToChosenDocumentSide = "";
            Regex EikExtractionPT1 = new Regex(@"(?<=ЕИК|Идент|Ном\.:).*", RegexOptions.IgnoreCase);
            Regex EikExtractionPT2 = new Regex("[0-9]{9,10}");
            switch (documentSide)
            {
                case DocumentSides.LeftSide:
                    PathToChosenDocumentSide = TextLeftFile;
                    break;
                case DocumentSides.RightSide:
                    PathToChosenDocumentSide = TextRightFIle;
                    break;
            }
            string EikStringPT1 = EikExtractionPT1.Match(PathToChosenDocumentSide).ToString();
            string EIK = EikExtractionPT2.Match(EikStringPT1).ToString();
            return EIK;
        }
        public string DDSExtract(DocumentSides documentSide) 
        {
            string PathToChosenDocumentSide = "";
            Regex DDSNumberRegexPT1 = new Regex(@"(?<=ДДС).{11,30}", RegexOptions.IgnoreCase);
            Regex DDSNumberRegexPT2 = new Regex(@"(\d{9})");
            switch (documentSide)
            {
                case DocumentSides.LeftSide:
                    PathToChosenDocumentSide = TextLeftFile;
                    break;
                case DocumentSides.RightSide:
                    PathToChosenDocumentSide = TextRightFIle;
                    break;
            }
            string DDSNumberStringPt1 = DDSNumberRegexPT1.Match(PathToChosenDocumentSide).ToString();
            string DDS = $"BG{DDSNumberRegexPT2.Match(DDSNumberStringPt1)}";
            return DDS;
        }
        public string InvoiceNumberExtract()
        {
            Regex InvoiceNumberExtraction = new Regex("[0-9]{10}", RegexOptions.IgnoreCase);
            string InvoiceNumber = InvoiceNumberExtraction.Match(TextFullFile).ToString();
            return InvoiceNumber;
        }
        public string FullValueExtract()
        {
            Regex FullValueExtractionPt1 = new Regex(@"(?<=Сума за плащане:?\s?|Общо:?\s?|Всичко:?\s?).*", RegexOptions.IgnoreCase);
            Regex FullValueExtractionPt2 = new Regex(@"\d{1,5},\d{0,2}");
            string FullValueStringPt1 = FullValueExtractionPt1.Match(TextFullFile).ToString().Trim().Replace(" ", "");
            string FullValue = FullValueExtractionPt2.Match(FullValueStringPt1).ToString();
            return FullValue;
        }
        public string DateExtract()
        {
            Regex DateExtractionPT1 = new Regex(@"(?<=Дата).*", RegexOptions.IgnoreCase);
            Regex DateExtractionPT2 = new Regex(@"\d{1,2}\.\d{1,2}\.\d{1,4}");
            string StrDateExtPT1 = DateExtractionPT1.Match(TextFullFile).ToString();
            string dateTime = DateExtractionPT2.Match(StrDateExtPT1).ToString();
            return dateTime;
        }
        public string DanOsnExtract()
        {
            Regex DanOsnExtractionPt1 = new Regex(@"(?<=Данъчна основа:?\s?|ДО:\s?|Дан. основа:?\s?).*", RegexOptions.IgnoreCase);
            Regex DanOsnExtractionPt2 = new Regex(@"\d{1,5},\d{0,2}");
            string DanOsnPt1 = DanOsnExtractionPt1.Match(TextFullFile).ToString().Trim().Replace(" ", "");
            string DoDec = DanOsnExtractionPt2.Match(DanOsnPt1).ToString();
            return DoDec;
        }
        public Tuple<bool,bool,bool> DocTypeExtract()
        {
            Regex FakturaRegex = new Regex(@"(Фактура)", RegexOptions.IgnoreCase);
            Regex KreditRegex = new Regex(@"(Кредитно известие)", RegexOptions.IgnoreCase);
            Regex ToFakturaRegex = new Regex(@"(Към Фактура)", RegexOptions.IgnoreCase);
            var DocumentTypes = Tuple.Create(FakturaRegex.IsMatch(TextFullFile), KreditRegex.IsMatch(TextFullFile), ToFakturaRegex.IsMatch(TextFullFile));
            return DocumentTypes;
        }
    }
}
