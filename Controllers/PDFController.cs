using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PDF_API.Models;

namespace PDF_API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class PDFController : ControllerBase {

        [HttpPost]
        public IActionResult UploadFile(IFormFile file) {
            return Ok(MyPDF.Upload(file));
        }


    }
}
