using EveryWhere.Desktop.Domain.Printer.Win32Api.Dto;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EveryWhere.Desktop.Domain.Printer;

public class Printer
{
    public readonly PrinterInfo PrinterInfo;
    public string PrinterName => PrinterInfo.PrinterName;
    public bool IsOffline => PrinterInfo.isOffLine;

    public Printer(PrinterInfo printerInfo)
    {
        PrinterInfo = printerInfo;
    }

    public static List<Printer> GetLocalPrinters()
    {
        List<Printer> localPrinters = new();
        PrinterInfo[] printerInfos;
        uint cbNeeded = 0;
        uint cReturned = 0;
        //获取一个打印机信息的内存大小
        bool ret = EnumPrinters(PrinterEnumFlags.PRINTER_ENUM_LOCAL, null, 2, IntPtr.Zero, 0, ref cbNeeded, ref cReturned);
        IntPtr pAddr = Marshal.AllocHGlobal((int)cbNeeded);

        //获取全部打印机信息
        ret = EnumPrinters(PrinterEnumFlags.PRINTER_ENUM_LOCAL, null, 2, pAddr, cbNeeded, ref cbNeeded, ref cReturned);
        if (!ret) return localPrinters;

        printerInfos = new PrinterInfo[cReturned];
        long offset = pAddr.ToInt64();
        for (int i = 0; i < cReturned; i++)
        {
            PrinterInfo2 t = Marshal.PtrToStructure<PrinterInfo2>(new IntPtr(offset))!;
            printerInfos[i] = new PrinterInfo(t);
            offset += Marshal.SizeOf(typeof(PrinterInfo2));
            printerInfos[i].devMode = Marshal.PtrToStructure<DevMode>(printerInfos[i].pDevMode);
            localPrinters.Add(new Printer(printerInfos[i]));
        }
        Marshal.FreeHGlobal(pAddr);
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
}