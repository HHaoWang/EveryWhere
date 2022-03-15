using EveryWhere.FileConverter.DTO;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EveryWhere.Database;

namespace EveryWhere.FileConverter.Workers
{
    public abstract class BaseWorker : IDisposable
    {
        protected readonly RabbitConnectOption _connectOption;
        protected readonly ILogger _logger;
        protected readonly IModel? _channel;
        protected readonly IConnection? _connection;
        protected readonly Repository _repository;
        protected readonly string _queueName;

        public BaseWorker(IOptions<Settings> settings, ILogger<BaseWorker> logger, Repository repository, string queueName)
        {
            this._connectOption = settings.Value.RabbitConnect;
            this._logger = logger;
            this._repository = repository;
            this._queueName = queueName;

            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _connectOption.HostName,
                    UserName = _connectOption.UserName,
                    Password = _connectOption.Password,
                    Port = _connectOption.Port
                };
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.QueueDeclare(queueName, false, false, false, null);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, queueName + "初始化失败！");
            }
        }

        public void Dispose()
        {
            _logger.LogDebug(_queueName + " Consumer Dispose");
            _channel?.Dispose();
            _connection?.Close();
        }

        public void startWorking()
        {
            //事件基本消费者
            EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);

            //接收到消息事件
            consumer.Received += OnReceived;

            //启动消费者 设置为手动应答消息
            _channel.BasicConsume(_queueName, false, consumer);

            _logger.LogInformation(_queueName + " 开始工作");
        }

        public abstract void OnReceived(object? ch, BasicDeliverEventArgs ea);
    }
}
