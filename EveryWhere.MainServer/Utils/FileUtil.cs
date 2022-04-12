namespace EveryWhere.MainServer.Utils;

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
    /// 获取图片文件夹路径
    /// </summary>
    /// <returns>完整的图片文件夹路径</returns>
    private static DirectoryInfo GetImgDirectory()
    {
        DirectoryInfo sourceDirectory = GetSourceDirectory();
        DirectoryInfo imgDirectory = new(Path.Combine(sourceDirectory.FullName, "img"));
        if (!imgDirectory.Exists)
        {
            imgDirectory.Create();
        }
        return imgDirectory;
    }

    /// <summary>
    /// 获取头像文件存放路径
    /// </summary>
    /// <returns>完整的头像文件存放路径</returns>
    public static DirectoryInfo GetAvatarDirectory()
    {
        DirectoryInfo imgDirectory = GetImgDirectory();
        DirectoryInfo avatarDirectory = new(Path.Combine(imgDirectory.FullName, "avatar"));
        if (!avatarDirectory.Exists)
        {
            avatarDirectory.Create();
        }
        return avatarDirectory;
    }
}