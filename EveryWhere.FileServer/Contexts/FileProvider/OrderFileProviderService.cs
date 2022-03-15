using EveryWhere.FileServer.Contexts.FileProvider.DTO;
using EveryWhere.Util;

namespace EveryWhere.FileServer.Contexts.FileProvider;

/// <summary>
/// aka FileProviderApplicationService
/// </summary>
public class OrderFileProviderService
{
    private readonly OrderFileProviderRepo _fileProviderRepo;
    private readonly FileConvertNotifyFacade _fileConvertNotifyFacade;

    public OrderFileProviderService(OrderFileProviderRepo fileProviderRepo, FileConvertNotifyFacade fileConvertNotifyFacade)
    {
        _fileProviderRepo = fileProviderRepo;
        _fileConvertNotifyFacade = fileConvertNotifyFacade;
    }

    /// <summary>
    /// 获取所需的文件
    /// </summary>
    /// <param name="requirement">需要的文件的标识</param>
    /// <returns>文件信息</returns>
    public FileInfo GetOrderJobFile(JobFileRequirement requirement)
    {
        OrderFileProvider provider = _fileProviderRepo.GetOrderFileProvider(requirement.OrderId);
        FileInfo orderJobFile = provider.GetJobFile(requirement.JobSequence);
        return orderJobFile;
    }

    /// <summary>
    /// 创建打印任务文件
    /// </summary>
    /// <param name="addition"></param>
    /// <returns></returns>
    public void CreateJobFile(JobFileAddition addition)
    {
        OrderFileProvider provider = _fileProviderRepo.GetOrderFileProvider(addition.OrderId);
        FileInfo fileInfo = PhysicalFileService.SaveFileToUploadDir(addition.FileStream, addition.OriginalName);
        provider.AddJobFile(fileInfo, addition.JobSequence, addition.OriginalName);
        int newFileId = _fileProviderRepo.SaveOnAdd(provider);
        _fileConvertNotifyFacade.AddFile(newFileId);
    }

    public FileInfo GetPrinterJobFile(int printerId)
    {
        var job = _fileProviderRepo.GetPrintJobByPrinter(printerId);
        string fileName = Path.Combine(FileUtil.GetConvertedFileDirectory().FullName,job.File.Name);
        return new FileInfo(fileName);
    }
}