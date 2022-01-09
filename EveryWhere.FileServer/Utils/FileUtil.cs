namespace EveryWhere.FileServer.Utils;

public static class FileUtil
{
    /// <summary>
    /// 获取资源文件夹路径
    /// </summary>
    /// <returns>完整的资源文件夹路径</returns>
    public static DirectoryInfo GetSourceDirectory()
    {
        DirectoryInfo appDirectory = new DirectoryInfo(AppContext.BaseDirectory);
        var sourcePath = Path.Combine(appDirectory.Parent!.FullName, "source");
        DirectoryInfo sourceDirectory = new DirectoryInfo(sourcePath);
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
        DirectoryInfo logDirectory = new DirectoryInfo(Path.Combine(sourceDirectory.FullName, "log"));
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
    public static DirectoryInfo GetFileDirectory()
    {
        DirectoryInfo sourceDirectory = GetSourceDirectory();
        DirectoryInfo directory = new DirectoryInfo(Path.Combine(sourceDirectory.FullName, "convertedFile"));
        if (!directory.Exists)
        {
            directory.Create();
        }
        return directory;
    }
}