using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using EveryWhere.MainServer.Entity.Setting;
using RabbitMQ.Client;
using File = EveryWhere.Database.PO.File;

namespace EveryWhere.MainServer.MessageQueue;

public class Publisher
{
    private readonly ILogger _logger;
    private readonly IModel _channel;

    public Publisher(IOptions<MessageQueueSettings> settings, ILogger<Publisher> logger)
    {
        RabbitConnectOption? connectOption = settings.Value?.RabbitConnect;
        this._logger = logger;

        try
        {
            ConnectionFactory factory = new()
            {
                HostName = connectOption?.HostName,
                UserName = connectOption?.UserName,
                Password = connectOption?.Password,
                Port = connectOption!.Port!.Value
            };
            _channel = factory.CreateConnection().CreateModel();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "RabbitMQ初始化失败！");
        }
    }

    public virtual void PushMessage(string routingKey, object message)
    {
        _logger.LogInformation($"Push Message, routingKey:{routingKey}");

        //事先声明队列防止不存在导致数据丢失
        _channel.QueueDeclare(routingKey, false, false, false, null);
        _channel.ExchangeDeclare("exchange", ExchangeType.Direct);
        _channel.QueueBind(routingKey, "exchange", routingKey);

        string msgJson = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(msgJson);
        _channel.BasicPublish("exchange", routingKey, null, body);
    }

    public void AddFileConvertMission(File file)
    {
        PushMessage("FileConvert",file);
    }
}