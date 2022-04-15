using System.Reflection;
using System.Security.AccessControl;

namespace EveryWhere.FileServer.Domain;

public static class FileConverterFactory
{
    /// <summary>
    /// 获取根据文件信息获取具体转换器类实例
    /// </summary>
    /// <param name="file">给定的文件信息</param>
    /// <returns>具体转换器实例</returns>
    public static FileConverter? GetFileConverter(FileInfo file)
    {
        string ext = file.Extension;
        //获取所有转换器类
        List<Type> converters = GetConverters();
        //通过注解特性寻找支持给定文件格式的具体转换器类
        Type? type = converters.Find(converterType =>
            GetAttributes(converterType).Exists(attribute => 
                attribute.TypeName.Equals(ext, StringComparison.CurrentCultureIgnoreCase)
                )
            );
        //如果找到了支持给定文件格式的具体转换器类就实例化并返回
        if (type != null)
        {
            return Activator.CreateInstance(type) as FileConverter;
        }
        //找不到符合要求的类则返回空
        return null;
    }

    /// <summary>
    /// 获取给定类型的所有注解特性
    /// </summary>
    /// <param name="type">给定的类型</param>
    /// <returns>给定类型的所有注解特性</returns>
    private static List<SupportTypeAttribute> GetAttributes(Type type)
    {
        SupportTypeAttribute[] attributes = (SupportTypeAttribute[])Attribute
            .GetCustomAttributes(type, typeof(SupportTypeAttribute));
        return new List<SupportTypeAttribute>(attributes);
    }

    /// <summary>
    /// 获取程序集中的所有具体转换器类
    /// </summary>
    /// <returns>本程序集中的所有转换器类</returns>
    private static List<Type> GetConverters()
    {
        //获取程序集中的所有类型
        Type[] assemblyTypes = Assembly.GetCallingAssembly().GetTypes();
        //筛选出具体转换器类
        List<Type> converterTypes = new();
        Type typeFullName = typeof(FileConverter);
        foreach (Type type in assemblyTypes)
        {
            Type? baseType = type.BaseType;
            //依据继承链向上查找是否继承自抽象转换器类
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