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
    private readonly OrderFileProviderService _fileProviderService;
    private readonly ILogger<FileController> _logger;

    public FileController(IConfiguration config, OrderFileProviderService service, ILogger<FileController> logger)
    {
        _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
        _fileProviderService = service;
        this._logger = logger;
    }

    [HttpPost]
    [Route("Upload/orderId/{orderId}/jobSequence/{jobSequence}")]
    public IActionResult Upload([Required] IFormFile file,
        [Required] int orderId,
        [Required] int jobSequence)
    {
        if (file.Length > _fileSizeLimit)
        {
            var d = new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary();
            d.AddModelError("file", "out of max size 20Mib");
            return BadRequest(d);
        }

        _logger.LogInformation(orderId + "" + jobSequence);

        _fileProviderService.CreateJobFile(new JobFileAddition()
        {
            FileStream = file.OpenReadStream(),
            JobSequence = jobSequence,
            OrderId = orderId,
            OriginalName = file.FileName
        });

        return Ok();
    }

    [HttpGet]
    public IActionResult GetFile([Required] int orderId,[Required] int jobSequence, [Required] int printerId)
    {
        try
        {
            FileInfo fileInfo = _fileProviderService.GetOrderJobFile(new JobFileRequirement(1, orderId, jobSequence));
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

    [HttpGet]
    [Route("Printer/{printerId}/Lastest")]
    public IActionResult GetFileByPrinter([Required] int printerId)
    {
        try
        {
            FileInfo fileInfo = _fileProviderService.GetPrinterJobFile(printerId);
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