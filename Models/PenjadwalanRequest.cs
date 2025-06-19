namespace APITaklimSmart.Models
{
    public class PenjadwalanRequest
    {
        public string Nama_Penjadwalan { get; set; }
        public DateOnly Tanggal_Penjadwalan { get; set; }
        public TimeSpan Waktu_Penjadwalan { get; set; }
        public int Id_Lokasi { get; set; }
        public string? Deskripsi_Penjadwalan { get; set; }
    }
}
