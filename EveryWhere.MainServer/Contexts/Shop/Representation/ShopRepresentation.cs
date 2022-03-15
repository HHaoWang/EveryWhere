using Newtonsoft.Json;

namespace EveryWhere.MainServer.Contexts.Shop.Representation
{
    public class ShopRepresentation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonProperty("printers")]
        List<PrinterRepresentation> Printers;

        public ShopRepresentation(int id, string name, List<Database.PO.Printer> printers)
        {
            Id = id;
            Name = name;
            Printers = new List<PrinterRepresentation>();
            printers.ForEach(printer => Printers.Add(new PrinterRepresentation(printer)));
        }
    }
}
