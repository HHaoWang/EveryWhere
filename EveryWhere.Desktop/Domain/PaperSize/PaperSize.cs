using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;

namespace EveryWhere.Desktop.Domain.PaperSize;

public class PaperSize
{
    private static Dictionary<PageMediaSizeName, string> _sizeMap = new()
    {
        { PageMediaSizeName.ISOA0, "A0" },
        { PageMediaSizeName.ISOA1, "A1" },
        { PageMediaSizeName.ISOA2, "A2" },
        { PageMediaSizeName.ISOA3, "A3" },
        { PageMediaSizeName.ISOA4 , "A4"},
        { PageMediaSizeName.ISOA5, "A5" },
        { PageMediaSizeName.ISOA6, "A6" },
        { PageMediaSizeName.ISOA7, "A7" },
        { PageMediaSizeName.ISOA8, "A8" },
        { PageMediaSizeName.ISOA9, "A9" },
        { PageMediaSizeName.ISOA10, "A10" },
        { PageMediaSizeName.ISOB0, "B0" },
        { PageMediaSizeName.ISOB1, "B1" },
        { PageMediaSizeName.ISOB2, "B2" },
        { PageMediaSizeName.ISOB3, "B3" },
        { PageMediaSizeName.ISOB4, "B4" },
        { PageMediaSizeName.ISOB5Extra, "B5" },
        { PageMediaSizeName.ISOB7, "B7" },
        { PageMediaSizeName.ISOB8, "B8" },
        { PageMediaSizeName.ISOB9, "B9" }
    };

    public static List<string> GetFrom(List<PageMediaSizeName> sizes,bool ignoreNotSupport = false)
    {
        List<string> mappedSizes = new();
        foreach (PageMediaSizeName sizeName in sizes)
        {
            _sizeMap.TryGetValue(sizeName, out string? name);
            if (ignoreNotSupport && name == null)
            {
                continue;
            }
            mappedSizes.Add(name ?? sizeName.ToString());
        }

        return mappedSizes;
    }

    public static List<PageMediaSizeName> GetFrom(List<string> sizes, bool ignoreNotSupport = false)
    {
        List<PageMediaSizeName> mappedSizes = new();
        foreach (string sizeName in sizes)
        {
            var pair = _sizeMap.FirstOrDefault(p => p.Value.Equals(sizeName, StringComparison.CurrentCultureIgnoreCase));
            if (ignoreNotSupport && !_sizeMap.ContainsValue(sizeName))
            {
                continue;
            }
            mappedSizes.Add(pair.Key);
        }

        return mappedSizes;
    }

    public static string GetFrom(PageMediaSizeName size)
    {
        return _sizeMap.TryGetValue(size, out string? name) ? name : size.ToString();
    }

    public static PageMediaSizeName? GetFrom(string size)
    {
        KeyValuePair<PageMediaSizeName, string> pair = _sizeMap.FirstOrDefault(p => p.Value.Equals(size, StringComparison.CurrentCultureIgnoreCase));
        if (_sizeMap.ContainsValue(size))
        {
            return pair.Key;
        }

        if (Enum.TryParse(size, true, out PageMediaSizeName sizeName))
        {
            return sizeName;
        }
        return null;
    }
}