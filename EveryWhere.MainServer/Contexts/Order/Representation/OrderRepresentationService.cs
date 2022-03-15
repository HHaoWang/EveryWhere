using EveryWhere.Database;

namespace EveryWhere.MainServer.Contexts.Order.Representation
{
    public class OrderRepresentationService
    {
        private readonly Repository _repository;

        public OrderRepresentationService(Repository repository)
        {
            _repository = repository;
        }

        public OrdersRepresentation GetOrders()
        {
            List<Database.PO.Order> orders = _repository.Order
                .OrderByDescending(O => O.CreateTime)
                .ToList();

            return new OrdersRepresentation(orders);
        }
    }
}
