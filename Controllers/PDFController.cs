using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PDF_API.Models;


namespace PDF_API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class PDFController : ControllerBase {

        [HttpPost("CanFileBeUploaded")]
        public IActionResult CanFileBeUploaded(IFormFile file) {
            try {
                MyPDF.CanBeUpload(file);
                return Ok("Success");
            }
            catch (PDFException e) {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("DeletePage")]
        public ActionResult DeletePage(IFormFile fileToUpload, int pageNumber) {
            try {
                var mypdf = new MyPDF(fileToUpload);

                mypdf.DeletePage(pageNumber);

                byte[] fileBytes = System.IO.File.ReadAllBytes(mypdf.getOutputFilePath());
                mypdf.Clear();

                return File(fileBytes, "application/pdf", "returned.pdf");
            }
            catch (PDFException e) {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("SwapPages")]
        public ActionResult SwapPages(IFormFile fileToUpload, int pageFromSwap, int pageToSwap) {
            try {
                var mypdf = new MyPDF(fileToUpload);

                mypdf.SwapPages(pageFromSwap, pageToSwap);

                byte[] fileBytes = System.IO.File.ReadAllBytes(mypdf.getOutputFilePath());
                mypdf.Clear();

                return File(fileBytes, "application/pdf", "returned.pdf");
            }
            catch (PDFException e) {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("CombinePdfFiles")]
        public ActionResult CombinePdfFiles(IFormFile fileToUpload1, IFormFile fileToUpload2) {
            try {
                var mypdf = new MyPDF(fileToUpload1, fileToUpload2);

                mypdf.CombineFiles();

                byte[] fileBytes = System.IO.File.ReadAllBytes(mypdf.getOutputFilePath());
                mypdf.Clear();

                return File(fileBytes, "application/pdf", "returned.pdf");
            }
            catch (PDFException e) {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("SplitFile")]
        public ActionResult SplitFile(IFormFile fileToUpload, int breakPage) {
            try {
                var mypdf = new MyPDF(fileToUpload);

                mypdf.SplitFile(breakPage);

                byte[] fileBytes = System.IO.File.ReadAllBytes(mypdf.getOutputFilePath());
                mypdf.Clear();

                // Два варианта для отправки нескольких pdf файлов.
                // 1) zip файлом отправить.
                // 2) отправить пути до файла, и потом клиент несколько раз отправляет запрос.
                return File(fileBytes, "application/pdf", "returned.pdf");
            }
            catch (PDFException e) {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("InsertImage")]
        public ActionResult InsertImage(IFormFile pdfFileToUpload, IFormFile imageFileToUpload, int numberPageToInsert, float width, float height, int x, int y) {
            try {
                var mypdf = new MyPDF(pdfFileToUpload);
                var myimage = new MyImage(imageFileToUpload);

                mypdf.InsertImage(myimage, numberPageToInsert, width, height, x, y);

                byte[] fileBytes = System.IO.File.ReadAllBytes(mypdf.getOutputFilePath());
                mypdf.Clear();

                return File(fileBytes, "application/pdf", "returned.pdf");
            }
            catch (PDFException e) {
                return BadRequest(e.Message);
            }
        }
    }
}