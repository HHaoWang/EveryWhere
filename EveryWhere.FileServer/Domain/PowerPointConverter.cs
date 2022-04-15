namespace EveryWhere.FileServer.Domain;

[SupportType("ppt")]
[SupportType("pptx")]
public class PowerPointConverter:FileConverter
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