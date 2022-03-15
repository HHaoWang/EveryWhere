namespace EveryWhere.MainServer.Contexts.Order.Representation
{
    public class OrdersRepresentation
    {
        public List<OrderRepresentation> Orders{ get; set; }

        public OrdersRepresentation(List<Database.PO.Order> orders)
        {
            Orders = new List<OrderRepresentation>();
            orders.ForEach(o =>
            {
                Orders.Add(new OrderRepresentation(o));
            });
        }
    }
}
