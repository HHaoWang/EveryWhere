using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
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

    private DispatcherTimer? _timer;

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
        
        _timer = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = TimeSpan.FromSeconds(10)
        };
        _timer.Tick += RefreshPrinters;
        _timer.Tick += ReceivePrintJobs;
        _timer.Tick += CheckPrintersState;
        _timer.Start();
    }

    /// <summary>
    /// 获取店铺信息
    /// </summary>
    /// <returns></returns>
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
            MessageBox.Show("获取店铺信息失败！您可能尚未开设店铺，访问https://everywhere.hhao.wang以开设店铺，程序即将退出。若反复出现此错误请联系技术支持人员。");
            Close();
            return;
        }

        _shop = shopInfoResponse.Data.Shop;
        ShopName.Text = _shop.Name;
    }

    /// <summary>
    /// 获取店铺打印机
    /// </summary>
    /// <returns></returns>
    private async Task GetShopPrinters()
    {
        #region 获取服务器打印机数据

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

        #endregion

        #region 提取本地打印机并展示

        List<Printer> localPrinters = Printer.GetLocalPrinters();
        List<Printer> printers = new();
        List<Database.PO.Printer> remotePrinters = printersResponse.Data.Printers
            .Where(r => r.ComputerId!.Equals(MachineGuid.ToString()))
            .ToList();
        //查找本地与服务器打印机的交集
        foreach (Printer localPrinter in localPrinters)
        {
            Database.PO.Printer? remotePrinter = remotePrinters.FirstOrDefault(remotePrinter
                => remotePrinter.DeviceName!.Equals(localPrinter.PhysicalName));
            if (remotePrinter == null)
            {
                continue;
            }

            localPrinter.Id = remotePrinter.Id;
            localPrinter.OnJobFinished += OnFinishedPrintJob;
            localPrinter.PrinterName = remotePrinter.Name!;
            printers.Add(localPrinter);
        }

        Printers.AddRange(printers);
        Printers.Sort((pa, pb) => string.Compare(pa.PhysicalName, pb.PhysicalName, StringComparison.CurrentCultureIgnoreCase));

        //添加打印机
        Printers = Printers.Prepend(new Printer
        {
            IsVirtual = true
        }).ToList();

        OnPropertyChanged(nameof(Printers));

        #endregion
    }

    /// <summary>
    /// 刷新各打印机状态
    /// </summary>
    /// <param name="o"></param>
    /// <param name="e"></param>
    private void RefreshPrinters(object? o, EventArgs e)
    {
        foreach (Printer printer in Printers)
        {
            printer.Refresh();
        }
        OnPropertyChanged(nameof(Printers));
    }

    #region 打印功能相关

    /// <summary>
    /// 接收打印机的待打印任务
    /// </summary>
    /// <param name="o"></param>
    /// <param name="e"></param>
    private async void ReceivePrintJobs(object? o,EventArgs e)
    {
        foreach (Printer printer in Printers)
        {
            if (printer.IsVirtual || printer.IsOffline)
            {
                continue;
            }
            List<PrintJob> jobs = await GetPrintJobs(printer.Id ?? -1);
            foreach (PrintJob job in jobs)
            {
                try
                {
                    if (printer.ExistsJob(job.Id))
                    {
                        Debug.WriteLine("打印任务已存在");
                        continue;
                    }
                    FileInfo file = await GetJobFile(job);
                    AddJobToPrinter(printer, job, file);
                }
                catch (Exception)
                {
                    MessageBox.Show("获取任务失败！请重新启动或检查网络连接，若反复出现错误请联系支持人员。");
                }
            }
        }
    }

    /// <summary>
    /// 获取打印机的待打印任务
    /// </summary>
    /// <param name="printerId">打印机ID</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task<List<PrintJob>> GetPrintJobs(int printerId)
    {
        Debug.WriteLine("获取打印任务");
        if (printerId <= 0)
        {
            return new List<PrintJob>();
        }

        string content = await RequestAsync($"{BaseUrl}/api/Printer/{printerId}/UnfinishedJobs", "", HttpMethod.Get,
            new List<(string, string)>()
            {
                ("Authorization", "Bearer " + Application.Current.Properties["Token"])
            });
        BaseResponse<PrintJobsResponse>? response = JsonConvert.DeserializeObject<BaseResponse<PrintJobsResponse>>(content);
        if (response is { StatusCode: 200, Data.Jobs: not null })
        {
            return response.Data.Jobs;
        }

        MessageBox.Show("获取打印机打印任务失败！请重新启动或检查网络连接，若反复出现错误请联系支持人员。");
        throw new Exception("获取打印机失败！");
    }

    /// <summary>
    /// 下载打印文件
    /// </summary>
    /// <param name="job">打印任务信息</param>
    /// <returns></returns>
    private async Task<FileInfo> GetJobFile(PrintJob job)
    {
        Debug.WriteLine("获取打印文件");
        Stream fileStream = await _httpClient.GetStreamAsync(job.File!.Location);
        string dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Download");
        string aimPath = Path.Combine(dirPath, Path.GetRandomFileName());
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        await using FileStream localStream = new(aimPath, FileMode.Create);
        await fileStream.CopyToAsync(localStream);
        await fileStream.DisposeAsync();
        await localStream.DisposeAsync();
        return new FileInfo(aimPath);
    }

    /// <summary>
    /// 添加打印任务到打印机
    /// </summary>
    /// <param name="printer">打印机</param>
    /// <param name="job">打印配置</param>
    /// <param name="file">要打印的文件</param>
    private static void AddJobToPrinter(Printer printer, PrintJob job, FileInfo file)
    {
        Debug.WriteLine("添加打印任务");
        printer.AddPrintJob(file, job.PageStart!.Value, job.PageEnd!.Value,
            job.Count!.Value, job.Color!.Value, job.Duplex!.Value, job.PageSize!,job.Id);
    }

    /// <summary>
    /// 检查每个打印机的任务状态
    /// </summary>
    private void CheckPrintersState(object? o, EventArgs e)
    {
        foreach (Printer printer in Printers)
        {
            printer.CheckJobsState();
        }
    }

    /// <summary>
    /// 打印机完成打印任务回调
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="jobId">打印任务ID</param>
    private async void OnFinishedPrintJob(object? sender, int jobId)
    {
        Debug.WriteLine($"完成ID为{jobId}的打印任务！");
        
        string responseStr = await RequestAsync($"{BaseUrl}/api/Order/PrintJob/{jobId}/Finish", "", HttpMethod.Get,
            new List<(string, string)>()
            {
                ("Authorization", "Bearer " + Application.Current.Properties["Token"])
            });
        BaseResponse<OrderResponse>? response = JsonConvert.DeserializeObject<BaseResponse<OrderResponse>>(responseStr);
        if (response is {StatusCode:200,Data.Order:not null})
        {
            (sender as Printer)?.RemoveJob(jobId);
            return;
        }
        Debug.WriteLine("通知打印完成失败！");
    }

    #endregion

    #region 基础功能相关

    private async Task<string> RequestAsync(string url, string content, HttpMethod method,
        List<(string, string)>? headers = null)
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

    #endregion

    #region 页面相关

    private void OnAddPrinter(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        AddPrinterWindow window = new(_shop!.Id);
        if (window.ShowDialog() == true)
        {
            Dispatcher.InvokeAsync(Init);
        }
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (_timer is {IsEnabled: true})
        {
            _timer.Stop();
        }
        base.OnClosing(e);
    }

    #endregion
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