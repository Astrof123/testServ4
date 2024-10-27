using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PDF_API.Models;

namespace PDF_API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class PDF : ControllerBase {

        [HttpPost]
        //[DisableCors]
        public IActionResult Negr(IFormFile file) {
            return Ok(new UploadHandler().Upload(file));
        }
    }
}
