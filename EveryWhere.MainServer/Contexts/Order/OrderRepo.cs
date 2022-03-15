using EveryWhere.Database;

namespace EveryWhere.MainServer.Contexts.Order
{
    public class OrderRepo
    {
        private readonly Repository _repository;

        public OrderRepo(Repository repository)
        {
            _repository = repository;
        }

        public int insert(Order order)
        {
            Database.PO.Order pOrder = new Database.PO.Order()
            {
                ShopId = order.ShopId,
                Status = Database.PO.Order.StatusState.NotUploaded,
                PrintJobs = new List<Database.PO.PrintJob>()
            };
            order.Jobs.ForEach(job => pOrder.PrintJobs.Add(new Database.PO.PrintJob()
            {
                PrinterId = 1,
                JobSequence = job.Sequence,
                Status = job.Status
            }));
            _repository.Order.Add(pOrder);
            _repository.SaveChanges();

            order.DistributeId(pOrder.Id);
            order.Jobs.ForEach(j =>
            {
                var t = pOrder.PrintJobs.FirstOrDefault(p => p.JobSequence == j.Sequence);
                if (t != null) j.Id = t.Id;
            });
            order.CreateTime = pOrder.CreateTime;


            return pOrder.Id;
        }
    }
}
