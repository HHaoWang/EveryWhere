using System.Collections.Generic;
using EveryWhere.Database.PO;

namespace EveryWhere.Desktop.Entity.Response;

public class PrintJobsResponse
{
    public List<PrintJob>? Jobs { get; set; }
}