using static EveryWhere.Database.PO.Order;

namespace EveryWhere.MainServer.Contexts.Order;
#nullable disable
public class Order
{
    public int Id { get; set; }
    public int ShopId { get; }
    public List<PrintJob> Jobs { get; }
    public decimal TotalPrice { get; }
    public StatusState Status { get; }
    public DateTime CreateTime { get; set; }

    public Order(int shopId)
    {
        ShopId = shopId;
        Jobs = new List<PrintJob>();
        Status = StatusState.NotUploaded;
    }

    public void Pay()
    {
        if (Jobs.Exists(j => j.Status < Database.PO.PrintJob.StatusState.Uploaded))
        {
            throw new FileNotFoundException();
        }

        Jobs.ForEach(j => j.StartConversion());
    }

    public void DistributeId(int id)
    {
        this.Id = id;
    }

    public void AppendJob()
    {
        Jobs.Add(new PrintJob(Jobs.Count + 1));
    }
}

