using EveryWhere.Util;

namespace EveryWhere.FileServer.Contexts.FileProvider;

public static class PhysicalFileService
{
    public static FileInfo SaveFileToUploadDir(Stream fileStream, string originalName)
    {
        string extension = Path.GetExtension(originalName);
        string fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-") + Path.GetRandomFileName() + extension;
        string fullFileName = Path.Combine(FileUtil.GetFileDirectory().FullName, fileName);

        using var stream = new FileStream(fullFileName, FileMode.Create);
        fileStream.CopyTo(stream);
        return new FileInfo(fullFileName);
    }
}