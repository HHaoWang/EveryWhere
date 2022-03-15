using EveryWhere.Database;
using Microsoft.EntityFrameworkCore;

namespace EveryWhere.MainServer.Contexts.Shop.Representation
{
    public class ShopRepresentationService
    {
        private readonly Repository _repository;

        public ShopRepresentationService(Repository repository)
        {
            _repository = repository;
        }

        public ShopsRepresentation GetShops()
        {
            List<Database.PO.Shop> shops = _repository.Shop.Include(s => s.Printers)
                .OrderByDescending(s => s.CreateTime)
                .ToList();
            return new ShopsRepresentation(shops);
        }
    }
}
