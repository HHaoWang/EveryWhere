using EveryWhere.Database;
using EveryWhere.Database.PO;

namespace EveryWhere.MainServer.Services;

public class PrinterService:BaseService<Printer>
{
    public PrinterService(Repository repository) : base(repository)
    {
    }
}