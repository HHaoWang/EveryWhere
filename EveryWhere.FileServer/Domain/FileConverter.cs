namespace EveryWhere.FileServer.Domain;

public abstract class FileConverter
{
    public abstract bool ConvertToPdf(string sourcePath, string targetName, out int pageCount);
    public abstract bool ConvertToXps(string sourcePath, string targetName, out int pageCount);
    public abstract bool ConvertToFixedFormat(string sourcePath, string targetName, out int pageCount);
}