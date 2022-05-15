using EveryWhere.Database;
using EveryWhere.Database.PO;

namespace EveryWhere.MainServer.Services;

public class ShopViewService:BaseService<ShopView>
{
    public ShopViewService(Repository repository) : base(repository)
    {
    }

    /// <summary>
    /// 浏览店铺
    /// </summary>
    /// <param name="shopId">店铺ID</param>
    /// <returns></returns>
    public async Task VisitShop(int shopId)
    {
        _ = await AddAsync(new ShopView
        {
            ShopId = shopId
        });
    }
}