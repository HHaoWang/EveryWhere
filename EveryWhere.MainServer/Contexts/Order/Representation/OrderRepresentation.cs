namespace EveryWhere.MainServer.Contexts.Order.Representation
{
    public class OrderRepresentation
    {
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
        public string Status { get; set; }
        public int ShopId { get; set; }

        public OrderRepresentation(Database.PO.Order o)
        {
            Id = o.Id;
            CreateTime = o.CreateTime;
            Status = o.Status.ToString();
            ShopId = o.ShopId;
        }
    }
}
