using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using EveryWhere.FileServer.Utils;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace EveryWhere.FileServer.Domain;

[SupportType(".xls")]
[SupportType(".xlsx")]
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
        bool result;
        Application application = new();
        Workbook? document = null;
        pageCount = -1;
        try
        {
            application.Visible = false;
            document = application.Workbooks.Open(sourcePath);

            string pdfPath = Path.Combine(FileUtil.GetPdfFileDirectory().FullName, targetName + ".pdf");
            document.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, pdfPath);
            string xpsPath = Path.Combine(FileUtil.GetXpsFileDirectory().FullName, targetName + ".xps");
            document.ExportAsFixedFormat(XlFixedFormatType.xlTypeXPS, xpsPath);

            PdfDocument pdfDocument = PdfReader.Open(pdfPath);
            pageCount = pdfDocument.PageCount;
            pdfDocument.Close();

            result = true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            result = false;
        }
        finally
        {
            document?.Close();
            application.Quit();
#pragma warning disable CA1416 // 验证平台兼容性
            if (document != null)
            {
                Marshal.ReleaseComObject(document);
            }
            Marshal.ReleaseComObject(application);
#pragma warning restore CA1416 // 验证平台兼容性
        }
        return result;
    }
}