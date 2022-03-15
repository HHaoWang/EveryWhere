using EveryWhere.MainServer.Contexts.Shop.Representation;

namespace EveryWhere.MainServer.Contexts.Shop
{
    public class ShopApplicationService
    {
        private readonly ShopRepo _shopRepo;
        private readonly ShopFactory _shopFactory;
        private readonly ShopRepresentationService _shopRepresentationService;

        public ShopApplicationService(ShopRepo shopRepo, ShopFactory shopFactory, ShopRepresentationService shopRepresentationService)
        {
            _shopRepo = shopRepo;
            _shopFactory = shopFactory;
            _shopRepresentationService = shopRepresentationService;
        }

        public ShopsRepresentation GetShops()
        {
            return _shopRepresentationService.GetShops();
        }
    }
}
