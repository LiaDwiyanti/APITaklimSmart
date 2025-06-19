namespace APITaklimSmart.Models
{
    public class UpdateRiwayatRequest
    {
        public StatusPenjadwalan Status_Baru { get; set; }
        public int Changed_By { get; set; }
        public DateTime Changed_At { get; set; }
    }
}
