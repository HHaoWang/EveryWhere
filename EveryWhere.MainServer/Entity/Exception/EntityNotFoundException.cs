namespace EveryWhere.MainServer.Entity.Exception;

public class EntityNotFoundException:System.Exception
{
    public EntityNotFoundException(string entityName,int entityId) 
        : base($"没有找到ID为{entityId}的{entityName}")
    {
    }
}