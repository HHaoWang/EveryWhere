using EveryWhere.Database;
using EveryWhere.Database.PO;

namespace EveryWhere.MainServer.Services;

public class ShopService
{
    private readonly Repository _repository;

    public ShopService(Repository repository)
    {
        _repository = repository;
    }

    public List<Shop> GetShopsByAreaCode(string areaCode)
    {
        return _repository.Shops
            !.Where(s => s.AreaCode.Equals(areaCode)).ToList();
    }
}