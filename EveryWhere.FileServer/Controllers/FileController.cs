using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using HeyRed.Mime;
using EveryWhere.FileServer.Utils;

namespace EveryWhere.FileServer.Controllers;

[Route("api/[controller]/Converted")]
[ApiController]
public class FileController : ControllerBase
{
    [Route("Wps/{name}")]
    [HttpGet]
    public IActionResult GetWpsFile(string name)
    {
        FileInfo fileInfo = new(Path.Combine(FileUtil.GetWpsFileDirectory().FullName, name));
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

    [Route("Pdf/{name}")]
    [HttpGet]
    public IActionResult GetPdfFile(string name)
    {
        FileInfo fileInfo = new(Path.Combine(FileUtil.GetPdfFileDirectory().FullName, name));
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
}