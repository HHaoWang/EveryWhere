namespace EveryWhere.MainServer.Contexts.Shop
{
    public class Printer
    {
        public int ShopId { get; set; }
        public string Name { get; set; }
        public string MachineGUID { get; set; }

        public static Printer Create(int shopId, string name, string machineGUID)
        {
            return new Printer(shopId, name, machineGUID);
        }

        private Printer(int shopId, string name, string machineGUID)
        {
            ShopId = shopId;
            Name = name;
            MachineGUID = machineGUID;
        }
    }
}
