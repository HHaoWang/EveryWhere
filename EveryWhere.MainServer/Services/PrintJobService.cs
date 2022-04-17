using EveryWhere.Database;
using EveryWhere.Database.PO;

namespace EveryWhere.MainServer.Services;

public class PrintJobService:BaseService<PrintJob>
{
    public PrintJobService(Repository repository) : base(repository)
    {
    }
}