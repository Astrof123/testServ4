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
                var mypdf = new MyPDF(fileToUpload);

                if (!mypdf.DeletePage(pageNumber)) {
                    return BadRequest("Incorrect file page selected");
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(mypdf.getOutputFilePath());
                mypdf.Clear();

                return File(fileBytes, "application/pdf", "returned.pdf");
            }
            else {
                return BadRequest("This file cannot be processed");
            }
        }

        [HttpPost("SwapPages")]
        public ActionResult SwapPages(IFormFile fileToUpload, int pageFromSwap, int pageToSwap) {
            if (MyPDF.CanBeUpload(fileToUpload) == "Success") {
                var mypdf = new MyPDF(fileToUpload);

                if (!mypdf.SwapPages(pageFromSwap, pageToSwap)) {
                    return BadRequest("Incorrect file page selected");
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(mypdf.getOutputFilePath());
                mypdf.Clear();

                return File(fileBytes, "application/pdf", "returned.pdf");
            }
            else {
                return BadRequest("This file cannot be processed");
            }
        }

        [HttpPost("CombinePdfFiles")]
        public ActionResult CombinePdfFiles(IFormFile fileToUpload1, IFormFile fileToUpload2) {
            if (MyPDF.CanBeUpload(fileToUpload1) == "Success" && MyPDF.CanBeUpload(fileToUpload2) == "Success") {
                var mypdf = new MyPDF(fileToUpload1, fileToUpload2);


                if (!mypdf.CombineFiles()) {
                    return BadRequest("Combine files failed");
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(mypdf.getOutputFilePath());
                mypdf.Clear();

                return File(fileBytes, "application/pdf", "returned.pdf");
            }
            else {
                return BadRequest("This file cannot be processed");
            }
        }
    }
}