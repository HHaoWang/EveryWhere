using QRCoder;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using EveryWhere.DTO.Entity;
using System.Windows.Threading;
using QRCoder.Xaml;
using Window = System.Windows.Window;

namespace EveryWhere.Desktop.Views;

/// <summary>
/// LoginWindow.xaml 的交互逻辑
/// </summary>
public partial class LoginWindow : Window
{
    private readonly HttpClient _httpClient;
    private readonly DispatcherTimer _qrCodeTimer;
    private readonly DispatcherTimer _checkLoginTimer;
    private string _currentUuid = "";

    public LoginWindow()
    {
        InitializeComponent();
        _httpClient = new HttpClient();
        _qrCodeTimer = new DispatcherTimer();
        _qrCodeTimer.Tick += GetUuid;
        _qrCodeTimer.Interval = new TimeSpan(0, 0, 15);
        _qrCodeTimer.Start();
        GetUuid(null,null!);

        _checkLoginTimer = new DispatcherTimer();
        _checkLoginTimer.Tick += CheckIsLogin;
        _checkLoginTimer.Interval = new TimeSpan(0, 0, 3);
        _checkLoginTimer.Start();
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private async void CheckIsLogin(object? sender, EventArgs e)
    {
        string checkValidUrl = $"https://everywhere.hhao.wang/api/Login/QRCode/{_currentUuid}/Valid";
        string loginInfoResponseStr = await _httpClient.GetStringAsync(checkValidUrl);
        BaseResponse<CheckLoginValidResponse>? loginInfoResponse = JsonConvert
            .DeserializeObject<BaseResponse<CheckLoginValidResponse>>(loginInfoResponseStr);
        if (loginInfoResponse?.StatusCode!=200)
        {
            Debug.WriteLine(loginInfoResponseStr);
            return;
        }

        TipText.Text = "登录成功！";
        _checkLoginTimer.Stop();
        await Task.Delay(2000);
        DialogResult = true;
        Close();
    }

    private async void GetUuid(object? sender, EventArgs e)
    {
        const string qrCodeUuidApi = "https://everywhere.hhao.wang/api/Login/QRCode";
        Debug.WriteLine("开始获取二维码信息");
        try
        {
            string uuidResponseStr = await _httpClient.GetStringAsync(qrCodeUuidApi);
            BaseResponse<QrCodeUuidResponse>? uuidResponse = JsonConvert
                .DeserializeObject<BaseResponse<QrCodeUuidResponse>>(uuidResponseStr);
            if (uuidResponse?.Data?.Uuid == null)
            {
                Debug.WriteLine("请求二维码数据失败！");
                return;
            }
            Debug.WriteLine(uuidResponse.Data.Uuid);
            _currentUuid = uuidResponse.Data.Uuid;
            DrawingImage qrCodeBitmap = GenerateQrCode(uuidResponse.Data.Uuid);
            Dispatcher.Invoke(() =>
            {
                QrCodeImage.Source = qrCodeBitmap;
                //SetQrCodeImage(qrCodeBitmap);
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private DrawingImage GenerateQrCode(string uuid)
    {
        QRCodeGenerator qrGenerator = new();
        string data = JsonConvert.SerializeObject(new
        {
            operation="login",
            data = new
            {
                uuid,
                platform = "desktop"
            }
        });
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
        XamlQRCode qrCodeImage = new(qrCodeData);
        return qrCodeImage.GetGraphic(20);
    }

    public class QrCodeUuidResponse
    {
        public string? Uuid { get; set; }
    }

    public class CheckLoginValidResponse
    {
        public string? Token { get; set; }
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        _qrCodeTimer.Stop();
        _checkLoginTimer.Stop();
    }
}