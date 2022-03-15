using EveryWhere.Database;
using EveryWhere.DTO.MessageQueue;
using EveryWhere.FileConverter.DTO;
using EveryWhere.Util;
using Microsoft.Extensions.Options;
using Microsoft.Office.Interop.Word;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System.Runtime.InteropServices;
using System.Text;
using Document = Microsoft.Office.Interop.Word.Document;
using Microsoft.EntityFrameworkCore;

namespace EveryWhere.FileConverter.Workers
{
    public class Converter : BaseWorker
    {
        public Converter(IOptions<Settings> settings, ILogger<BaseWorker> logger, Repository repository) 
            : base(settings, logger, repository, "FileConvertQueue")
        {
        }

        public override void OnReceived(object? ch, BasicDeliverEventArgs ea)
        {
            string message = Encoding.UTF8.GetString(ea.Body.ToArray());
            _logger.LogInformation($"{_queueName} 收到消息： {message}");
            AddNotConvertedFile? msgObj = JsonConvert.DeserializeObject<AddNotConvertedFile>(message);
            if (msgObj is null)
            {
                _logger.LogError(message + " 解析错误");
                //确认该消息已被消费
                _channel!.BasicAck(ea.DeliveryTag, false);
                return;
            }

            Database.PO.File? file = _repository.File
                .Include(f=>f.PrintJob)
                .FirstOrDefault(f => f.Id == msgObj.FileId);
            if (file is null)
            {
                _logger.LogError(message + " 未找到文件信息");
                //确认该消息已被消费
                _channel!.BasicAck(ea.DeliveryTag, false);
                return;
            }

            string fullFilePath = Path.Combine(FileUtil.GetFileDirectory().FullName, file.Name);
            FileInfo fileInfo = new FileInfo(fullFilePath);
            if (!fileInfo.Exists)
            {
                _logger.LogError(message + " 未找到文件信息");
                //确认该消息已被消费
                _channel!.BasicAck(ea.DeliveryTag, false);
                return;
            }

            if (fileInfo.Extension!=".doc" && fileInfo.Extension!=".docx")
            {
                _logger.LogError(message + " 不支持的文件");
                //确认该消息已被消费
                _channel!.BasicAck(ea.DeliveryTag, false);
                return;
            }

            string targetFileName = Path.Combine(FileUtil.GetConvertedFileDirectory().FullName, fileInfo.Name + ".xps");
            var result = WordToXPS(fileInfo.FullName,targetFileName);

            if (!result)
            {
                _logger.LogError(message + " 转换失败");
                //确认该消息已被消费
                _channel!.BasicAck(ea.DeliveryTag, false);
            }

            file.Name = Path.GetFileName(targetFileName);
            file.PrintJob.Status = Database.PO.PrintJob.StatusState.Uploaded;
            _repository.SaveChanges();
            _logger.LogInformation(message + "转换完成");
        }

        public static bool WordToXPS(string sourcePath, string targetPath)
        {
            bool result = false;
            Application application = new Application();
            Document? document = null;
            Documents documents = application.Documents;
            try
            {
                application.Visible = false;
                document = documents.Open(sourcePath);
                document.ExportAsFixedFormat(targetPath, WdExportFormat.wdExportFormatXPS);
                result = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(sourcePath);
                Console.WriteLine(targetPath);
                Console.WriteLine(e.Message);
                result = false;
            }
            finally
            {
                document?.Close();
                application.Quit();
                Marshal.ReleaseComObject(document);
                Marshal.ReleaseComObject(documents);
                Marshal.ReleaseComObject(application);
            }
            return result;
        }
    }
}
