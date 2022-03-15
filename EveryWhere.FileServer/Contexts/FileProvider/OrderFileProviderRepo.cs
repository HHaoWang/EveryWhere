using EveryWhere.Database;
using EveryWhere.Database.PO;
using EveryWhere.FileServer.Contexts.FileProvider.Entity;
using EveryWhere.Util;
using EveryWhere.FileServer.Contexts.FileProvider.Exception;
using Microsoft.EntityFrameworkCore;
using File = EveryWhere.Database.PO.File;

namespace EveryWhere.FileServer.Contexts.FileProvider;

public class OrderFileProviderRepo
{
    private readonly Repository _repository;

    public OrderFileProviderRepo(Repository repository)
    {
        _repository = repository;
    }

    public OrderFileProvider GetOrderFileProvider(int orderId)
    {
        List<PrintJob> printJobs = _repository.PrintJob
            .Include(j => j.File)
            .Where(j => j.OrderId == orderId)
            .ToList();

        List<Entity.JobFile> fileInfos = new List<Entity.JobFile>();
        string fileDirectoryPath = FileUtil.GetFileDirectory().ToString();

        foreach (var job in printJobs)
        {
            if (job.File==null)
            {
                fileInfos.Add(
                new Entity.JobFile(
                    null,
                    job.JobSequence,
                    null,
                    null,
                    job.Status,
                    job.Id
                    )
                );
            }
            else
            {
                fileInfos.Add(
                new Entity.JobFile(
                    job.File.Id,
                    job.JobSequence,
                    new FileInfo(Path.Combine(fileDirectoryPath, job.File.Name)),
                    job.File.OriginalName,
                    job.Status,
                    job.Id
                    )
                );
            }
        }

        return new OrderFileProvider(orderId, fileInfos);
    }

    /// <summary>
    /// 在添加新的打印任务对应的文件后持久化provider
    /// </summary>
    /// <param name="provider">要保存的provider</param>
    /// <returns>新添加的打印任务对应的文件的id</returns>
    public int SaveOnAdd(OrderFileProvider provider)
    {
        List<JobFile> files = provider.Files;
        JobFile? file = files.FirstOrDefault(f => f.Id is null);
        if (file == null)
            throw new JobFileNotFoundException();
        File pFile = new()
        {
            Name = file.GetPhysicalFileName(),
            OriginalName = file.OriginalFileName,
            PrintJobId = file.JobId
        };
        _repository.File.Add(pFile);
        _repository.SaveChanges();
        return pFile.Id;
    }

    public PrintJob GetPrintJobByPrinter(int id)
    {
        PrintJob? job = _repository.PrintJob.Include(j => j.File)
            .Where(j => j.PrinterId == id)
            .Where(j => j.Status == PrintJob.StatusState.Uploaded)
            .OrderByDescending(j => j.File.CreateTime)
            .FirstOrDefault();
        if (job is null)
        {
            throw new JobFileNotFoundException();
        }
        job.Status = PrintJob.StatusState.Queuing;
        _repository.SaveChanges();
        return job;
    }
}