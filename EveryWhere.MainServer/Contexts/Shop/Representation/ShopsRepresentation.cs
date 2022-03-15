namespace EveryWhere.MainServer.Contexts.Shop.Representation
{
    public class ShopsRepresentation
    {
        public List<ShopRepresentation> shops;

        public ShopsRepresentation(List<Database.PO.Shop> shops)
        {
            this.shops = new List<ShopRepresentation>();
            shops.ForEach(shop => this.shops.Add(new ShopRepresentation(shop.Id,shop.Name,shop.Printers)));
        }
    }
}
