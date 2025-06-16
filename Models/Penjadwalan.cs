using System.ComponentModel.DataAnnotations;

namespace APITaklimSmart.Models
{
    public class Penjadwalan
    {
        public int Id_Penjadwalan { get; set; }

        [Required]
        public string Nama_Penjadwalan { get; set; } = string.Empty;

        [Required]
        public DateTime Tanggal_Penjadwalan { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime Waktu_Penjadwalan { get; set; } = DateTime.UtcNow;

        [Required]
        public int Id_Lokasi { get; set; }

        [Required]
        public string Deskripsi_Penjadwalan { get; set; } = string.Empty;

        [Required]
        public string Status_Penjadwalan { get; set; } = string.Empty;

        [Required]
        public int Created_By { get; set; }

        public DateTime Created_At { get; set; } = DateTime.UtcNow;
        public DateTime Updated_At { get; set; } = DateTime.UtcNow;
    }
}
