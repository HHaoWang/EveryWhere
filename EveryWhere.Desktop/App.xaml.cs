using System;
using System.Threading;
using System.Windows;
using EveryWhere.Desktop.Views;

namespace EveryWhere.Desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{

    protected override void OnStartup(StartupEventArgs e)
    {
        _ = new Mutex(true, "EveryWhere.Desktop:SingleInstanceApp", out bool createdNew);
        if (!createdNew)
        {
            MessageBox.Show("启动失败，程序已经在运行！", "EveryWhere", MessageBoxButton.OK, MessageBoxImage.Stop);
            Environment.Exit(0);
        }

        Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        LoginWindow loginWindow = new();
        if (loginWindow.ShowDialog() == true)
        {
            base.OnStartup(e);
            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }
        else
        {
            Shutdown();
        }
    }
}