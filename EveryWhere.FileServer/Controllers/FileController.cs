using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EveryWhere.FileServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly long _fileSizeLimit;

        public FileController(IConfiguration config)
        {
            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
        }

        [Authorize]
        [HttpPost]
        [Route("Upload")]
        public IActionResult Upload([Required] IFormFile file)
        {
            if (file.Length > _fileSizeLimit)
            {
                var d = new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary();
                d.AddModelError("file", "out of max size 20Mib");
                return BadRequest(d);
            }

            return new JsonResult(new { statusCode = 200, data = new { savedFileName = "/api/Article/Img/" } });
        }
    }
}
