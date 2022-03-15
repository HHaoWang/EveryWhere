namespace EveryWhere.MainServer.Contexts.Order
{
    public class OrderApplicationService
    {
        private readonly OrderRepo _orderRepo;

        public OrderApplicationService(OrderRepo orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public int CreateOrder(int shopId)
        {
            Order order = OrderFactory.Create(shopId);
            _orderRepo.insert(order);
            return order.Id;
        }
    }
}
