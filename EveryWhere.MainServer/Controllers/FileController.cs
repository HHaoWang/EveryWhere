using EveryWhere.MainServer.Utils;
using HeyRed.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using EveryWhere.MainServer.Services;
using File = EveryWhere.Database.PO.File;

namespace EveryWhere.MainServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly long _fileSizeLimit;
    private readonly FileService _fileService;
    private readonly ILogger<FileController> _logger;

    public FileController(IConfiguration config, FileService fileService, ILogger<FileController> logger)
    {
        _fileService = fileService;
        _logger = logger;
        _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
    }

    [Route("Avatar/{name}")]
    [HttpGet]
    public IActionResult GetAvatarFile(string name)
    {
        FileInfo fileInfo = FileUtil.GetAvatar(name);
        try
        {
            FileStream stream = System.IO.File.OpenRead(fileInfo.FullName);
            return File(stream, MimeTypesMap.GetMimeType(fileInfo.Name), fileInfo.Name);
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }

    [Route("Uploaded/{name}")]
    [HttpGet]
    public IActionResult GetUploadedFile(string name)
    {
        FileInfo fileInfo = new(Path.Combine(FileUtil.GetUploadedFileDirectory().FullName, name));
        _logger.LogInformation(fileInfo.FullName);
        try
        {
            FileStream stream = System.IO.File.OpenRead(fileInfo.FullName);
            return File(stream, MimeTypesMap.GetMimeType(fileInfo.Name), fileInfo.Name);
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }

    [Route("StaticImg/{name}")]
    [HttpGet]
    public IActionResult GetStaticImgFile(string name)
    {
        FileInfo fileInfo = new(Path.Combine(FileUtil.GetStaticImgDirectory().FullName, name));
        try
        {
            FileStream stream = System.IO.File.OpenRead(fileInfo.FullName);
            return File(stream, MimeTypesMap.GetMimeType(fileInfo.Name), fileInfo.Name);
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [Route("Upload")]
    [Authorize]
    public async Task<IActionResult> UploadFileAsync([Required] IFormFile file,[Required][FromForm] string fileName)
    {
        int uploaderId = Convert.ToInt32(HttpContext.User.FindFirst(c=>c.Type.Equals("UserId",StringComparison.CurrentCultureIgnoreCase))?.Value);

        if (file.Length > _fileSizeLimit)
        {
            var d = new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary();
            d.AddModelError("file", "上传文件最大为30Mib！");
            return BadRequest(d);
        }

        File fileRecord = await _fileService.UploadFile(file.OpenReadStream(),
            fileName, uploaderId);
        return new JsonResult(new
        {
            statusCode = 200,
            data = fileRecord
        });
    }

    [HttpGet]
    [Route("Info/{id:int}")]
    public async Task<IActionResult> GetFileInfo([Required]int id)
    {
        return new JsonResult(new
        {
            statusCode = 200,
            data = await _fileService.GetByIdAsync(id)
        });
    }
}