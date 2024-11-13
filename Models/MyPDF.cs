using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace PDF_API.Models {
    public class MyPDF {
        public static List<string> validExtensions = new List<string>() { ".pdf" };
        public string fileName1;
        public string fileName2 = "";
        public string inputFilePath1;
        public string inputFilePath2 = "";
        public string outputFilePath;
        public int countUploadedDocuments;
        public static string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "pdf");

        public MyPDF(IFormFile fileToUpload1) {
            CanBeUpload(fileToUpload1);
            fileName1 = Upload(fileToUpload1);
            inputFilePath1 = GetUploadPath(fileName1);
            outputFilePath = GetEditPath(fileName1);
            countUploadedDocuments = 1;
        }

        public MyPDF(IFormFile fileToUpload1, IFormFile fileToUpload2) {
            CanBeUpload(fileToUpload1);
            CanBeUpload(fileToUpload2);

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

        public static bool CanBeUpload(IFormFile file) {
            string extention = Path.GetExtension(file.FileName);

            if (!validExtensions.Contains(extention)) {
                throw new PDFException("Extension is not valid");
            }

            long size = file.Length;

            if (size > (100 * 1024 * 1024)) {
                throw new PDFException("Incorrect number of documents uploaded.");
            }

            return true;
        }

        public string Upload(IFormFile file) {
            string extention = Path.GetExtension(file.FileName);

            string fileName = Guid.NewGuid().ToString() + extention;

            using FileStream stream = new FileStream(Path.Combine(uploadFolder, fileName), FileMode.Create);

            file.CopyTo(stream);

            return fileName;
        }

        public static void DeleteFile(string filePath) {
            if (File.Exists(filePath)) {
                File.Delete(filePath);
            }
        }

        public string GetUploadPath(string fileName) {
            return Path.Combine(uploadFolder, fileName);
        }

        public string GetEditPath(string fileName) {
            return Path.Combine(uploadFolder, "Edited" + fileName);
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

            using (var pdfDocument = new PdfDocument(new PdfReader(inputFilePath1), new PdfWriter(outputFilePath))) {
                if (pageNumber < 1 || pageNumber > pdfDocument.GetNumberOfPages()) {
                    throw new PDFException("The page specified is outside the scope of the document.");
                }

                int deleted = 0;
                foreach (var i in pagesToDelete) {
                    pdfDocument.RemovePage(i - deleted);
                    deleted++;
                }
            }

            return true;
        }

        public bool SwapPages(int pageFromSwap, int pageToSwap) {
            List<int> pages = new List<int>();

            using (var inputPdfDocument = new PdfDocument(new PdfReader(inputFilePath1))) {
                using (var outputPdfDocument = new PdfDocument(new PdfWriter(outputFilePath))) {
                    if (pageFromSwap < 1 || pageToSwap < 1 || pageFromSwap > inputPdfDocument.GetNumberOfPages()
                        || pageToSwap > inputPdfDocument.GetNumberOfPages()) {
                        throw new PDFException("The page specified is outside the scope of the document.");
                    }

                    for (int i = 1; i <= inputPdfDocument.GetNumberOfPages(); i++) {
                        pages.Add(i);
                    }

                    pages[pageFromSwap - 1] = pageToSwap;
                    pages[pageToSwap - 1] = pageFromSwap;

                    inputPdfDocument.CopyPagesTo(pages, outputPdfDocument);
                }
            }

            return true;
        }

        public bool CombineFiles() {

            if (countUploadedDocuments != 2) {
                throw new PDFException("Incorrect number of documents uploaded.");
            }

            using (var inputPdfDocument2 = new PdfDocument(new PdfReader(inputFilePath2))) {
                using (var outputPdfDocument = new PdfDocument(new PdfReader(inputFilePath1), new PdfWriter(outputFilePath))) {
                    List<int> pages = new List<int>();

                    for (int i = 1; i <= inputPdfDocument2.GetNumberOfPages(); i++) {
                        pages.Add(i);
                    }

                    inputPdfDocument2.CopyPagesTo(pages, outputPdfDocument);
                }
            }

            return true;
        }

        public void SplitFile(int breakPage) {
            using (var inputPdfDocument = new PdfDocument(new PdfReader(inputFilePath1), new PdfWriter(inputFilePath1))) {
                using (var outputPdfDocument = new PdfDocument(new PdfWriter(outputFilePath))) {

                    if (breakPage < 1 || breakPage > inputPdfDocument.GetNumberOfPages()) {
                        throw new PDFException("The page specified is outside the scope of the document.");
                    }

                    List<int> pages = new List<int>();
                    for (int i = breakPage; i <= inputPdfDocument.GetNumberOfPages(); i++) {
                        pages.Add(i);
                    }


                    inputPdfDocument.CopyPagesTo(pages, outputPdfDocument);

                    int deleted = 0;
                    foreach (var i in pages) {
                        inputPdfDocument.RemovePage(i - deleted);
                        deleted++;
                    }
                }
            }
        }

        public void InsertImage(MyImage image, int numberPageToInsert, float width, float height, int x, int y) {
            using (var inputPdfDocument = new PdfDocument(new PdfReader(inputFilePath1), new PdfWriter(outputFilePath))) {
                using (var document = new Document(inputPdfDocument)) {
                    var pagePageSize = inputPdfDocument.GetPage(numberPageToInsert).GetPageSize();

                    if (y < 0 || y > pagePageSize.GetHeight() || x < 0 || x > pagePageSize.GetWidth()) {
                        throw new PDFException("The coordinates specified are outside the page.");
                    }

                    if (height < 0 || height > pagePageSize.GetHeight() || width < 0 || width > pagePageSize.GetWidth()) {
                        throw new PDFException("Dimensions are beyond the page.");
                    }

                    if (numberPageToInsert < 1 || numberPageToInsert > inputPdfDocument.GetNumberOfPages()) {
                        throw new PDFException("The page specified is outside the scope of the document.");
                    }

                    ImageData imageData = ImageDataFactory.Create(image.GetPath());

                    Image newImage = new Image(imageData).ScaleAbsolute(width, height).SetFixedPosition(numberPageToInsert, x, y);
                    document.Add(newImage);
                }
            }
        }
    }
}
