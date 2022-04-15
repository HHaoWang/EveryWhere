using EveryWhere.Database;
using EveryWhere.FileServer.Entity.Setting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using EveryWhere.FileServer.Domain;
using EveryWhere.FileServer.Utils;
using File = EveryWhere.Database.PO.File;
using Task = System.Threading.Tasks.Task;

namespace EveryWhere.FileServer.Service;

public class ConvertFileService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ConvertFileService> _logger;
    private readonly IModel? _channel;
    private readonly IConnection? _connection;
    private const string QueueName = "FileConvert";
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;

    public ConvertFileService(IServiceProvider serviceProvider, IOptions<MessageQueueSettings> settings, ILogger<ConvertFileService> logger, IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        RabbitConnectOption connectOption = settings.Value.RabbitConnect!;
        _logger = logger;
        _clientFactory = clientFactory;
        _configuration = configuration;
        _logger.LogInformation("开始初始化队列");
        try
        {
            ConnectionFactory factory = new()
            {
                HostName = connectOption.HostName,
                UserName = connectOption.UserName,
                Password = connectOption.Password,
                Port = connectOption.Port!.Value
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(QueueName, false, false, false, null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "初始化失败！");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //事件基本消费者
        EventingBasicConsumer consumer = new(_channel);

        //接收到消息事件
        consumer.Received += OnReceived;

        //启动消费者 设置为手动应答消息
        _channel.BasicConsume(QueueName, false, consumer);

        await Task.CompletedTask;
    }

    public override void Dispose()
    {
        base.Dispose();
        _logger.LogDebug(QueueName + " Consumer Dispose");
        _channel?.Dispose();
        _connection?.Close();
    }

    private async void OnReceived(object? sender, BasicDeliverEventArgs eventArgs)
    {
        //接收消息
        string message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
        _logger.LogInformation($"收到消息： {message}");

        File? file = JsonConvert.DeserializeObject<File>(message);

        if (file is null)
        {
            _logger.LogDebug("解析消息失败！");
            _channel?.BasicAck(eventArgs.DeliveryTag, false);
            return;
        }

        //下载文件
        string savedFullName = "";
        try
        {
            savedFullName = await DownloadFile(file.Location, file.Name);
        }
        catch (Exception e)
        {
            _logger.LogDebug("文件下载失败！\n" + e.Message);
            _channel?.BasicAck(eventArgs.DeliveryTag, false);
            return;
        }
        if (string.IsNullOrEmpty(savedFullName))
        {
            _logger.LogDebug("文件下载失败！\n");
            _channel?.BasicAck(eventArgs.DeliveryTag, false);
            return;
        }

        //格式转换
        FileInfo unconvertedFile = new(savedFullName);
        FileConverter? fileConvert = FileConverterFactory.GetFileConverter(unconvertedFile);
        if (fileConvert is null)
        {
            _logger.LogDebug($"不支持的文件格式！文件ID为：{file.Id}");
            _channel?.BasicAck(eventArgs.DeliveryTag, false);
            return;
        }
        fileConvert.ConvertToFixedFormat(savedFullName, unconvertedFile.Name,out int pageCount);

        //结果写回
        using (IServiceScope scope = _serviceProvider.CreateScope())
        {
            Repository repository = scope.ServiceProvider.GetRequiredService<Repository>();
            repository.File!.Attach(file);

            file.IsConverted = true;
            file.PageCount = pageCount;
            file.Location = _configuration["serverHost"] + "/api/File/Converted/Wps/" + file.Name;

            await repository.SaveChangesAsync();
        }

        //确认该消息已被消费
        _channel?.BasicAck(eventArgs.DeliveryTag, false);
    }

    /// <summary>
    /// 从指定url处下载文件并以指定名称保存
    /// </summary>
    /// <param name="url">下载链接</param>
    /// <param name="fileName">指定的文件名</param>
    /// <returns>完整的本地物理保存路径</returns>
    private async Task<string> DownloadFile(string url,string fileName)
    {
        //下载文件
        HttpClient? client = _clientFactory.CreateClient();
        HttpResponseMessage httpResponse = await client.GetAsync(url);
        httpResponse.EnsureSuccessStatusCode();

        //保存文件至本地
        Stream fileStream = await httpResponse.Content.ReadAsStreamAsync();
        DirectoryInfo unconvertedFileDirectory = FileUtil.GetUnconvertedFileDirectory();
        string fullFileName = Path.Combine(unconvertedFileDirectory.FullName, fileName);
        await using FileStream stream = new(fullFileName, FileMode.Create);
        await fileStream.CopyToAsync(stream);

        return fullFileName;
    }
}