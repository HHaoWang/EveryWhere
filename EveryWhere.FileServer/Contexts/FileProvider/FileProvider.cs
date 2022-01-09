namespace EveryWhere.FileServer.Contexts.FileProvider
{
    public class FileProvider
    {
        protected int OrderId { get; set; }
        protected List<FileInfo> Files { get; set; }

        public FileProvider()
        {
            Files = new List<FileInfo>();
        }
    }
}
