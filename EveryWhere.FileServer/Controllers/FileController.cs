using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using EveryWhere.FileServer.Contexts.FileProvider;
using EveryWhere.FileServer.Contexts.FileProvider.DTO;
using EveryWhere.FileServer.Contexts.FileProvider.Exception;
using HeyRed.Mime;

namespace EveryWhere.FileServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly long _fileSizeLimit;
    private readonly FileProviderService _fileProviderService;

    public FileController(IConfiguration config,FileProviderService service)
    {
        _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
        _fileProviderService = service;
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
    public IActionResult GetFile([Required] int orderId,[Required] int jobSequence, [Required] int printerId)
    {
        try
        {
            FileInfo fileInfo = _fileProviderService.GetOrderJobFile(new FileRequirement(1, orderId, jobSequence));
            var stream = System.IO.File.OpenRead(fileInfo.FullName);
            return File(stream, MimeTypesMap.GetMimeType(fileInfo.Name), fileInfo.Name);
        }
        catch (JobFileNotFoundException)
        {
            return NotFound();
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }
}