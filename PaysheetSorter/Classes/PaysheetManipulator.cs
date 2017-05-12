using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PaysheetSorter.Classes
{
    class PaysheetManipulator
    {

        static Regex expression_regular = new Regex(@"Usuario:\s(\w+)");

        public static void SplitPdfs(string OriginalPdf, List<string> PdfSheets, string workdir = "")
        {
            if (workdir == "") workdir = System.IO.Path.GetTempPath() + DateTime.Now.Ticks.ToString();

            Directory.CreateDirectory(workdir);

            PdfReader sourcePdf = new PdfReader(OriginalPdf);
            Document destinationPdfContainer = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage = null;

            for (int numPage = 0; numPage < sourcePdf.NumberOfPages; numPage++)
            {
                destinationPdfContainer = new Document(sourcePdf.GetPageSizeWithRotation(1));
                pdfCopyProvider = new PdfCopy(destinationPdfContainer, new FileStream(System.IO.Path.Combine(workdir, numPage + ".pdf"), FileMode.Create));
                destinationPdfContainer.Open();

                importedPage = pdfCopyProvider.GetImportedPage(sourcePdf, numPage + 1);
                pdfCopyProvider.AddPage(importedPage);
                PdfSheets.Add(System.IO.Path.Combine(workdir, numPage + ".pdf"));
                destinationPdfContainer.Close();
            }
            
        }


        public static UserData ClasifySheet(string PdfSheetPath, IEnumerable<UserData> Users)
        {
            PdfReader pdfReader = new PdfReader(PdfSheetPath);
            ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
            string pdf_text = PdfTextExtractor.GetTextFromPage(pdfReader, 1, strategy);
            pdf_text = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(pdf_text)));
            Match match = expression_regular.Match(pdf_text);
            if (match.Success)
            {
                foreach (UserData currentUser in Users)
                {
                    if (currentUser.Pattern == match.Groups[1].Value) { return currentUser; }
                }
            }
            return null;
        }
    }
}
