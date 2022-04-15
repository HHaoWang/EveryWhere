namespace EveryWhere.FileServer.Domain;

[SupportType("xls")]
[SupportType("xlsx")]
public class ExcelConverter:FileConverter
{
    public override bool ConvertToPdf(string sourcePath, string targetName, out int pageCount)
    {
        throw new NotImplementedException();
    }

    public override bool ConvertToXps(string sourcePath, string targetName, out int pageCount)
    {
        throw new NotImplementedException();
    }

    public override bool ConvertToFixedFormat(string sourcePath, string targetName, out int pageCount)
    {
        throw new NotImplementedException();
    }
}