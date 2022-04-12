using EveryWhere.MainServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EveryWhere.MainServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AreaController : ControllerBase
{
    private readonly AreaService _areaService;

    public AreaController(AreaService areaService)
    {
        _areaService = areaService;
    }

    [HttpGet]
    [Route("Tree")]
    public IActionResult GetAreasTree()
    {
        return new JsonResult(new
        {
            statusCode = 200,
            areasTree = _areaService.GetAreasTree()
        });
    }

    [HttpGet]
    [Route("SplitList")]
    public async Task<IActionResult> GetAreasListAsync()
    {
        (Dictionary<string, string> provinceList, Dictionary<string, string> cityList,
            Dictionary<string, string> countryList) = await _areaService.GetAreaTupleAsync();
        return new JsonResult(new
        {
            statusCode = 200,
            data = new
            {
                provinceList,
                cityList,
                countryList
            }
        });
    }
}