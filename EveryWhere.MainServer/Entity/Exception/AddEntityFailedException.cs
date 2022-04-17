namespace EveryWhere.MainServer.Entity.Exception;

public class AddEntityFailedException:System.Exception
{
    public AddEntityFailedException(string? entityName) : base($"添加{entityName}数据失败！")
    {
    }
}