using System.Runtime.InteropServices;
using EveryWhere.FileServer.Utils;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;

namespace EveryWhere.FileServer.Domain;

[SupportType(".ppt")]
[SupportType(".pptx")]
public class PowerPointConverter:FileConverter
{
    public override bool ConvertToPdf(string sourcePath, string targetName, out int pageCount)
    {
        bool result;
        Application application = new();
        Presentation? document = null;
        pageCount = -1;
        try
        {
            document = application.Presentations.Open(sourcePath, ReadOnly: MsoTriState.msoTrue, WithWindow: MsoTriState.msoFalse);
            pageCount = document.Slides.Count;
            string pdfPath = Path.Combine(FileUtil.GetPdfFileDirectory().FullName, targetName + ".pdf");
            document.ExportAsFixedFormat(pdfPath, PpFixedFormatType.ppFixedFormatTypePDF);
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

    public override bool ConvertToXps(string sourcePath, string targetName, out int pageCount)
    {
        bool result;
        Application application = new();
        Presentation? document = null;
        pageCount = -1;
        try
        {
            document = application.Presentations.Open(sourcePath,ReadOnly:MsoTriState.msoTrue, WithWindow: MsoTriState.msoFalse);
            pageCount = document.Slides.Count;
            string xpsPath = Path.Combine(FileUtil.GetXpsFileDirectory().FullName, targetName + ".xps");
            document.ExportAsFixedFormat(xpsPath,PpFixedFormatType.ppFixedFormatTypeXPS);
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

    public override bool ConvertToFixedFormat(string sourcePath, string targetName, out int pageCount)
    {
        bool result;
        Application application = new();
        Presentation? document = null;
        pageCount = -1;
        try
        {
            document = application.Presentations.Open(sourcePath, ReadOnly: MsoTriState.msoTrue, WithWindow: MsoTriState.msoFalse);
            pageCount = document.Slides.Count;
            string xpsPath = Path.Combine(FileUtil.GetXpsFileDirectory().FullName, targetName + ".xps");
            document.ExportAsFixedFormat(xpsPath, PpFixedFormatType.ppFixedFormatTypeXPS);
            string pdfPath = Path.Combine(FileUtil.GetPdfFileDirectory().FullName, targetName + ".pdf");
            document.ExportAsFixedFormat(pdfPath, PpFixedFormatType.ppFixedFormatTypePDF);
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