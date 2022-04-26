using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using EveryWhere.Desktop.Domain.PaperSize;
using EveryWhere.Desktop.Domain.Printer;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace EveryWhere.Desktop.Views;

/// <summary>
/// AddPrinterWindow.xaml 的交互逻辑
/// </summary>
public sealed partial class AddPrinterWindow:INotifyPropertyChanged
{
    public List<Printer>? Printers { get; set; }
    public ObservableCollection<PrinterPrice> Prices { get; set; } = new();

    public Printer? CurrentPrinter => Printers == null || PrinterSelector.SelectedIndex<0 ? null : Printers![PrinterSelector.SelectedIndex];

    #region ColorAndDuplexSupport

    private bool _supportColor;
    public bool SupportColor
    {
        get => _supportColor;
        set
        {
            _supportColor = value;
            OnPropertyChanged(nameof(NotSupportColor));
            OnPropertyChanged(nameof(SupportColor));
            OnPropertyChanged(nameof(ColorPriceVisibility));
            OnPropertyChanged(nameof(DuplexColorPriceVisibility));
        }
    }
    public bool NotSupportColor
    {
        get => !_supportColor;
        set
        {
            _supportColor = !value;
            OnPropertyChanged(nameof(NotSupportColor));
            OnPropertyChanged(nameof(SupportColor));
            OnPropertyChanged(nameof(ColorPriceVisibility));
            OnPropertyChanged(nameof(DuplexColorPriceVisibility));
        }
    }

    private bool _supportDuplex;
    public bool SupportDuplex
    {
        get => _supportDuplex;
        set
        {
            _supportDuplex = value;
            OnPropertyChanged(nameof(NotSupportDuplex));
            OnPropertyChanged(nameof(SupportDuplex));
            OnPropertyChanged(nameof(DuplexPriceVisibility));
            OnPropertyChanged(nameof(DuplexColorPriceVisibility));
        }
    }
    public bool NotSupportDuplex
    {
        get => !_supportDuplex;
        set
        {
            _supportDuplex = !value;
            OnPropertyChanged(nameof(NotSupportDuplex));
            OnPropertyChanged(nameof(SupportDuplex));
            OnPropertyChanged(nameof(DuplexPriceVisibility));
            OnPropertyChanged(nameof(DuplexColorPriceVisibility));
        }
    }
    
    public Visibility ColorPriceVisibility => NotSupportColor ? Visibility.Hidden : Visibility.Visible;
    public Visibility DuplexPriceVisibility => NotSupportDuplex ? Visibility.Hidden : Visibility.Visible;
    public Visibility DuplexColorPriceVisibility => NotSupportColor||NotSupportDuplex ? Visibility.Hidden : Visibility.Visible;

    #endregion


    public List<Tuple<string,bool>> AvailableSize
    {
        get
        {
            if (CurrentPrinter is null)
            {
                return new List<Tuple<string, bool>>();
            }
            return PaperSize.GetFrom(CurrentPrinter.SupportSizeNames,true)
                .Select(n => 
                    new Tuple<string, bool>(n.ToString(), 
                        !Prices
                            .ToList()
                            .Exists(p => p.Size!.Equals(n.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    ))
                .ToList();
        }
    }

    public Visibility ShowPricesTable { get; set; } = Visibility.Hidden;

    public AddPrinterWindow()
    {
        InitializeComponent();
        DataContext = this;
        Init();
    }

    private void Init()
    {
        Printers = Printer.GetLocalPrinters();
        OnPropertyChanged(nameof(Printers));
        OnPropertyChanged(nameof(AvailableSize));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void OnChooseAPrinter(object sender, SelectionChangedEventArgs e)
    {
        Prices = new ObservableCollection<PrinterPrice>();
        ShowPricesTable = Visibility.Visible;
        SupportColor = CurrentPrinter!.SupportColor;
        SupportDuplex = CurrentPrinter!.SupportDuplex;
        OnPropertyChanged(nameof(ShowPricesTable));
        OnPropertyChanged(nameof(AvailableSize));
        OnPropertyChanged(nameof(Prices));
        OnPropertyChanged(nameof(CurrentPrinter));
    }

    private void AddSizePrice(object sender, RoutedEventArgs e)
    {
        Prices.Add(new PrinterPrice());
        OnPropertyChanged(nameof(Prices));
    }

    private void OnChangeColorDuplex(object sender, RoutedEventArgs e)
    {
        OnPropertyChanged(nameof(ColorPriceVisibility));
        OnPropertyChanged(nameof(DuplexPriceVisibility));
        OnPropertyChanged(nameof(DuplexColorPriceVisibility));
    }

    private void OnCellChanged(object? sender, DataGridCellEditEndingEventArgs e)
    {
        if ((string) e.Column.Header == "纸张")
        {
            OnPropertyChanged(nameof(AvailableSize));
        }
        e.Cancel = false;
    }
}

public sealed class PrinterPrice:INotifyPropertyChanged
{
    private string? _size;

    public string? Size
    {
        get => _size??"";
        set
        {
            if (value!=null)
            {
                _size = value;
            }
            OnPropertyChanged(nameof(Size));
        }
    }

    public decimal SingleBlack { get; set; }
    public decimal SingleColor { get; set; }
    public decimal DuplexBlack { get; set; }
    public decimal DuplexColor { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}