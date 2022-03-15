namespace EveryWhere.FileServer.Contexts.FileProvider.DTO;

public class JobFileRequirement
{
    public int ShopId { get;}
    public int OrderId { get;}
    public int JobSequence { get;}

    public JobFileRequirement(int shopId, int orderId, int jobSequence)
    {
        ShopId = shopId;
        OrderId = orderId;
        JobSequence = jobSequence;
    }
}