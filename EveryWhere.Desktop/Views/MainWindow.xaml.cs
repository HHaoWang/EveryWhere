using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using EveryWhere.Desktop.Domain.Printer;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Win32;

namespace EveryWhere.Desktop.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public static Guid MachineGuid
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

    private HubConnection? _connection;
    public bool IsConnected => _connection is {State: HubConnectionState.Connected};

    public MainWindow()
    {
        InitializeComponent();
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