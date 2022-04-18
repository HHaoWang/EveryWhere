using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using EveryWhere.Desktop.Domain.Printer;

namespace EveryWhere.Desktop.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

}

public class PrinterTemplateSelector : DataTemplateSelector
{
    public DataTemplate AddPrinterTemplate { get; set; }
    public DataTemplate NormalPrinterTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (item is not Printer printer) return null;
        return printer.IsVirtual ? AddPrinterTemplate : NormalPrinterTemplate;
    }
}