using Microsoft.Office.Interop.Word;
using System.Runtime.InteropServices;
using EveryWhere.FileServer.Utils;

namespace EveryWhere.FileServer.Domain;

[SupportType(".doc")]
[SupportType(".docx")]
public class WordConverter:FileConverter
{
    public override bool ConvertToPdf(string sourcePath, string targetName, out int pageCount)
    {
        bool result;
        Application application = new();
        Document? document = null;
        Documents documents = application.Documents;
        pageCount = -1;
        try
        {
            application.Visible = false;
            document = documents.Open(sourcePath);
            pageCount = document.ComputeStatistics(WdStatistic.wdStatisticPages);
            string pdfPath = Path.Combine(FileUtil.GetWpsFileDirectory().FullName, targetName + ".xps");
            document.ExportAsFixedFormat(pdfPath, WdExportFormat.wdExportFormatXPS);
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
            Marshal.ReleaseComObject(documents);
            Marshal.ReleaseComObject(application);
            #pragma warning restore CA1416 // 验证平台兼容性
        }
        return result;
    }

    public override bool ConvertToXps(string sourcePath, string targetName, out int pageCount)
    {
        bool result;
        Application application = new();
        Document? document = null;
        Documents documents = application.Documents;
        pageCount = -1;
        try
        {
            application.Visible = false;
            document = documents.Open(sourcePath);
            pageCount = document.ComputeStatistics(WdStatistic.wdStatisticPages);
            string xpsPath = Path.Combine(FileUtil.GetWpsFileDirectory().FullName, targetName + ".xps");
            document.ExportAsFixedFormat(xpsPath, WdExportFormat.wdExportFormatXPS);
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
            Marshal.ReleaseComObject(documents);
            Marshal.ReleaseComObject(application);
            #pragma warning restore CA1416 // 验证平台兼容性
        }
        return result;
    }

    public override bool ConvertToFixedFormat(string sourcePath, string targetName, out int pageCount)
    {
        bool result;
        Application application = new();
        Document? document = null;
        Documents documents = application.Documents;
        pageCount = -1;
        try
        {
            application.Visible = false;
            document = documents.Open(sourcePath);
            pageCount = document.ComputeStatistics(WdStatistic.wdStatisticPages);
            string pdfPath = Path.Combine(FileUtil.GetPdfFileDirectory().FullName, targetName + ".pdf");
            string xpsPath = Path.Combine(FileUtil.GetWpsFileDirectory().FullName, targetName + ".xps");
            document.ExportAsFixedFormat(pdfPath, WdExportFormat.wdExportFormatPDF);
            document.ExportAsFixedFormat(xpsPath, WdExportFormat.wdExportFormatXPS);
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
            Marshal.ReleaseComObject(documents);
            Marshal.ReleaseComObject(application);
            #pragma warning restore CA1416 // 验证平台兼容性
        }
        return result;
    }
}