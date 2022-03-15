namespace EveryWhere.MainServer.Contexts.Shop.DTO
{
    public class CreatePrinterCommand
    {
        public int ShopId { get; set; }
        public string Name { get; set; }
        public string MachineGUID { get; set; }
    }
}
