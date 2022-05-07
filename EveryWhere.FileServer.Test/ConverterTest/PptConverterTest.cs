using EveryWhere.FileServer.Domain;
using NUnit.Framework;

namespace EveryWhere.FileServer.Test.ConverterTest;

public class PptConverterTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        PowerPointConverter converter = new();
        bool result = converter.ConvertToXps("D:\\“º È∆¿¥±ÁPPT.pptx", "D:\\test.out.xps", out int pageCount);
        Assert.IsTrue(result && pageCount == 12);
    }

    [Test]
    public void Test2()
    {
        PowerPointConverter converter = new();
        bool result = converter.ConvertToPdf("D:\\“º È∆¿¥±ÁPPT.pptx", "D:\\test.out", out int pageCount);
        Assert.IsTrue(result && pageCount == 12);
    }
}