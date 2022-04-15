using System;
using System.Collections.Generic;
using EveryWhere.Desktop.Domain.Printer;
using NUnit.Framework;

namespace EveryWhere.Desktop.Test.PrinterTest;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        List<Printer> printers = Printer.GetLocalPrinters();
        Assert.IsTrue(printers.Exists(p=>p.PrinterName.Equals("Canon MG2400 series Printer",StringComparison.CurrentCultureIgnoreCase) && p.IsOffline));
    }
}