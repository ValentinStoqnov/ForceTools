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
        public bool KontragentSideExtract(OperationType operationType, DocumentSides documentSide)
        {
            string PathToChosenDocumentSide = "";
            Regex DostavchikOrPoluchatelRegex = new Regex("");
            switch (documentSide)
            {
                case DocumentSides.LeftSide:
                    PathToChosenDocumentSide = TextLeftFile;
                    break;
                case DocumentSides.RightSide:
                    PathToChosenDocumentSide = TextRightFIle;
                    break;
            }
            switch (operationType)
            {
                case OperationType.Purchase:
                    DostavchikOrPoluchatelRegex = new Regex(@"(?=Доставчик:?).*", RegexOptions.IgnoreCase);

                    break;
                case OperationType.Sale:
                    DostavchikOrPoluchatelRegex = new Regex(@"(?=Получател:?).*", RegexOptions.IgnoreCase);

                    break;
            }
            bool DataFound = DostavchikOrPoluchatelRegex.IsMatch(PathToChosenDocumentSide);
            return DataFound;
        }
        public string KontragentNameExtract(OperationType operationType)
        {
            string KontragentName = "";
            string replacementString = "";
            Regex FullLineRegex = new Regex("");
            Regex ExcluderRegex = new Regex("");
            switch (operationType)
            {
                case OperationType.Purchase:
                    FullLineRegex = new Regex(@"(?<=Доставчик:?).*", RegexOptions.IgnoreCase);
                    ExcluderRegex = new Regex(@"(?=Получател:?).*", RegexOptions.IgnoreCase);

                    break;
                case OperationType.Sale:
                    FullLineRegex = new Regex(@"(?<=Получател:?).*", RegexOptions.IgnoreCase);
                    ExcluderRegex = new Regex(@"(?=Доставчик:?).*", RegexOptions.IgnoreCase);

                    break;
            }
            KontragentName = FullLineRegex.Match(TextFullFile).ToString();
            replacementString = ExcluderRegex.Match(KontragentName).ToString();
            if (replacementString != string.Empty)
            {
                KontragentName = KontragentName.Replace(replacementString, "");
            }
            KontragentName = KontragentName.Replace(":", "");
            KontragentName = KontragentName.Trim();
            return KontragentName;
        }
        public string EIKExtract(DocumentSides documentSide, RegexExtractionMethod regexExtractionMethod)
        {
            string PathToChosenDocumentSide = "";
            string EikString = "";
            Regex FullLineRegex = new Regex(@"(?<=ЕИК|Идент|Ном\.:).*", RegexOptions.IgnoreCase);
            Regex NumbersOnlyExtraction = new Regex("[0-9]{9,10}");
            switch (documentSide)
            {
                case DocumentSides.LeftSide:
                    PathToChosenDocumentSide = TextLeftFile;
                    break;
                case DocumentSides.RightSide:
                    PathToChosenDocumentSide = TextRightFIle;
                    break;
            }
            switch (regexExtractionMethod)
            {
                case RegexExtractionMethod.One:
                    EikString = FullLineRegex.Match(PathToChosenDocumentSide).ToString().Replace("В6", "");
                    EikString = EikString.Replace("В06", "");
                    EikString = NumbersOnlyExtraction.Match(EikString).ToString();
                    break;
                case RegexExtractionMethod.Two:
                    EikString = NumbersOnlyExtraction.Match(PathToChosenDocumentSide).ToString();
                    break;
            }

            return EikString;
        }
        public bool DDSExtract(DocumentSides documentSide)
        {
            string PathToChosenDocumentSide = "";
            Regex FullLineExtraction = new Regex(@"(?<=ДДС).{11,30}", RegexOptions.IgnoreCase);
            Regex NumbersOnlyExtraction = new Regex(@"(\d{9})");
            switch (documentSide)
            {
                case DocumentSides.LeftSide:
                    PathToChosenDocumentSide = TextLeftFile;
                    break;
                case DocumentSides.RightSide:
                    PathToChosenDocumentSide = TextRightFIle;
                    break;
            }
            bool isDDSFound = false;
            string DDSNumberString;
            DDSNumberString = FullLineExtraction.Match(PathToChosenDocumentSide).ToString();
            DDSNumberString = NumbersOnlyExtraction.Match(DDSNumberString).ToString();
            if (DDSNumberString != string.Empty)
                isDDSFound = true;
            return isDDSFound;
        }
        public string InvoiceNumberExtract(RegexExtractionMethod regexExtractionMethod)
        {
            Regex FullLineExtraction = new Regex(@"(?<=Фактура:?).*");
            Regex NumbersOnlyExtraction = new Regex("[0-9]{10}", RegexOptions.IgnoreCase);
            string InvoiceNumber = "";
            switch (regexExtractionMethod)
            {
                case RegexExtractionMethod.One:
                    InvoiceNumber = FullLineExtraction.Match(TextFullFile).ToString();
                    InvoiceNumber = NumbersOnlyExtraction.Match(InvoiceNumber).ToString();
                    break;
                case RegexExtractionMethod.Two:
                    InvoiceNumber = NumbersOnlyExtraction.Match(TextFullFile).ToString();
                    break;
            }
            return InvoiceNumber;
        }
        public string FullValueExtract()
        {
            Regex FullValueExtractionPt1 = new Regex(@"(?<=Сума за плащане:?\s?|Общо:?\s?|Всичко:?\s?).*", RegexOptions.IgnoreCase);
            Regex FullValueExtractionPt2 = new Regex(@"\d{1,5},\d{0,2}");
            string FullValueStringPt1 = FullValueExtractionPt1.Match(TextFullFile).ToString().Trim().Replace(" ", "").Replace(".", ",");
            string FullValue = FullValueExtractionPt2.Match(FullValueStringPt1).ToString();
            return FullValue;
        }
        public string DateExtract(RegexExtractionMethod regexExtractionMethod)
        {
            Regex FullLineExtraction = new Regex(@"(?<=Дата).*", RegexOptions.IgnoreCase);
            Regex DateOnlyExtraction = new Regex(@"\d{1,2}\.\d{1,2}\.\d{1,4}");
            string dateTime = "";
            switch (regexExtractionMethod)
            {
                case RegexExtractionMethod.One:
                    string StrDateExtPT1 = FullLineExtraction.Match(TextFullFile).ToString();
                    dateTime = DateOnlyExtraction.Match(StrDateExtPT1).ToString();
                    break;
                case RegexExtractionMethod.Two:
                    dateTime = DateOnlyExtraction.Match(TextFullFile).ToString();
                    break;
            }
            return dateTime;
        }
        public string DanOsnExtract()
        {
            Regex DanOsnExtractionPt1 = new Regex(@"(?<=Данъчна основа:?\s?|ДО:\s?|Дан\. основа:?\s?|основа:?\s?).*", RegexOptions.IgnoreCase);
            Regex DanOsnExtractionPt2 = new Regex(@"\d{1,5},\d{0,2}|\d{1,5}\.\d{0,2}");
            string DanOsnPt1 = DanOsnExtractionPt1.Match(TextFullFile).ToString().Trim().Replace(" ", "").Replace(".", ",");
            string DoDec = DanOsnExtractionPt2.Match(DanOsnPt1).ToString();
            return DoDec;
        }
        public Tuple<bool, bool, bool> DocTypeExtract()
        {
            Regex FakturaRegex = new Regex(@"(Фактура)", RegexOptions.IgnoreCase);
            Regex KreditRegex = new Regex(@"(Кредитно известие)", RegexOptions.IgnoreCase);
            Regex ToFakturaRegex = new Regex(@"(Към Фактура)", RegexOptions.IgnoreCase);
            var DocumentTypes = Tuple.Create(FakturaRegex.IsMatch(TextFullFile), KreditRegex.IsMatch(TextFullFile), ToFakturaRegex.IsMatch(TextFullFile));
            return DocumentTypes;
        }
    }
}
