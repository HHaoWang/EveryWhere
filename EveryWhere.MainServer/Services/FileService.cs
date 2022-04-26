using ByteSizeLib;
using EveryWhere.Database;
using EveryWhere.Database.PO;
using EveryWhere.MainServer.Infrastructure.MessageQueue;
using EveryWhere.MainServer.Utils;
using File = EveryWhere.Database.PO.File;

namespace EveryWhere.MainServer.Services;

public class FileService:BaseService<File>
{
    private readonly Publisher _publisher;
    private readonly IConfiguration _configuration;

    public FileService(Repository repository, Publisher publisher, IConfiguration configuration) : base(repository)
    {
        this._publisher = publisher;
        _configuration = configuration;
    }

    public async Task<File> UploadFile(Stream file, string originalName, int uploaderId)
    {
        string extension = Path.GetExtension(originalName);
        string fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-") + Path.GetRandomFileName() + extension;
        string fullFileName = Path.Combine(FileUtil.GetUploadedFileDirectory().FullName, fileName);

        await using FileStream stream = new(fullFileName, FileMode.Create);
        await file.CopyToAsync(stream);
        File fileRecord = new()
        {
            IsConverted = false,
            Location = _configuration["serverHost"] + "/api/File/Uploaded/" + fileName,
            Name = fileName,
            Size = Math.Round(ByteSize.FromBytes(file.Length).MebiBytes, 2),
            UploaderId = uploaderId,
            OriginalName = originalName
        };
        int insertedFileCount = await AddAsync(fileRecord);
        _publisher.AddFileConvertMission(fileRecord);
        return fileRecord;
    }
}