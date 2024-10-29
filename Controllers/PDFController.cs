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
            return Ok(MyPDF.CanBeUpload(file));
        }

        [HttpPost("DeletePage")]
        public ActionResult DeletePage(IFormFile fileToUpload, int pageNumber) {
            if (MyPDF.CanBeUpload(fileToUpload) == "Success") {

                string fileName = MyPDF.Upload(fileToUpload);
                string inputFilePath = MyPDF.GetUploadPath(fileName);
                var file = MyPDF.ReadPdfFile(inputFilePath);

                string outputFilePath = MyPDF.GetEditPath(fileName);
                if (!MyPDF.DeletePage(file, inputFilePath, outputFilePath, pageNumber)) {
                    return BadRequest("Incorrect file page selected");
                }

                file.Close();
                byte[] fileBytes = System.IO.File.ReadAllBytes(outputFilePath);

                MyPDF.DeleteFile(inputFilePath);
                MyPDF.DeleteFile(outputFilePath);

                return File(fileBytes, "application/pdf", "returned.pdf");
            }
            else {
                return BadRequest("This file cannot be processed");
            }
        }

        [HttpPost("SwapPages")]
        public ActionResult SwapPages(IFormFile fileToUpload, int pageFromSwap, int pageToSwap) {
            if (MyPDF.CanBeUpload(fileToUpload) == "Success") {

                string fileName = MyPDF.Upload(fileToUpload);
                string inputFilePath = MyPDF.GetUploadPath(fileName);

                string outputFilePath = MyPDF.GetEditPath(fileName);
                if (!MyPDF.SwapPages(inputFilePath, outputFilePath, pageFromSwap, pageToSwap)) {
                    return BadRequest("Incorrect file page selected");
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(outputFilePath);

                MyPDF.DeleteFile(inputFilePath);
                MyPDF.DeleteFile(outputFilePath);

                return File(fileBytes, "application/pdf", "returned.pdf");
            }
            else {
                return BadRequest("This file cannot be processed");
            }
        }

    }
}
