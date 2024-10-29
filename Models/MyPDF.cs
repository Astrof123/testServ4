using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Components.Forms;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

namespace PDF_API.Models {
    public class MyPDF {
        public static List<string> validExtensions = new List<string>() { ".pdf" };

        public static string GetPath(string fileName) {
            return Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName);
        }

        public static string GetEditPath(string fileName) {
            return Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "d" + fileName);
        }

        public static PdfDocument ReadPdfFile(string path) {
            var reader = new PdfReader(path);

            var pdfDocument = new PdfDocument(reader);

            return pdfDocument;
        }

        public static bool DeletePage(PdfDocument pdf_file, string inputFilePath, string outputFilePath, int pageNumber) {
            if (pageNumber < 1 || pageNumber > pdf_file.GetNumberOfPages()) {
                return false;
            }
            
            string sourceFile = inputFilePath;
            string targetFile = outputFilePath;
            int[] pagesToDelete = { pageNumber };

            using (var pdfReader = new PdfReader(sourceFile))
            using (var pdfWriter = new PdfWriter(targetFile)) {
                using (var pdfDocument = new PdfDocument(pdfReader, pdfWriter)) {
                    int deleted = 0;
                    foreach (var i in pagesToDelete) {
                        pdfDocument.RemovePage(i - deleted);
                        deleted++;
                    }
                }
            }

            return true;
        }

        public static string CanBeUpload(IFormFile file) {
            string extention = Path.GetExtension(file.FileName);

            if (!validExtensions.Contains(extention)) {
                return "Extension is not valid";
            }

            long size = file.Length;

            if (size > (100 * 1024 * 1024)) {
                return "Maximum size can be 100Mb";
            }

            return "Success";
        }

        public static string Upload(IFormFile file) {
            string extention = Path.GetExtension(file.FileName);

            string fileName = Guid.NewGuid().ToString() + extention;

            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            using FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create);

            file.CopyTo(stream);

            return fileName;
        }

        public static void DeleteFile(string filePath) {
            if (File.Exists(filePath)) {
                File.Delete(filePath);
            }
        }
    }
}
