using EveryWhere.FileConverter.Workers;

namespace EveryWhere.FileConverter
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly Converter _converter;

        public Worker(ILogger<Worker> logger, Converter converter)
        {
            _logger = logger;
            _converter = converter;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("����ʼִ��");
            _converter.startWorking();
            return Task.CompletedTask;
        }
    }
}