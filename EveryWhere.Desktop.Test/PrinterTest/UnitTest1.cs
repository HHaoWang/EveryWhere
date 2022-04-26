using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Printing;
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
        Assert.IsFalse(printers.Exists(p=>p.PrinterName.Equals("Canon MG2400 series Printer",StringComparison.CurrentCultureIgnoreCase) && p.IsOffline));
    }
}