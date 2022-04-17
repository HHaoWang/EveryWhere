namespace EveryWhere.MainServer.Entity.Exception;

public class UpdateEntityException:System.Exception
{
    public UpdateEntityException(string? entityName,int entityId)
        : base($"更新ID为{entityId}的{entityName}数据失败！")
    {
    }
}