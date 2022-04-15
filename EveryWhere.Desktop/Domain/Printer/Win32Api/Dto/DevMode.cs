using System;
using System.Runtime.InteropServices;
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace EveryWhere.Desktop.Domain.Printer.Win32Api.Dto;
//[StructLayout(LayoutKind.Explicit,Pack =1, CharSet = CharSet.Unicode)]
//public struct DevMode
//{
//    public const int CCHDEVICENAME = 32;
//    public const int CCHFORMNAME = 32;

//    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
//    [FieldOffset(0)]
//    public string dmDeviceName;
//    [FieldOffset(32)]
//    public ushort dmSpecVersion;
//    [FieldOffset(34)]
//    public ushort dmDriverVersion;
//    [FieldOffset(36)]
//    public ushort dmSize;
//    [FieldOffset(38)]
//    public ushort dmDriverExtra;
//    [FieldOffset(40)]
//    public uint dmFields;

//    [FieldOffset(44)]
//    public short dmOrientation;
//    [FieldOffset(46)]
//    public PaperSize dmPaperSize;
//    [FieldOffset(48)]
//    public short dmPaperLength;
//    [FieldOffset(50)]
//    public short dmPaperWidth;
//    [FieldOffset(52)]
//    public short dmScale;
//    [FieldOffset(54)]
//    public short dmCopies;
//    [FieldOffset(56)]
//    public short dmDefaultSource;
//    [FieldOffset(58)]
//    public short dmPrintQuality;

//    [FieldOffset(44)]
//    public POINTL dmPosition;
//    [FieldOffset(52)]
//    public uint dmDisplayOrientation;
//    [FieldOffset(56)]
//    public uint dmDisplayFixedOutput;

//    [FieldOffset(60)]
//    public DMCOLOR dmColor;
//    [FieldOffset(62)]
//    public DMDUP dmDuplex;
//    [FieldOffset(64)]
//    public short dmYResolution;
//    [FieldOffset(66)]
//    public short dmTTOption;
//    [FieldOffset(68)]
//    public DMCOLLATE dmCollate;

//    [FieldOffset(72)]
//    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
//    public string dmFormName;
//    [FieldOffset(104)]
//    public ushort dmLogPixels;
//    [FieldOffset(106)]
//    public uint dmBitsPerPel;
//    [FieldOffset(112)]
//    public uint dmPelsWidth;
//    [FieldOffset(116)]
//    public uint dmPelsHeight;

//    [FieldOffset(120)]
//    public uint dmDisplayFlags;
//    [FieldOffset(120)]
//    public uint dmNup;

//    [FieldOffset(124)]
//    public uint dmDisplayFrequency;
//    [FieldOffset(128)]
//    public uint dmICMMethod;
//    [FieldOffset(132)]
//    public uint dmICMIntent;
//    [FieldOffset(136)]
//    public uint dmMediaType;
//    [FieldOffset(140)]
//    public uint dmDitherType;
//    [FieldOffset(144)]
//    public uint dmReserved1;
//    [FieldOffset(148)]
//    public uint dmReserved2;
//    [FieldOffset(150)]
//    public uint dmPanningWidth;
//    [FieldOffset(154)]
//    public uint dmPanningHeight;
//}

[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Auto)]
public struct DevMode
{
    private const int CCHDEVICENAME2 = 32;
    private const int CCHFORMNAME2 = 32;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME2)]
    public string dmDeviceName;
    public short dmSpecVersion;
    public short dmDriverVersion;
    public short dmSize;
    public short dmDriverExtra;
    public int dmFields;

    public short dmOrientation;
    public PaperSize dmPaperSize;
    public short dmPaperLength;
    public short dmPaperWidth;
    public short dmScale;
    public short dmCopies;
    public short dmDefaultSource;
    public short dmPrintQuality;

    public POINTL dmPosition;
    public int dmDisplayOrientation;
    public int dmDisplayFixedOutput;

    public DMCOLOR dmColor;
    public DMDUP dmDuplex;
    public short dmYResolution;
    public short dmTTOption;
    public DMCOLLATE dmCollate;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME2)]
    public string dmFormName;
    public short dmLogPixels;
    public int dmBitsPerPel;
    public int dmPelsWidth;
    public int dmPelsHeight;
    public int dmDisplayFlags;
    //public int dmNup;
    public int dmDisplayFrequency;
}

public struct POINTL
{
    public int x;
    public int y;
}

/// <summary>
/// Selects duplex or double-sided printing for printers capable of duplex printing.
/// </summary>
public enum DMDUP : short
{
    /// <summary>
    /// Unknown setting.
    /// </summary>
    DMDUP_UNKNOWN = 0,

    /// <summary>
    /// Normal (nonduplex) printing.
    /// </summary>
    DMDUP_SIMPLEX = 1,

    /// <summary>
    /// Long-edge binding, that is, the long edge of the page is vertical.
    /// </summary>
    DMDUP_VERTICAL = 2,

    /// <summary>
    /// Short-edge binding, that is, the long edge of the page is horizontal.
    /// </summary>
    DMDUP_HORIZONTAL = 3,
}

/// <summary>
/// Specifies whether collation should be used when printing multiple copies.
/// </summary>
public enum DMCOLLATE : short
{
    /// <summary>
    /// Do not collate when printing multiple copies.
    /// </summary>
    DMCOLLATE_FALSE = 0,

    /// <summary>
    /// Collate when printing multiple copies.
    /// </summary>
    DMCOLLATE_TRUE = 1
}

/// <summary>
/// Switches between color and monochrome on color printers.
/// </summary>
public enum DMCOLOR : short
{
    DMCOLOR_UNKNOWN = 0,

    DMCOLOR_MONOCHROME = 1,

    DMCOLOR_COLOR = 2
}

[Flags]
public enum PaperSize:short
{
    /* only after windows XP */
    DMPAPER_FIRST                         = DMPAPER_LETTER,
    DMPAPER_LETTER                        = 1  /* Letter 8 1/2 x 11 in               */,
    DMPAPER_LETTERSMALL                   = 2  /* Letter Small 8 1/2 x 11 in         */,
    DMPAPER_TABLOID                       = 3  /* Tabloid 11 x 17 in                 */,
    DMPAPER_LEDGER                        = 4  /* Ledger 17 x 11 in                  */,
    DMPAPER_LEGAL                         = 5  /* Legal 8 1/2 x 14 in                */,
    DMPAPER_STATEMENT                     = 6  /* Statement 5 1/2 x 8 1/2 in         */,
    DMPAPER_EXECUTIVE                     = 7  /* Executive 7 1/4 x 10 1/2 in        */,
    DMPAPER_A3                            = 8  /* A3 297 x 420 mm                    */,
    DMPAPER_A4                            = 9  /* A4 210 x 297 mm                    */,
    DMPAPER_A4SMALL                       = 10  /* A4 Small 210 x 297 mm              */,
    DMPAPER_A5                            = 11  /* A5 148 x 210 mm                    */,
    DMPAPER_B4                            = 12  /* B4 (JIS) 250 x 354                 */,
    DMPAPER_B5                            = 13  /* B5 (JIS) 182 x 257 mm              */,
    DMPAPER_FOLIO                         = 14  /* Folio 8 1/2 x 13 in                */,
    DMPAPER_QUARTO                        = 15  /* Quarto 215 x 275 mm                */,
    DMPAPER_10X14                         = 16  /* 10x14 in                           */,
    DMPAPER_11X17                         = 17  /* 11x17 in                           */,
    DMPAPER_NOTE                          = 18  /* Note 8 1/2 x 11 in                 */,
    DMPAPER_ENV_9                         = 19  /* Envelope #9 3 7/8 x 8 7/8          */,
    DMPAPER_ENV_10                        = 20  /* Envelope #10 4 1/8 x 9 1/2         */,
    DMPAPER_ENV_11                        = 21  /* Envelope #11 4 1/2 x 10 3/8        */,
    DMPAPER_ENV_12                        = 22  /* Envelope #12 4 \276 x 11           */,
    DMPAPER_ENV_14                        = 23  /* Envelope #14 5 x 11 1/2            */,
    DMPAPER_CSHEET                        = 24  /* C size sheet                       */,
    DMPAPER_DSHEET                        = 25  /* D size sheet                       */,
    DMPAPER_ESHEET                        = 26  /* E size sheet                       */,
    DMPAPER_ENV_DL                        = 27  /* Envelope DL 110 x 220mm            */,
    DMPAPER_ENV_C5                        = 28  /* Envelope C5 162 x 229 mm           */,
    DMPAPER_ENV_C3                        = 29  /* Envelope C3  324 x 458 mm          */,
    DMPAPER_ENV_C4                        = 30  /* Envelope C4  229 x 324 mm          */,
    DMPAPER_ENV_C6                        = 31  /* Envelope C6  114 x 162 mm          */,
    DMPAPER_ENV_C65                       = 32  /* Envelope C65 114 x 229 mm          */,
    DMPAPER_ENV_B4                        = 33  /* Envelope B4  250 x 353 mm          */,
    DMPAPER_ENV_B5                        = 34  /* Envelope B5  176 x 250 mm          */,
    DMPAPER_ENV_B6                        = 35  /* Envelope B6  176 x 125 mm          */,
    DMPAPER_ENV_ITALY                     = 36  /* Envelope 110 x 230 mm              */,
    DMPAPER_ENV_MONARCH                   = 37  /* Envelope Monarch 3.875 x 7.5 in    */,
    DMPAPER_ENV_PERSONAL                  = 38  /* 6 3/4 Envelope 3 5/8 x 6 1/2 in    */,
    DMPAPER_FANFOLD_US                    = 39  /* US Std Fanfold 14 7/8 x 11 in      */,
    DMPAPER_FANFOLD_STD_GERMAN            = 40  /* German Std Fanfold 8 1/2 x 12 in   */,
    DMPAPER_FANFOLD_LGL_GERMAN            = 41  /* German Legal Fanfold 8 1/2 x 13 in */,
    DMPAPER_ISO_B4                        = 42  /* B4 (ISO) 250 x 353 mm              */,
    DMPAPER_JAPANESE_POSTCARD             = 43  /* Japanese Postcard 100 x 148 mm     */,
    DMPAPER_9X11                          = 44  /* 9 x 11 in                          */,
    DMPAPER_10X11                         = 45  /* 10 x 11 in                         */,
    DMPAPER_15X11                         = 46  /* 15 x 11 in                         */,
    DMPAPER_ENV_INVITE                    = 47  /* Envelope Invite 220 x 220 mm       */,
    DMPAPER_RESERVED_48                   = 48  /* RESERVED--DO NOT USE               */,
    DMPAPER_RESERVED_49                   = 49  /* RESERVED--DO NOT USE               */,
    DMPAPER_LETTER_EXTRA                  = 50  /* Letter Extra 9 \275 x 12 in        */,
    DMPAPER_LEGAL_EXTRA                   = 51  /* Legal Extra 9 \275 x 15 in         */,
    DMPAPER_TABLOID_EXTRA                 = 52  /* Tabloid Extra 11.69 x 18 in        */,
    DMPAPER_A4_EXTRA                      = 53  /* A4 Extra 9.27 x 12.69 in           */,
    DMPAPER_LETTER_TRANSVERSE             = 54  /* Letter Transverse 8 \275 x 11 in   */,
    DMPAPER_A4_TRANSVERSE                 = 55  /* A4 Transverse 210 x 297 mm         */,
    DMPAPER_LETTER_EXTRA_TRANSVERSE       = 56 /* Letter Extra Transverse 9\275 x 12 in */,
    DMPAPER_A_PLUS                        = 57  /* SuperA/SuperA/A4 227 x 356 mm      */,
    DMPAPER_B_PLUS                        = 58  /* SuperB/SuperB/A3 305 x 487 mm      */,
    DMPAPER_LETTER_PLUS                   = 59  /* Letter Plus 8.5 x 12.69 in         */,
    DMPAPER_A4_PLUS                       = 60  /* A4 Plus 210 x 330 mm               */,
    DMPAPER_A5_TRANSVERSE                 = 61  /* A5 Transverse 148 x 210 mm         */,
    DMPAPER_B5_TRANSVERSE                 = 62  /* B5 (JIS) Transverse 182 x 257 mm   */,
    DMPAPER_A3_EXTRA                      = 63  /* A3 Extra 322 x 445 mm              */,
    DMPAPER_A5_EXTRA                      = 64  /* A5 Extra 174 x 235 mm              */,
    DMPAPER_B5_EXTRA                      = 65  /* B5 (ISO) Extra 201 x 276 mm        */,
    DMPAPER_A2                            = 66  /* A2 420 x 594 mm                    */,
    DMPAPER_A3_TRANSVERSE                 = 67  /* A3 Transverse 297 x 420 mm         */,
    DMPAPER_A3_EXTRA_TRANSVERSE           = 68  /* A3 Extra Transverse 322 x 445 mm   */,
    DMPAPER_DBL_JAPANESE_POSTCARD         = 69 /* Japanese Double Postcard 200 x 148 mm */,
    DMPAPER_A6                            = 70  /* A6 105 x 148 mm                 */,
    DMPAPER_JENV_KAKU2                    = 71  /* Japanese Envelope Kaku #2       */,
    DMPAPER_JENV_KAKU3                    = 72  /* Japanese Envelope Kaku #3       */,
    DMPAPER_JENV_CHOU3                    = 73  /* Japanese Envelope Chou #3       */,
    DMPAPER_JENV_CHOU4                    = 74  /* Japanese Envelope Chou #4       */,
    DMPAPER_LETTER_ROTATED                = 75  /* Letter Rotated 11 x 8 1/2 11 in */,
    DMPAPER_A3_ROTATED                    = 76  /* A3 Rotated 420 x 297 mm         */,
    DMPAPER_A4_ROTATED                    = 77  /* A4 Rotated 297 x 210 mm         */,
    DMPAPER_A5_ROTATED                    = 78  /* A5 Rotated 210 x 148 mm         */,
    DMPAPER_B4_JIS_ROTATED                = 79  /* B4 (JIS) Rotated 364 x 257 mm   */,
    DMPAPER_B5_JIS_ROTATED                = 80  /* B5 (JIS) Rotated 257 x 182 mm   */,
    DMPAPER_JAPANESE_POSTCARD_ROTATED     = 81 /* Japanese Postcard Rotated 148 x 100 mm */,
    DMPAPER_DBL_JAPANESE_POSTCARD_ROTATED = 82 /* Double Japanese Postcard Rotated 148 x 200 mm */,
    DMPAPER_A6_ROTATED                    = 83  /* A6 Rotated 148 x 105 mm         */,
    DMPAPER_JENV_KAKU2_ROTATED            = 84  /* Japanese Envelope Kaku #2 Rotated */,
    DMPAPER_JENV_KAKU3_ROTATED            = 85  /* Japanese Envelope Kaku #3 Rotated */,
    DMPAPER_JENV_CHOU3_ROTATED            = 86  /* Japanese Envelope Chou #3 Rotated */,
    DMPAPER_JENV_CHOU4_ROTATED            = 87  /* Japanese Envelope Chou #4 Rotated */,
    DMPAPER_B6_JIS                        = 88  /* B6 (JIS) 128 x 182 mm           */,
    DMPAPER_B6_JIS_ROTATED                = 89  /* B6 (JIS) Rotated 182 x 128 mm   */,
    DMPAPER_12X11                         = 90  /* 12 x 11 in                      */,
    DMPAPER_JENV_YOU4                     = 91  /* Japanese Envelope You #4        */,
    DMPAPER_JENV_YOU4_ROTATED             = 92  /* Japanese Envelope You #4 Rotated*/,
    DMPAPER_P16K                          = 93  /* PRC 16K 146 x 215 mm            */,
    DMPAPER_P32K                          = 94  /* PRC 32K 97 x 151 mm             */,
    DMPAPER_P32KBIG                       = 95  /* PRC 32K(Big) 97 x 151 mm        */,
    DMPAPER_PENV_1                        = 96  /* PRC Envelope #1 102 x 165 mm    */,
    DMPAPER_PENV_2                        = 97  /* PRC Envelope #2 102 x 176 mm    */,
    DMPAPER_PENV_3                        = 98  /* PRC Envelope #3 125 x 176 mm    */,
    DMPAPER_PENV_4                        = 99  /* PRC Envelope #4 110 x 208 mm    */,
    DMPAPER_PENV_5                        = 100 /* PRC Envelope #5 110 x 220 mm    */,
    DMPAPER_PENV_6                        = 101 /* PRC Envelope #6 120 x 230 mm    */,
    DMPAPER_PENV_7                        = 102 /* PRC Envelope #7 160 x 230 mm    */,
    DMPAPER_PENV_8                        = 103 /* PRC Envelope #8 120 x 309 mm    */,
    DMPAPER_PENV_9                        = 104 /* PRC Envelope #9 229 x 324 mm    */,
    DMPAPER_PENV_10                       = 105 /* PRC Envelope #10 324 x 458 mm   */,
    DMPAPER_P16K_ROTATED                  = 106 /* PRC 16K Rotated                 */,
    DMPAPER_P32K_ROTATED                  = 107 /* PRC 32K Rotated                 */,
    DMPAPER_P32KBIG_ROTATED               = 108 /* PRC 32K(Big) Rotated            */,
    DMPAPER_PENV_1_ROTATED                = 109 /* PRC Envelope #1 Rotated 165 x 102 mm */,
    DMPAPER_PENV_2_ROTATED                = 110 /* PRC Envelope #2 Rotated 176 x 102 mm */,
    DMPAPER_PENV_3_ROTATED                = 111 /* PRC Envelope #3 Rotated 176 x 125 mm */,
    DMPAPER_PENV_4_ROTATED                = 112 /* PRC Envelope #4 Rotated 208 x 110 mm */,
    DMPAPER_PENV_5_ROTATED                = 113 /* PRC Envelope #5 Rotated 220 x 110 mm */,
    DMPAPER_PENV_6_ROTATED                = 114 /* PRC Envelope #6 Rotated 230 x 120 mm */,
    DMPAPER_PENV_7_ROTATED                = 115 /* PRC Envelope #7 Rotated 230 x 160 mm */,
    DMPAPER_PENV_8_ROTATED                = 116 /* PRC Envelope #8 Rotated 309 x 120 mm */,
    DMPAPER_PENV_9_ROTATED                = 117 /* PRC Envelope #9 Rotated 324 x 229 mm */,
    DMPAPER_PENV_10_ROTATED               = 118 /* PRC Envelope #10 Rotated 458 x 324 mm */,
    DMPAPER_LAST                          = DMPAPER_PENV_10_ROTATED,
    DMPAPER_USER                          = 256
}