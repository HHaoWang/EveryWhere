using System.Reflection;
using System.Security.AccessControl;

namespace EveryWhere.FileServer.Domain;

public static class FileConverterFactory
{
    public static FileConverter? GetFileConverter(FileInfo file)
    {
        string ext = file.Extension;
        List<Type> converters = GetConverters();
        Type? type = converters.Find(c =>
            GetAttributes(c).Exists(a => 
                a.TypeName.Equals(ext, StringComparison.CurrentCultureIgnoreCase)
                )
            );
        if (type != null)
        {
            return Activator.CreateInstance(type) as FileConverter;
        }
        return null;
    }

    private static List<SupportTypeAttribute> GetAttributes(Type t)
    {
        SupportTypeAttribute[] attributes = (SupportTypeAttribute[])Attribute.GetCustomAttributes(t, typeof(SupportTypeAttribute));
        return new List<SupportTypeAttribute>(attributes);
    }

    private static List<Type> GetConverters()
    {
        Type[] assemblyTypes = Assembly.GetCallingAssembly().GetTypes();
        Type typeFullName = typeof(FileConverter);
        List<Type> converterTypes = new();
        foreach (Type type in assemblyTypes)
        {
            Type? baseType = type.BaseType;
            while (baseType != null)
            {
                if (baseType == typeFullName)
                {
                    converterTypes.Add(type);
                    break;
                }
                baseType = baseType.BaseType;
            }
        }

        return converterTypes;
    }
}