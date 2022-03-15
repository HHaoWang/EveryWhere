using EveryWhere.FileServer.Contexts.FileProvider.Exception;
using JobFile = EveryWhere.FileServer.Contexts.FileProvider.Entity.JobFile;

namespace EveryWhere.FileServer.Contexts.FileProvider;

public class OrderFileProvider
{
    public int OrderId { get; }
    public List<JobFile> Files { get; }

    public OrderFileProvider(int orderId,List<JobFile> files)
    {
        this.OrderId = orderId;
        this.Files = files;
    }

    public FileInfo GetJobFile(int jobSequence)
    {
        var file = Files.FirstOrDefault(f=> f.JobSequence == jobSequence);
        if (file is null)
        {
            throw new JobFileNotFoundException();
        }

        return file.FileInfo;
    }

    public void AddJobFile(FileInfo fileInfo,int jobSequence, string originalName)
    {
        var jobFile = Files.FirstOrDefault(f => f.JobSequence == jobSequence);
        if (jobFile is null)
        {
            throw new JobFileNotFoundException();
        }
        jobFile.SetFile(fileInfo,originalName);
    }

    public int GetJobFileId(int jobSequence)
    {
        return Files.Find(f => f.JobSequence == jobSequence)?.Id ?? throw new JobFileNotFoundException();
    }
}