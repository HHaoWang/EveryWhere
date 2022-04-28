﻿using EveryWhere.Desktop.Domain.Printer.Win32Api.Dto;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Printing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.Windows.Markup;

namespace EveryWhere.Desktop.Domain.Printer;

public sealed class Printer:INotifyPropertyChanged
{
    #region Fields

    private readonly PrintQueue? _queue;
    private readonly PrintCapabilities? _capabilities;
    private readonly List<int> _jobIdList = new();
    private readonly Dictionary<int, PrintSystemJobInfo> _printingJobs = new();

    public int? Id { get; set; } = -1;
    public bool IsVirtual = false;
    public string PhysicalName { get; }
    public string PrinterName { get; set; }
    public string DisplayName => (IsOffline ? "(离线)" : "") + PrinterName;
    public bool IsOffline { get; private set; }
    public bool SupportColor => _capabilities!.OutputColorCapability.Contains(OutputColor.Color);
    public bool SupportDuplex => _capabilities!
        .DuplexingCapability
        .Aggregate(false, (_, duplex) 
            => duplex == Duplexing.TwoSidedLongEdge || duplex == Duplexing.TwoSidedShortEdge,
            computedValue=>computedValue);

    public List<PageMediaSizeName> SupportSizeNames =>
        _capabilities!.PageMediaSizeCapability
            .Select(s => s.PageMediaSizeName)
            .Where(s=>s!=null)
            .Select(s=>(PageMediaSizeName)s!)
            .ToList();

    public string DisplayImg => IsOffline ? "/Static/Img/printer-offline.png" : "/Static/Img/printer.png";

    public event EventHandler<int>? OnJobFinished; 

    #endregion

#pragma warning disable CS8618
    public Printer(){}
#pragma warning restore CS8618

    private Printer(PrinterInfo printerInfo)
    {
        IsOffline = printerInfo.isOffLine;
        PhysicalName = printerInfo.PrinterName;
        PrinterName = printerInfo.PrinterName;
        _queue = new LocalPrintServer().GetPrintQueue(PhysicalName);
        _capabilities = _queue.GetPrintCapabilities();
    }

    public void Refresh()
    {
        if (IsVirtual)
        {
            return;
        }
        Printer? newPrinter = GetLocalPrinters().Find(p => p.PhysicalName == PhysicalName);
        IsOffline = newPrinter is not {IsOffline: false};
        OnPropertyChanged(nameof(IsOffline));
        OnPropertyChanged(nameof(DisplayName));
        OnPropertyChanged(nameof(DisplayImg));
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

    public bool ExistsJob(int jobId)
    {
        return _jobIdList.Contains(jobId) || _printingJobs.ContainsKey(jobId);
    }

    public void AddPrintJob(FileInfo file, int pageStart, int pageEnd, int count, bool color, bool duplex, string size,int jobId)
    {
        if (_jobIdList.Contains(jobId))
        {
            return;
        }
        _jobIdList.Add(jobId);
        PrintXps(file,pageStart,pageEnd,count,color,duplex,size);
        DateTime submitTime = DateTime.Now.ToUniversalTime();
        PrintJobInfoCollection? infoCollection = _queue!.GetPrintJobInfoCollection();
        foreach (PrintSystemJobInfo jobInfo in infoCollection)
        {
            if (jobInfo.TimeJobSubmitted.Subtract(submitTime) < TimeSpan.FromSeconds(3) && jobInfo.NumberOfPages == pageEnd - pageStart + 1)
            {
                _printingJobs.Add(jobId,jobInfo);
            }
        }
    }

    /// <summary>
    /// 打印XPS文件
    /// </summary>
    /// <param name="file">XPS文件</param>
    /// <param name="pageStart">起始页</param>
    /// <param name="pageEnd">结束页</param>
    /// <param name="count">份数</param>
    /// <param name="color">是否彩印</param>
    /// <param name="duplex">是否正反印刷</param>
    /// <param name="size">纸张大小</param>
    private void PrintXps(FileInfo file,int pageStart,int pageEnd,int count,bool color,bool duplex,string size)
    {
        try
        {
            PrintTicket ticket = _queue!.DefaultPrintTicket;

            //逐份打印
            if (_capabilities?.CollationCapability.Contains(Collation.Collated) == true)
            {
                ticket.Collation = Collation.Collated;
            }

            //双页打印
            if (duplex && _capabilities?.DuplexingCapability.Contains(Duplexing.TwoSidedLongEdge) == true)
            {
                ticket.Duplexing = Duplexing.TwoSidedLongEdge;
            }
            else
            {
                ticket.Duplexing = Duplexing.OneSided;
            }

            //彩印
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
                        _capabilities?.PageMediaSizeCapability.FirstOrDefault(
                            p => p.PageMediaSizeName == PageMediaSizeName.ISOA4) ?? ticket.PageMediaSize;
                    break;
                case "A3":
                    ticket.PageMediaSize =
                        _capabilities?.PageMediaSizeCapability.FirstOrDefault(
                            p => p.PageMediaSizeName == PageMediaSizeName.ISOA3) ?? ticket.PageMediaSize;
                    break;
            }

            //打印范围
            SplitXpsIntoTemp(file,pageStart,pageEnd);

            XpsDocument xpsFile = new(file.FullName + ".temp", FileAccess.Read);

            XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(_queue);
            writer.Write(xpsFile.GetFixedDocumentSequence(),ticket);

            //PrintSystemJobInfo? jobInfo = _queue?.AddJob(file.Name, file.FullName + ".temp", _queue.IsXpsDevice);
            Debug.WriteLine("添加打印任务到队列完成");

            //if (jobInfo==null)
            //{
            //    Debug.WriteLine("添加打印任务失败！");
            //    return;
            //}

            //jobInfo.Refresh();
            //JobInfos.Add(jobInfo);
        }
        catch (PrintJobException e)
        {
            Debug.WriteLine("添加打印任务到队列失败！");
            Debug.WriteLine(e.Message);
        }
    }

    /// <summary>
    /// 提取xps文件中的指定页，以原文件名加“.temp”保存在原路径中
    /// </summary>
    /// <param name="file">要提取的XPS文件</param>
    /// <param name="pageStart">起始页</param>
    /// <param name="pageEnd">结束页</param>
    private static void SplitXpsIntoTemp(FileInfo file, int pageStart, int pageEnd)
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
            pages.Add(oldPages[i-1]);
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

    public void CheckJobsState()
    {
        foreach (KeyValuePair<int, PrintSystemJobInfo> pair in _printingJobs)
        {
            pair.Value.Refresh();
            if (!pair.Value.IsDeleted && !pair.Value.IsCompleted) continue;
            Debug.WriteLine("检测到打印任务已完成");
            OnJobFinished?.Invoke(this, pair.Key);
        }
    }

    public void RemoveJob(int jobId)
    {
        _printingJobs.Remove(jobId);
        _jobIdList.Remove(jobId);
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

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}