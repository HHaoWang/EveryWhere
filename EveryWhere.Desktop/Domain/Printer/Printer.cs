using EveryWhere.Desktop.Domain.Printer.Win32Api.Dto;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Printing;
using Serilog;
using System.IO;
using System.IO.Packaging;
using System.Windows.Documents;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.Windows.Markup;

namespace EveryWhere.Desktop.Domain.Printer;

public class Printer
{
    #region Fields

    private readonly PrinterInfo? _printerInfo;
    private readonly PrintQueue? _queue;
    private readonly PrintCapabilities? _capabilities;

    public bool IsVirtual = false;
    public string PhysicalName => _printerInfo!.PrinterName;
    public string PrinterName => (IsOffline?"(离线)":"")+_printerInfo!.PrinterName;
    public bool IsOffline => _printerInfo!.isOffLine;
    public bool SupportColor => _capabilities!.OutputColorCapability.Contains(OutputColor.Color);
    public bool SupportDuplex => _capabilities!
        .DuplexingCapability
        .Aggregate(false, (supportDuplex, duplex) 
            => duplex == Duplexing.TwoSidedLongEdge || duplex == Duplexing.TwoSidedShortEdge,
            computedValue=>computedValue);

    public List<PageMediaSizeName> SupportSizeNames =>
        _capabilities!.PageMediaSizeCapability
            .Select(s => s.PageMediaSizeName)
            .Where(s=>s!=null)
            .Select(s=>(PageMediaSizeName)s!)
            .ToList();

    public string DisplayImg => IsOffline ? "/Static/Img/printer-offline.png" : "/Static/Img/printer.png";

    #endregion

    public Printer(){}

    private Printer(PrinterInfo printerInfo)
    {
        _printerInfo = printerInfo;
        _queue = new LocalPrintServer().GetPrintQueue(PhysicalName);
        _capabilities = _queue.GetPrintCapabilities();
    }

    public static List<Printer> GetLocalPrinters()
    {
        List<Printer> localPrinters = new();
        uint cbNeeded = 0;
        uint cReturned = 0;
        //获取一个打印机信息的内存大小
        // ReSharper disable once JoinDeclarationAndInitializer
        bool ret;
        EnumPrinters(PrinterEnumFlags.PRINTER_ENUM_LOCAL, null!, 2, IntPtr.Zero, 0, ref cbNeeded, ref cReturned);
        IntPtr pAddress = Marshal.AllocHGlobal((int)cbNeeded);

        //获取全部打印机信息
        ret = EnumPrinters(PrinterEnumFlags.PRINTER_ENUM_LOCAL, null!, 2, pAddress, cbNeeded, ref cbNeeded, ref cReturned);
        if (!ret) return localPrinters;

        PrinterInfo[] printerInfos = new PrinterInfo[cReturned];
        long offset = pAddress.ToInt64();
        for (int i = 0; i < cReturned; i++)
        {
            PrinterInfo2 t = Marshal.PtrToStructure<PrinterInfo2>(new IntPtr(offset))!;
            printerInfos[i] = new PrinterInfo(t);
            offset += Marshal.SizeOf(typeof(PrinterInfo2));
            printerInfos[i].devMode = Marshal.PtrToStructure<DevMode>(printerInfos[i].pDevMode);
            localPrinters.Add(new Printer(printerInfos[i]));
        }
        Marshal.FreeHGlobal(pAddress);
        return localPrinters;
    }

    public void PrintXps(FileInfo file,int pageStart,int pageEnd,int count,bool color,bool duplex,string size)
    {
        try
        {
            PrintTicket ticket = _queue!.DefaultPrintTicket;

            //逐份打印
            if (_capabilities?.CollationCapability.Contains(Collation.Collated) == true)
            {
                ticket.Collation = Collation.Collated;
            }

            //彩印
            if (duplex && _capabilities?.DuplexingCapability.Contains(Duplexing.TwoSidedLongEdge) == true)
            {
                ticket.Duplexing = Duplexing.TwoSidedLongEdge;
            }
            else
            {
                ticket.Duplexing = Duplexing.OneSided;
            }

            //双页打印
            if (color && _capabilities?.OutputColorCapability.Contains(OutputColor.Color) == true)
            {
                ticket.OutputColor = OutputColor.Color;
            }
            else
            {
                ticket.OutputColor = OutputColor.Grayscale;
            }

            //份数
            ticket.CopyCount = count;

            //纸张大小
            switch (size)
            {
                case "A4":
                    ticket.PageMediaSize =
                        _capabilities?.PageMediaSizeCapability.First(
                            p => p.PageMediaSizeName == PageMediaSizeName.ISOA4);
                    break;
                case "A3":
                    ticket.PageMediaSize =
                        _capabilities?.PageMediaSizeCapability.First(
                            p => p.PageMediaSizeName == PageMediaSizeName.ISOA3);
                    break;
                default:
                    break;
            }

            //打印范围
            SplitXpsIntoTemp(file,pageStart,pageEnd);

            _queue?.AddJob(file.Name, file.FullName + ".temp", false, new PrintTicket());

            XpsDocumentWriter xpsDocumentWriter = PrintQueue.CreateXpsDocumentWriter(_queue);
            xpsDocumentWriter.Write(file.FullName);
        }
        catch (PrintJobException e)
        {
            Debug.WriteLine(e.Message);
        }
    }

    public static void SplitXpsIntoTemp(FileInfo file, int pageStart, int pageEnd)
    {
        List<PageContent> pages = new();

        XpsDocument document = new(file.FullName, FileAccess.Read);
        FixedDocumentSequence? doc = document.GetFixedDocumentSequence();
        PageContentCollection? oldPages = doc.References.First().GetDocument(false)?.Pages;

        if (oldPages == null)
        {
            return;
        }

        for (int i = 1; i < oldPages.Count + 1; i++)
        {
            if (i < pageStart || i > pageEnd)
            {
                continue;
            }
            pages.Add(oldPages[i]);
        }

        string savePath = file.FullName + ".temp";
        using XpsDocument xpsOutputDoc = new(savePath, FileAccess.Write);
        FixedDocumentSequence fixedDocSequence = new();
        DocumentReference docRef = new();

        FixedDocument fixedDoc = new();
        docRef.SetDocument(fixedDoc);
        foreach (PageContent pageItem in pages)
        {
            PageContent pageContent = new()
            {
                Source = pageItem.Source
            };
            (pageContent as IUriContext).BaseUri = ((IUriContext)pageItem).BaseUri;
            pageContent.GetPageRoot(true);
            fixedDoc.Pages.Add(pageContent);
        }

        docRef.GetDocument(true);
        fixedDocSequence.References.Add(docRef);

        XpsDocumentWriter xpsDocWriter = XpsDocument.CreateXpsDocumentWriter(xpsOutputDoc);
        xpsDocWriter.Write(fixedDocSequence);
        xpsOutputDoc.Close();
    }

    #region 引用 WindowsAPI

    // 引用API声明
    // ReSharper disable once StringLiteralTypo
    [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    // ReSharper disable once MemberCanBePrivate.Global
    public static extern bool EnumPrinters(
        PrinterEnumFlags Flags,
        string Name,
        uint Level,
        IntPtr pPrinterEnum,
        uint cbBuf,
        ref uint pcbNeeded,
        ref uint pcReturned
    );
    #endregion

    #region 内部类型

    

    #endregion
}