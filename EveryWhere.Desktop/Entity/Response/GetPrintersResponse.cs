using System.Collections.Generic;
using EveryWhere.Database.PO;

namespace EveryWhere.Desktop.Entity.Response;

public class GetPrintersResponse
{
    public List<Printer>? Printers { get; set; }
}