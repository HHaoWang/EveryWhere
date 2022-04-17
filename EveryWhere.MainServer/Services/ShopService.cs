using EveryWhere.Database;
using EveryWhere.Database.PO;

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