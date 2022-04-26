using System.Collections.Generic;
using System.Printing;
using EveryWhere.Desktop.Domain.PaperSize;
using NUnit.Framework;

namespace EveryWhere.Desktop.Test.PaperSizeTest;

public class UnitTest1
{
    [Test]
    public void Test1()
    {
        List<string> sizes = new()
        {
            "A4",
            "A5",
            "A12"
        };
        var sizeList = PaperSize.GetFrom(sizes,true);
        Assert.IsTrue(sizeList.Count == 2);
    }

    [Test]
    public void Test2()
    {
        List<string> sizes = new()
        {
            "A4",
            "A5",
            "A12"
        };
        var sizeList = PaperSize.GetFrom(sizes);
        Assert.IsTrue(sizeList.Count == 3 && sizeList.Contains(PageMediaSizeName.Unknown));
    }

    [Test]
    public void Test3()
    {
        List<PageMediaSizeName> sizes = new()
        {
            PageMediaSizeName.ISOA4,
            PageMediaSizeName.ISOA5,
            PageMediaSizeName.ISOA3,
            PageMediaSizeName.PRC16K
        };
        var sizeList = PaperSize.GetFrom(sizes);
        Assert.IsTrue(sizeList.Count == 4 && sizeList.Contains("PRC16K"));
    }

    [Test]
    public void Test4()
    {
        List<PageMediaSizeName> sizes = new()
        {
            PageMediaSizeName.ISOA4,
            PageMediaSizeName.ISOA5,
            PageMediaSizeName.ISOA3,
            PageMediaSizeName.PRC16K
        };
        var sizeList = PaperSize.GetFrom(sizes,true);
        Assert.IsTrue(sizeList.Count == 3);
    }
}