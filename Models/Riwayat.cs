namespace APITaklimSmart.Models
{
    public class Riwayat
    {
        public int Id_Riwayat { get; set; }
        public int Id_Penjadwalan { get; set; }
        public StatusPenjadwalan? Status_Lama { get; set; }
        public StatusPenjadwalan Status_Baru { get; set; } = StatusPenjadwalan.Diproses;
        public int Changed_By { get; set; }
        public string? Alasan { get; set; }
        public DateTime Changed_At { get; set; } = DateTime.UtcNow;
    }
}
