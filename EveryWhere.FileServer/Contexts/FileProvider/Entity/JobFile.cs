using EveryWhere.Database.PO;

namespace EveryWhere.FileServer.Contexts.FileProvider.Entity;

public class JobFile
{
    public int? Id { get; }
    public int JobSequence { get; }
    public string? OriginalFileName { get; set; }
    public FileInfo? FileInfo { get; set; }
    public PrintJob.StatusState Status { get; }

    public int JobId { get; }

    public JobFile(int? id, int jobSequence, FileInfo? fileInfo, string? originalFileName, PrintJob.StatusState status,int jobId)
    {
        Id = id;
        JobSequence = jobSequence;
        FileInfo = fileInfo;
        OriginalFileName = originalFileName;
        Status = status;
        JobId = jobId;
    }

    public string GetPhysicalFileName()
    {
        return FileInfo.Name;
    }

    public void SetFile(FileInfo fileInfo,string originalName)
    {
        this.OriginalFileName = originalName;
        this.FileInfo = fileInfo;
    }
}