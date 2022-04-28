using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using HeyRed.Mime;
using EveryWhere.FileServer.Utils;

namespace EveryWhere.FileServer.Controllers;

[Route("api/[controller]/Converted")]
[ApiController]
public class FileController : ControllerBase
{
    [Route("Xps/{name}")]
    [HttpGet]
    public IActionResult GetXpsFile(string name)
    {
        FileInfo fileInfo = new(Path.Combine(FileUtil.GetXpsFileDirectory().FullName, name+".xps"));
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
        FileInfo fileInfo = new(Path.Combine(FileUtil.GetPdfFileDirectory().FullName, name+".pdf"));
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