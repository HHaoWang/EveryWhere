namespace EveryWhere.MainServer.Entity.Exception;

public class PrinterNotSupportTicketException:System.Exception
{
    public PrinterNotSupportTicketException(int printerId,string propertyName)
        :base($"ID为{printerId}的打印机不支持打印{propertyName}")
    {
    }
}