namespace APITaklimSmart.Models
{
    public class UpdatePenjadwalanRequest
    {
        public string Nama_Penjadwalan { get; set; }
        public DateOnly Tanggal_Penjadwalan { get; set; }
        public TimeSpan Waktu_Penjadwalan { get; set; }
        public int Id_Lokasi { get; set; }
        public string? Deskripsi_Penjadwalan { get; set; }
        public StatusPenjadwalan Status_Penjadwalan { get; set; }
    }
}
