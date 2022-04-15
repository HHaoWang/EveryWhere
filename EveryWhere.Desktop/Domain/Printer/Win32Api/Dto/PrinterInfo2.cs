using System;
using System.Runtime.InteropServices;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
#pragma warning disable CS8618

namespace EveryWhere.Desktop.Domain.Printer.Win32Api.Dto;

[StructLayout(LayoutKind.Sequential)]
public class PrinterInfo2
{
    [MarshalAs(UnmanagedType.LPTStr)]
    public string pServerName;
    [MarshalAs(UnmanagedType.LPTStr)]
    public string pPrinterName;
    [MarshalAs(UnmanagedType.LPTStr)]
    public string pShareName;
    [MarshalAs(UnmanagedType.LPTStr)]
    public string pPortName;
    [MarshalAs(UnmanagedType.LPTStr)]
    public string pDriverName;
    [MarshalAs(UnmanagedType.LPTStr)]
    public string pComment;
    [MarshalAs(UnmanagedType.LPTStr)]
    public string pLocation;

    public IntPtr pDevMode;
    [MarshalAs(UnmanagedType.LPTStr)]
    public string pSepFile;
    [MarshalAs(UnmanagedType.LPTStr)]
    public string pPrintProcessor;
    [MarshalAs(UnmanagedType.LPTStr)]
    public string pDatatype;
    [MarshalAs(UnmanagedType.LPTStr)]
    public string pParameters;
    public IntPtr pSecurityDescriptor;
    public PrinterAttribute Attributes { set; get; }
    public uint Priority;
    public uint DefaultPriority;
    public uint StartTime;
    public uint UntilTime;
    public uint Status;
    public uint cJobs;
    public uint AveragePPM;
}

public class PrinterInfo:PrinterInfo2
{
    public PrinterInfo(PrinterInfo2 info2)
    {
        Attributes = info2.Attributes;
        pServerName = info2.pServerName;
        pPrinterName = info2.pPrinterName;
        pShareName = info2.pShareName;
        pPortName = info2.pPortName; 
        pDriverName = info2.pDriverName;
        pComment = info2.pComment;
        pLocation = info2.pLocation;
        pDevMode = info2.pDevMode;
        pSepFile = info2.pSepFile;
        pPrintProcessor = info2.pPrintProcessor;
        pDatatype = info2.pDatatype;
        pParameters = info2.pParameters;
        pSecurityDescriptor = info2.pSecurityDescriptor;
        Attributes = info2.Attributes;
        Priority = info2.Priority;
        DefaultPriority = info2.DefaultPriority;
        StartTime = info2.StartTime;
        UntilTime = info2.UntilTime ;
        Status = info2.Status;
        cJobs = info2.cJobs;
        AveragePPM = info2.AveragePPM;

        devMode = Marshal.PtrToStructure<DevMode>(pDevMode);
    }

    public bool isOffLine => (Attributes & PrinterAttribute.PRINTER_ATTRIBUTE_WORK_OFFLINE) > 0;
    public DevMode devMode { get; set; }
    public string PrinterName => pPrinterName;
}

[Flags]
public enum PrinterEnumFlags
{
    PRINTER_ENUM_DEFAULT = 0x00000001,
    PRINTER_ENUM_LOCAL = 0x00000002,
    PRINTER_ENUM_CONNECTIONS = 0x00000004,
    PRINTER_ENUM_FAVORITE = 0x00000004,
    PRINTER_ENUM_NAME = 0x00000008,
    PRINTER_ENUM_REMOTE = 0x00000010,
    PRINTER_ENUM_SHARED = 0x00000020,
    PRINTER_ENUM_NETWORK = 0x00000040,
    PRINTER_ENUM_EXPAND = 0x00004000,
    PRINTER_ENUM_CONTAINER = 0x00008000,
    PRINTER_ENUM_ICONMASK = 0x00ff0000,
    PRINTER_ENUM_ICON1 = 0x00010000,
    PRINTER_ENUM_ICON2 = 0x00020000,
    PRINTER_ENUM_ICON3 = 0x00040000,
    PRINTER_ENUM_ICON4 = 0x00080000,
    PRINTER_ENUM_ICON5 = 0x00100000,
    PRINTER_ENUM_ICON6 = 0x00200000,
    PRINTER_ENUM_ICON7 = 0x00400000,
    PRINTER_ENUM_ICON8 = 0x00800000,
    PRINTER_ENUM_HIDE = 0x01000000
}

/// <summary>
/// The printer attributes.
/// </summary>
/// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/dd162845(v=vs.85).aspx"/>
[Flags]
public enum PrinterAttribute
{
    /// <summary>
    /// No attribute set.
    /// </summary>
    PRINTER_ATTRIBUTE_NONE = 0,

    /// <summary>
    /// If set, the printer spools and starts printing after the last page is spooled.
    /// If not set and PRINTER_ATTRIBUTE_DIRECT is not set, the printer spools and
    /// prints while spooling.
    /// </summary>
    PRINTER_ATTRIBUTE_QUEUED = 0x1,

    /// <summary>
    /// Job is sent directly to the printer (it is not spooled).
    /// </summary>
    PRINTER_ATTRIBUTE_DIRECT = 0x2,

    /// <summary>
    /// Printer is default printer.
    /// </summary>
    PRINTER_ATTRIBUTE_DEFAULT = 0x4,

    /// <summary>
    /// Printer is shared.
    /// </summary>
    PRINTER_ATTRIBUTE_SHARED = 0x8,

    /// <summary>
    /// Printer is a network printer connection.
    /// </summary>
    PRINTER_ATTRIBUTE_NETWORK = 0x10,

    /// <summary>
    /// Reserved for future use.
    /// </summary>
    PRINTER_ATTRIBUTE_HIDDEN = 0x20,

    /// <summary>
    /// Printer is a local printer.
    /// </summary>
    PRINTER_ATTRIBUTE_LOCAL = 0x40,

    /// <summary>
    /// If set, DevQueryPrint is called. DevQueryPrint may fail if the document
    /// and printer setups do not match. Setting this flag causes mismatched
    /// documents to be held in the queue.
    /// </summary>
    PRINTER_ATTRIBUTE_ENABLE_DEVQ = 0x80,

    /// <summary>
    /// If set, jobs are kept after they are printed. If unset, jobs are deleted.
    /// </summary>
    PRINTER_ATTRIBUTE_KEEPPRINTEDJOBS = 0x100,

    /// <summary>
    /// If set and printer is set for print-while-spooling, any jobs that have
    /// completed spooling are scheduled to print before jobs that have not
    /// completed spooling.
    /// </summary>
    PRINTER_ATTRIBUTE_DO_COMPLETE_FIRST = 0x200,

    /// <summary>
    /// No description.
    /// </summary>
    PRINTER_ATTRIBUTE_WORK_OFFLINE = 0x400,

    /// <summary>
    /// No description.
    /// </summary>
    PRINTER_ATTRIBUTE_ENABLE_BIDI = 0x800,

    /// <summary>
    /// Indicates that only raw data type print jobs can be spooled.
    /// </summary>
    PRINTER_ATTRIBUTE_RAW_ONLY = 0x1000,

    /// <summary>
    /// Indicates whether the printer is published in the directory service.
    /// </summary>
    PRINTER_ATTRIBUTE_PUBLISHED = 0x2000,

    /// <summary>
    /// <note>In Windows XP and later versions of Windows:</note>
    /// If set, printer is a fax printer. This can only be set by AddPrinter,
    /// but it can be retrieved by EnumPrinters and GetPrinter.
    /// </summary>
    PRINTER_ATTRIBUTE_FAX = 0x4000,

    /// <summary>
    /// <note>In Windows Server 2003:</note>
    /// Indicates the printer is currently connected through a terminal server.
    /// </summary>
    PRINTER_ATTRIBUTE_TS = 0x8000,

    /// <summary>
    /// <note>In Windows Vista and later versions of Windows:</note>
    /// The printer was installed by using the Push Printer Connections
    /// user policy. See Print Management Step-by-Step Guide.
    /// </summary>
    PRINTER_ATTRIBUTE_PUSHED_USER = 0x20000,

    /// <summary>
    /// <note>In Windows Vista and later versions of Windows:</note>
    /// The printer was installed by using the Push Printer Connections
    /// computer policy. See Print Management Step-by-Step Guide.
    /// </summary>
    PRINTER_ATTRIBUTE_PUSHED_MACHINE = 0x40000,

    /// <summary>
    /// <note>In Windows Vista and later versions of Windows:</note>
    /// Printer is a per-machine connection.
    /// </summary>
    PRINTER_ATTRIBUTE_MACHINE = 0x0000,

    /// <summary>
    /// <note>In Windows Vista and later versions of Windows:</note>
    /// A computer has connected to this printer and given it a friendly name.
    /// </summary>
    PRINTER_ATTRIBUTE_FRIENDLY_NAME = 0x100000,

    /// <summary>
    /// <note>In Windows Vista and later versions of Windows:</note>
    /// No description.
    /// </summary>
    PRINTER_ATTRIBUTE_TS_GENERIC_DRIVER = 0x200000,
}