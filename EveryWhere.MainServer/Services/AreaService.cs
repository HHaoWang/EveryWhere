using System.Text.RegularExpressions;
using EveryWhere.Database;
using EveryWhere.Database.PO;
using Microsoft.EntityFrameworkCore;

namespace EveryWhere.MainServer.Services;

public class AreaService:BaseService<Area>
{
    public AreaService(Repository repository) : base(repository)
    {
    }

    public List<Area> GetAreasTree()
    {
        return Repository.Areas
            !.AsNoTracking()
            .Where(a => a.ParentAreaId == null)
            .Include(a => a.SubAreas)
            !.ThenInclude(a=>a.SubAreas)
            .ToList();
    }

    public async Task<(Dictionary<string, string> provinceList, Dictionary<string, string> cityList, Dictionary<string, string> countryList)> GetAreaTupleAsync()
    {
        var provinceList = new Dictionary<string, string>();
        await Repository.Areas
            !.AsNoTracking()
            .Where(a => a.ParentAreaId == null)
            .ForEachAsync(a => provinceList.Add(a.AreaCode!, a.Name!));

        var cityList = new Dictionary<string, string>();
        await Repository.Areas
            !.AsNoTracking()
            .Where(a => Regex.IsMatch(a.AreaCode!, "[1-9][0-9](0[1-9]|[1-9][0-9])00"))
            .ForEachAsync(a => cityList.Add(a.AreaCode!, a.Name!));

        var countryList = new Dictionary<string, string>();
        await Repository.Areas
            !.AsNoTracking()
            .Where(a => Regex.IsMatch(a.AreaCode!, "....(0[1-9]|[1-9][0-9])"))
            .ForEachAsync(a => countryList.Add(a.AreaCode!, a.Name!));
        
        return (provinceList, cityList, countryList);
    }
}