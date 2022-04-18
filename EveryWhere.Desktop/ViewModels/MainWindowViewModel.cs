using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using EveryWhere.Desktop.Domain.Printer;

namespace EveryWhere.Desktop.ViewModels;

public class MainWindowViewModel
{
    public MainWindowViewModel()
    {
        Printers = Printer.GetLocalPrinters();
        Printers.Sort((pa, pb) => string.Compare(pa.PhysicalName, pb.PhysicalName, StringComparison.CurrentCultureIgnoreCase));
        Printers = Printers.Prepend(new Printer
        {
            IsVirtual = true
        }).ToList();
    }

    public List<Printer> Printers { get; set; }
}