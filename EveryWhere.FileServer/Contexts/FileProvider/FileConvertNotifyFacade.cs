using EveryWhere.DTO.MessageQueue;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Security.Policy;
using System.Text;

namespace EveryWhere.FileServer.Contexts.FileProvider
{
    public class FileConvertNotifyFacade
    {
        private readonly RabbitConnectOption connectOption;
        private readonly ILogger logger;
        private readonly IModel channel;

        public FileConvertNotifyFacade(IOptions<MessageQueueSettings> settings,
            ILogger<FileConvertNotifyFacade> logger)
        {
            this.connectOption = settings.Value?.RabbitConnect;
            this.logger = logger;

            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = connectOption.HostName,
                    UserName = connectOption.UserName,
                    Password = connectOption.Password,
                    Port = connectOption.Port
                };
                channel = factory.CreateConnection().CreateModel();
            }
            catch (System.Exception ex)
            {
                logger.LogError(ex, "RabbitMQ初始化失败！");
            }
        }

        private void PushMessage(string routingKey, object message)
        {
            logger.LogInformation($"Push Message, routingKey:{routingKey}");

            //事先声明队列防止不存在导致数据丢失
            channel.QueueDeclare(routingKey, false, false, false, null);
            channel.ExchangeDeclare("exchange", ExchangeType.Direct);
            channel.QueueBind(routingKey, "exchange", routingKey);

            string msgJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(msgJson);
            channel.BasicPublish("exchange", routingKey, null, body);
        }

        public void AddFile(int fileId)
        {
            PushMessage("FileConvertQueue", new AddNotConvertedFile
            {
                FileId = fileId,
            });
        }
    }
}
