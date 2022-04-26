using System.Net;
using EveryWhere.Database;
using EveryWhere.Database.PO;
using EveryWhere.MainServer.Entity.Dto;
using EveryWhere.MainServer.Entity.Response;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace EveryWhere.MainServer.Services;

public class ShopService:BaseService<Shop>
{
    public ShopService(Repository repository):base(repository)
    {
    }

    public List<Shop> GetShopsByAreaCode(string areaCode)
    {
        return GetAll(s => s.AreaCode!.Equals(areaCode));
    }

    
}