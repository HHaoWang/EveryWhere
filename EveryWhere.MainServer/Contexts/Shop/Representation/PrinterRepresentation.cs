namespace EveryWhere.MainServer.Contexts.Shop.Representation
{
    public class PrinterRepresentation
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public PrinterRepresentation(Database.PO.Printer printer)
        {
            Id = printer.Id;
            Name = printer.Name;
        }
    }
}
