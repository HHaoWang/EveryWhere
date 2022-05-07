using EveryWhere.FileServer.Utils;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace EveryWhere.FileServer.Domain;

public class PdfConverter:FileConverter
{
    public override bool ConvertToPdf(string sourcePath, string targetName, out int pageCount)
    {
        try
        {
            string pdfPath = Path.Combine(FileUtil.GetPdfFileDirectory().FullName, targetName + ".pdf");
            File.Copy(sourcePath,pdfPath,true);
            PdfDocument pdfDocument = PdfReader.Open(pdfPath);
            pageCount = pdfDocument.PageCount;
            return true;
        }
        catch (Exception)
        {
            pageCount = -1;
            return false;
        }
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