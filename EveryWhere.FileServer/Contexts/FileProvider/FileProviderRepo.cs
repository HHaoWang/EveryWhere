using EveryWhere.Database;
using EveryWhere.FileServer.Utils;
using File = EveryWhere.Database.PO.File;

namespace EveryWhere.FileServer.Contexts.FileProvider;

public class FileProviderRepo
{
    private readonly Repository _repository;

    public FileProviderRepo(Repository repository)
    {
        _repository = repository;
    }

    public FileProvider GetFileProvider(int orderId)
    {
        List<File> files = _repository.File.Where(file => file.OrderId == orderId)
            .ToList();

        List<Entity.File> fileInfos = new List<Entity.File>();
        string fileDirectoryPath = FileUtil.GetFileDirectory().ToString();
        foreach (File file in files)
        {
            fileInfos.Add(new Entity.File()
            {
                JobSequence = file.JobSequence,
                FileInfo = new FileInfo(Path.Combine(fileDirectoryPath, file.Name))
            });
        }
        return new FileProvider(fileInfos);
    }
}