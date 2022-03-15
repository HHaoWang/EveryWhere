using static EveryWhere.Database.PO.PrintJob;

namespace EveryWhere.MainServer.Contexts.Order
{
    public class PrintJob
    {
        public int Id { get; set; }
        public int Sequence { get; set; }
        public StatusState Status { get; set; }

        public PrintJob(int sequence)
        {
            Sequence = sequence;
            Status = StatusState.NotUploaded;
        }

        public void StartConversion()
        {
            this.Status = StatusState.Converting;
        }
    }
}
