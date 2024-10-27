using iText.Kernel.Pdf;
using System.Reflection.PortableExecutable;

namespace PDF_API.Models {
    public class MyPDF {
        public static PdfDocument ReadPdfFile(string fileName) {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            var reader = new PdfReader(Path.Combine(path, fileName));
            var pdfDocument = new PdfDocument(reader);

            return pdfDocument;
        }

        public static void DeletePage(int pageNumber) {

        }

        public static string Upload(IFormFile file) {
            List<string> validExtensions = new List<string>() { ".pdf" };

            string extention = Path.GetExtension(file.FileName);

            if (!validExtensions.Contains(extention)) {
                return "Extension is not valid";
            }

            long size = file.Length;

            if (size > (100 * 1024 * 1024)) {
                return "Maximum size can be 100Mb";
            }

            string fileName = Guid.NewGuid().ToString() + extention;

            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            using FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create);

            file.CopyTo(stream);

            return fileName;
        }
    }
}
