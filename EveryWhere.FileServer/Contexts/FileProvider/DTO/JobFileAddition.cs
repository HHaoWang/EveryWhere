namespace EveryWhere.FileServer.Contexts.FileProvider.DTO;

#nullable disable

public class JobFileAddition
{
    public int OrderId { get; set; }
    public int JobSequence { get; set; }
    public Stream FileStream  { get; set; }
    public string OriginalName { get; set; }
}