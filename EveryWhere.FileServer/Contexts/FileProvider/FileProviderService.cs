using EveryWhere.FileServer.Contexts.FileProvider.DTO;

namespace EveryWhere.FileServer.Contexts.FileProvider;

/// <summary>
/// aka FileProviderApplicationService
/// </summary>
public class FileProviderService
{
    private readonly FileProviderRepo _fileProviderRepo;

    public FileProviderService(FileProviderRepo fileProviderRepo)
    {
        _fileProviderRepo = fileProviderRepo;
    }

    /// <summary>
    /// 获取所需的文件
    /// </summary>
    /// <param name="requirement">需要的文件的标识</param>
    /// <returns>文件信息</returns>
    public FileInfo GetOrderJobFile(FileRequirement requirement)
    {
        FileProvider provider = _fileProviderRepo.GetFileProvider(requirement.OrderId);
        FileInfo orderJobFile = provider.GetJobFile(requirement.JobSequence);
        return orderJobFile;
    }
}