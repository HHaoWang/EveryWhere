using EveryWhere.MainServer.Contexts.Shop.DTO;

namespace EveryWhere.MainServer.Contexts.Shop
{
    public class Shop
    {
        public int Id { get; set; }
        public List<Printer> Printers { get; set; }

        public void AddPrinter(CreatePrinterCommand command)
        {
            Printers.Add(Printer.Create(command.ShopId,command.Name,command.MachineGUID));
        }

        
    }
}
