using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using EveryWhere.Database.PO;
using EveryWhere.Desktop.Entity.Response;
using EveryWhere.DTO.Entity;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Win32;
using Newtonsoft.Json;
using Printer = EveryWhere.Desktop.Domain.Printer.Printer;

namespace EveryWhere.Desktop.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public sealed partial class MainWindow:INotifyPropertyChanged
{
    #region Field

    private const string BaseUrl = "https://everywhere.hhao.wang";
    private readonly HttpClient _httpClient = new();
    private HubConnection? _connection;
    private Shop? _shop;

    public List<Printer> Printers { get; set; } = new();
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
    public bool IsConnected => _connection is { State: HubConnectionState.Connected };

    #endregion

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        Dispatcher.InvokeAsync(Init);
    }

    private async Task ConnectToServer()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5000/rpc/Desktop", option =>
            {
                option.AccessTokenProvider = () =>
                {
                    return Task.Run<string?>(() => MachineGuid.ToString());
                };
            })
            .WithAutomaticReconnect()
            .Build();
        _connection.Reconnected += connectionId =>
        {
            Dispatcher.Invoke(() =>
            {
                StateLight.Fill = Brushes.LawnGreen;
                StateText.Text = "连接服务器成功！";
            });
            return Task.CompletedTask;
        };
        _connection.Closed += connectionId =>
        {
            Dispatcher.Invoke(() =>
            {
                StateLight.Fill = Brushes.OrangeRed;
                StateText.Text = "未连接服务器";
            });
            return Task.CompletedTask;
        };
        await _connection.StartAsync();
    }

    private async Task Init()
    {
        Printers = new List<Printer>();
        await GetShopInfo();
        await GetShopPrinters();
    }

    private async Task GetShopInfo()
    {
        const string url = $"{BaseUrl}/api/Shop/Shopkeeper";
        List<(string, string)> headers = new()
        {
            ("Authorization","Bearer " + Application.Current.Properties["Token"])
        };
        string content = await RequestAsync(url, "", HttpMethod.Get, headers);

        BaseResponse<ShopInfoResponse>? shopInfoResponse = JsonConvert
            .DeserializeObject<BaseResponse<ShopInfoResponse>>(content);

        if (shopInfoResponse is not {StatusCode:200, Data.Shop: not null })
        {
            Debug.WriteLine("获取店铺信息失败!");
            MessageBox.Show("获取店铺信息失败！程序即将退出。若反复出现此错误请联系技术支持人员。");
            Close();
            return;
        }

        _shop = shopInfoResponse.Data.Shop;
    }

    private async Task GetShopPrinters()
    {
        string url = $"{BaseUrl}/api/Printer/Shop/{_shop!.Id}";
        List<(string, string)> headers = new()
        {
            ("Authorization", "Bearer " + Application.Current.Properties["Token"])
        };
        string content = await RequestAsync(url, "", HttpMethod.Get, headers);

        BaseResponse<GetPrintersResponse>? printersResponse = JsonConvert
            .DeserializeObject<BaseResponse<GetPrintersResponse>>(content);

        if (printersResponse is not { StatusCode: 200, Data.Printers: not null })
        {
            Debug.WriteLine("获取打印机失败!");
            MessageBox.Show("获取打印机信息失败！程序即将退出。若反复出现此错误请联系技术支持人员。");
            Close();
            return;
        }

        List<Printer> localPrinters = Printer.GetLocalPrinters();
        List<Printer> printers = new();
        foreach (Printer localPrinter in localPrinters)
        {
            if (printersResponse.Data.Printers.Exists(remotePrinter
                    =>remotePrinter.DeviceName!.Equals(localPrinter.PhysicalName))
                )
            { 
                printers.Add(localPrinter);
            }
        }
        Printers.AddRange(printers);
        Printers.Sort((pa, pb) => string.Compare(pa.PhysicalName, pb.PhysicalName, StringComparison.CurrentCultureIgnoreCase));
        Printers = Printers.Prepend(new Printer
        {
            IsVirtual = true
        }).ToList();
        OnPropertyChanged(nameof(Printers));
    }

    private async Task<string> RequestAsync(string url, string content,HttpMethod method,
        List<(string,string)>? headers = null)
    {
        if (headers != null)
        {
            foreach ((string? headerName, string? _) in headers)
            {
                _httpClient.DefaultRequestHeaders.Remove(headerName);
            }
            foreach ((string? headerName, string? headerValue) in headers)
            {
                _httpClient.DefaultRequestHeaders.Add(headerName, headerValue);
            }
        }

        HttpResponseMessage response = await _httpClient.SendAsync(new HttpRequestMessage
        {
            Content = new StringContent(content),
            Method = method,
            RequestUri = new Uri(url)
        });
        return await response.Content.ReadAsStringAsync();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void OnAddPrinter(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        AddPrinterWindow window = new();
        window.ShowDialog();
        Dispatcher.InvokeAsync(Init);
    }
}


public class PrinterTemplateSelector : DataTemplateSelector
{
    public DataTemplate? AddPrinterTemplate { get; set; }
    public DataTemplate? NormalPrinterTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (item is not Printer printer) return null;
        return printer.IsVirtual ? AddPrinterTemplate : NormalPrinterTemplate;
    }
}