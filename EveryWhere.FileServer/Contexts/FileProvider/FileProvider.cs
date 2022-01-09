using EveryWhere.FileServer.Contexts.FileProvider.Exception;
using File = EveryWhere.FileServer.Contexts.FileProvider.Entity.File;

namespace EveryWhere.FileServer.Contexts.FileProvider;

public class FileProvider
{
    private List<File> Files { get; }

    public FileProvider(List<File> files)
    {
        this.Files = files;
    }

    internal FileInfo GetJobFile(int jobSequence)
    {
        var file = Files.FirstOrDefault(f=> f.JobSequence == jobSequence);
        if (file is null)
        {
            throw new JobFileNotFoundException();
        }

        return file.FileInfo;
    }
}