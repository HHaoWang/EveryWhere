using EveryWhere.Desktop.Domain.Printer.Win32Api.Dto;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Printing;

namespace EveryWhere.Desktop.Domain.Printer;

public class Printer
{
    private readonly PrinterInfo _printerInfo;
    private readonly PrintQueue _queue;
    private readonly PrintCapabilities _capabilities;

    public string PrinterName => _printerInfo.PrinterName;
    public bool IsOffline => _printerInfo.isOffLine;
    public bool SupportColor => _capabilities.OutputColorCapability.Contains(OutputColor.Color);
    public bool SupportDuplex => _capabilities
        .DuplexingCapability
        .Aggregate(false, (supportDuplex, duplex) 
            => duplex == Duplexing.TwoSidedLongEdge || duplex == Duplexing.TwoSidedShortEdge,
            computedValue=>computedValue);

    public List<PageMediaSizeName> SupportSizeNames =>
        _capabilities.PageMediaSizeCapability
            .Select(s => s.PageMediaSizeName)
            .Where(s=>s!=null)
            .Select(s=>(PageMediaSizeName)s!)
            .ToList();

    private Printer(PrinterInfo printerInfo)
    {
        _printerInfo = printerInfo;
        _queue = new LocalPrintServer().GetPrintQueue(PrinterName);
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