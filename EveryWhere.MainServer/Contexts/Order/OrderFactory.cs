namespace EveryWhere.MainServer.Contexts.Order
{
    public class OrderFactory
    {
        public static Order Create(int shopId)
        {
            var order = new Order(shopId);
            order.AppendJob();
            return order;
        }
    }
}
