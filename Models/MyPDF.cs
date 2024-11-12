using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

namespace PDF_API.Models {
    public class MyPDF {
        public static List<string> validExtensions = new List<string>() { ".pdf" };
        public string fileName1;
        public string inputFilePath1;
        public string inputFilePath2 = "";
        public string outputFilePath;
        public int countUploadedDocuments;
        public string fileName2 = "";

        public MyPDF(IFormFile fileToUpload1) {
            fileName1 = Upload(fileToUpload1);
            inputFilePath1 = GetUploadPath(fileName1);
            outputFilePath = GetEditPath(fileName1);
            countUploadedDocuments = 1;
        }

        public MyPDF(IFormFile fileToUpload1, IFormFile fileToUpload2) {
            fileName1 = Upload(fileToUpload1);
            fileName2 = Upload(fileToUpload2);
            inputFilePath1 = GetUploadPath(fileName1);
            inputFilePath2 = GetUploadPath(fileName2);
            outputFilePath = GetEditPath(fileName1);
            countUploadedDocuments = 2;
        }

        public string getOutputFilePath() {
            return outputFilePath;
        }



        public string GetUploadPath(string fileName) {
            return Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName);
        }

        public string GetEditPath(string fileName) {
            return Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Edited" + fileName);
        }

        public void Clear() {
            if (countUploadedDocuments == 1) {
                DeleteFile(inputFilePath1);
                DeleteFile(outputFilePath);
            }
            else if (countUploadedDocuments == 2) {
                DeleteFile(inputFilePath1);
                DeleteFile(inputFilePath2);
                DeleteFile(outputFilePath);
            }
        }

        public bool DeletePage(int pageNumber) {
            int[] pagesToDelete = { pageNumber };

            using (var pdfReader = new PdfReader(inputFilePath1))
            using (var pdfWriter = new PdfWriter(outputFilePath)) {
                using (var pdfDocument = new PdfDocument(pdfReader, pdfWriter)) {
                    if (pageNumber < 1 || pageNumber > pdfDocument.GetNumberOfPages()) {
                        return false;
                    }

                    int deleted = 0;
                    foreach (var i in pagesToDelete) {
                        pdfDocument.RemovePage(i - deleted);
                        deleted++;
                    }
                }
            }

            return true;
        }

        public bool SwapPages(int pageFromSwap, int pageToSwap) {
            List<int> pages = new List<int>();

            var pdfReader = new PdfReader(inputFilePath1);
            using (var inputPdfDocument = new PdfDocument(pdfReader)) {
                using (var pdfWriter = new PdfWriter(outputFilePath)) {
                    using (var outputPdfDocument = new PdfDocument(pdfWriter)) {
                        if (pageFromSwap < 1 || pageToSwap < 1 || pageFromSwap > inputPdfDocument.GetNumberOfPages()
                            || pageToSwap > inputPdfDocument.GetNumberOfPages()) {
                            return false;
                        }

                        for (int i = 1; i <= inputPdfDocument.GetNumberOfPages(); i++) {
                            pages.Add(i);
                        }

                        pages[pageFromSwap - 1] = pageToSwap;
                        pages[pageToSwap - 1] = pageFromSwap;

                        inputPdfDocument.CopyPagesTo(pages, outputPdfDocument);
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

        public bool CombineFiles() {

            if (countUploadedDocuments != 2) {
                return false;
            }

            using (var pdfReader = new PdfReader(inputFilePath1))
            using (var pdfReader2 = new PdfReader(inputFilePath2))
            using (var inputPdfDocument1 = new PdfDocument(pdfReader)) {
                using (var inputPdfDocument2 = new PdfDocument(pdfReader2)) {
                    using (var pdfWriter = new PdfWriter(outputFilePath)) {
                        using (var outputPdfDocument = new PdfDocument(pdfWriter)) {
                            List<int> pages = new List<int>();

                            for (int i = 1; i <= inputPdfDocument1.GetNumberOfPages(); i++) {
                                pages.Add(i);
                            }

                            inputPdfDocument1.CopyPagesTo(pages, outputPdfDocument);

                            pages = new List<int>();

                            for (int i = 1; i <= inputPdfDocument2.GetNumberOfPages(); i++) {
                                pages.Add(i);
                            }

                            inputPdfDocument2.CopyPagesTo(pages, outputPdfDocument);
                        }
                    }
                }
            }

            return true;
        }
    }
}
