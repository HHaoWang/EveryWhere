using EveryWhere.FileServer.Domain;
using NUnit.Framework;

namespace EveryWhere.FileServer.Test.ConverterTest;

public class ExcelConverterTest
{
    [Test]
    public void TestCase1()
    {
        ExcelConverter converter = new();
        bool result = converter.ConvertToFixedFormat("D:\\21-22学生成绩文科.xls", "D:\\test.out", out int pageCount);
        Assert.IsTrue(result && pageCount == 12);
    }
}