namespace EveryWhere.Desktop.Entity.Dto;

public class JobState
{
    public int JobId { get; set; }
    public Status State { get; set; }

    public enum Status
    {
        Printing,
        Finished,
        Deleted
    }
}