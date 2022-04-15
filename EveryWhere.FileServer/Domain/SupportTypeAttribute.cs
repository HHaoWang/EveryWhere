namespace EveryWhere.FileServer.Domain;

[AttributeUsage(AttributeTargets.Class,AllowMultiple = true,Inherited = false)]
public class SupportTypeAttribute:Attribute
{
    public string TypeName { get; }

    public SupportTypeAttribute(string typeName)
    {
        TypeName = typeName;
    }
}