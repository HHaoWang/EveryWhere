using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using HeyRed.Mime;
using EveryWhere.FileServer.Utils;

namespace EveryWhere.FileServer.Controllers;

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

    [HttpGet]
    public IActionResult GetFile([Required] string fileName)
    {
        try
        {
            FileInfo fileInfo = new FileInfo(Path.Combine(FileUtil.GetFileDirectory().FullName, fileName));
            var stream = System.IO.File.OpenRead(fileInfo.FullName);
            return File(stream, MimeTypesMap.GetMimeType(fileInfo.Name), fileInfo.Name);
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }
}