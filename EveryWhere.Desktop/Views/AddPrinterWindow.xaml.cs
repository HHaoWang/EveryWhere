using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using EveryWhere.Desktop.Domain.PaperSize;
using EveryWhere.Desktop.Domain.Printer;
using EveryWhere.DTO.Entity;
using Microsoft.Win32;
using Newtonsoft.Json;

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
    private static Guid MachineGuid
    {
        get
        {
            Guid machineGuid = Guid.Empty;

            RegistryKey localMachineX64View = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            RegistryKey? cryptographySubKey = localMachineX64View.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");

            if (cryptographySubKey == null) return machineGuid;

            string? machineGuidValue = (string?)cryptographySubKey.GetValue("MachineGuid");

            if (!Guid.TryParse(machineGuidValue, out machineGuid))
            {
                machineGuid = Guid.Empty;
            }

            return machineGuid;
        }
    }
    private HttpClient? _httpClient = new();
    private const string BaseUrl = "https://everywhere.hhao.wang";
    private int _shopId;

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

    public AddPrinterWindow(int shopId)
    {
        _shopId = shopId;
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


    private async void OnSubmit(object sender, RoutedEventArgs e)
    {
        Dictionary<string, NewPrinter.PaperSizePrice> size = new();
        foreach (PrinterPrice printerPrice in Prices)
        {
            if (string.IsNullOrWhiteSpace(printerPrice.Size))
            {
                continue;
            }
            size.Add(printerPrice.Size,new NewPrinter.PaperSizePrice()
            {
                SingleBlack = printerPrice.SingleBlack,
                DuplexBlack = printerPrice.DuplexBlack,
                SingleColor = printerPrice.SingleColor,
                DuplexColor = printerPrice.DuplexColor
            });
        }

        NewPrinter printer = new()
        {
            ComputerId = MachineGuid.ToString(),
            DeviceName = CurrentPrinter!.PhysicalName,
            IsWork = false,
            Name = string.IsNullOrWhiteSpace(DisplayName.Text) ? CurrentPrinter!.PhysicalName : DisplayName.Text.Trim(),
            SupportColor = SupportColor,
            SupportDuplex = SupportDuplex,
            SupportSizes = size,
            ShopId = _shopId
        };

        _httpClient ??= new HttpClient();
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Application.Current.Properties["Token"]);
       
        StringContent content = new(JsonConvert.SerializeObject(printer), Encoding.UTF8, "application/json");
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
        HttpResponseMessage response = await _httpClient.SendAsync(new HttpRequestMessage()
        {
            Content = content,
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{BaseUrl}/api/Printer")
        });
        Debug.WriteLine(await response.Content.ReadAsStringAsync());
        if (response.IsSuccessStatusCode)
        {
            DialogResult = true;
            Close();
        }
        else
        {
            MessageBox.Show("服务器添加打印机失败！请检查是否已经添加过该打印机并重试，若反复失败请联系技术支持人员。");
        }
    }

    private async Task<string> RequestAsync(string url, string content, HttpMethod method,
        List<(string, string)>? headers = null)
    {
        if (headers != null)
        {
            foreach ((string? headerName, string? _) in headers)
            {
                _httpClient!.DefaultRequestHeaders.Remove(headerName);
            }
            foreach ((string? headerName, string? headerValue) in headers)
            {
                _httpClient!.DefaultRequestHeaders.Add(headerName, headerValue);
            }
        }

        HttpResponseMessage response = await _httpClient!.SendAsync(new HttpRequestMessage
        {
            Content = new StringContent(content),
            Method = method,
            RequestUri = new Uri(url)
        });
        return await response.Content.ReadAsStringAsync();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        _httpClient?.Dispose();
        base.OnClosing(e);
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