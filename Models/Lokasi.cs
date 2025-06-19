using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APITaklimSmart.Models
{
    public class Lokasi
    {
        public int Id_Lokasi { get; set; }

        [Required]
        public string Nama_Lokasi { get; set; } = string.Empty;

        [Required]
        public string Alamat { get; set; } = string.Empty;

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public string? Deskripsi_Lokasi { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
