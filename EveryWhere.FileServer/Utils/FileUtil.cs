namespace EveryWhere.FileServer.Utils;

public static class FileUtil
{
    /// <summary>
    /// 获取资源文件夹路径
    /// </summary>
    /// <returns>完整的资源文件夹路径</returns>
    public static DirectoryInfo GetSourceDirectory()
    {
        DirectoryInfo appDirectory = new(AppContext.BaseDirectory);
        var sourcePath = Path.Combine(appDirectory.Parent!.FullName, "source");
        DirectoryInfo sourceDirectory = new(sourcePath);
        if (!sourceDirectory.Exists)
        {
            sourceDirectory.Create();
        }
        return sourceDirectory;
    }

    /// <summary>
    /// 获取日志文件存放路径
    /// </summary>
    /// <returns>完整的日志文件存放路径</returns>
    public static DirectoryInfo GetLogDirectory()
    {
        DirectoryInfo sourceDirectory = GetSourceDirectory();
        DirectoryInfo logDirectory = new(Path.Combine(sourceDirectory.FullName, "log"));
        if (!logDirectory.Exists)
        {
            logDirectory.Create();
        }
        return logDirectory;
    }

    /// <summary>
    /// 获取下发文件存放路径
    /// </summary>
    /// <returns>完整的下发文件存放路径</returns>
    public static DirectoryInfo GetConvertedFileDirectory()
    {
        DirectoryInfo sourceDirectory = GetSourceDirectory();
        DirectoryInfo directory = new(Path.Combine(sourceDirectory.FullName, "ConvertedFile"));
        if (!directory.Exists)
        {
            directory.Create();
        }
        return directory;
    }

    /// <summary>
    /// 获取待转换文件存放路径
    /// </summary>
    /// <returns>完整的待转换文件存放路径</returns>
    public static DirectoryInfo GetUnconvertedFileDirectory()
    {
        DirectoryInfo sourceDirectory = GetSourceDirectory();
        DirectoryInfo directory = new(Path.Combine(sourceDirectory.FullName, "UnconvertedFile"));
        if (!directory.Exists)
        {
            directory.Create();
        }
        return directory;
    }

    /// <summary>
    /// 获取转换后的PDF文件存放路径
    /// </summary>
    /// <returns>完整的转换后的PDF文件存放路径</returns>
    public static DirectoryInfo GetPdfFileDirectory()
    {
        DirectoryInfo convertedFileDirectory = GetConvertedFileDirectory();
        DirectoryInfo directory = new(Path.Combine(convertedFileDirectory.FullName, "pdf"));
        if (!directory.Exists)
        {
            directory.Create();
        }
        return directory;
    }

    /// <summary>
    /// 获取转换后的XPS文件存放路径
    /// </summary>
    /// <returns>完整的转换后的XPS文件存放路径</returns>
    public static DirectoryInfo GetWpsFileDirectory()
    {
        DirectoryInfo convertedFileDirectory = GetConvertedFileDirectory();
        DirectoryInfo directory = new(Path.Combine(convertedFileDirectory.FullName, "xps"));
        if (!directory.Exists)
        {
            directory.Create();
        }
        return directory;
    }
}