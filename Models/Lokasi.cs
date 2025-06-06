using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APITaklimSmart.Models
{
    public class Lokasi
    {
        public int Id_Lokasi { get; set; }

        [Required]
        [StringLength(100)]
        public string Nama_Lokasi { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Alamat { get; set; } = string.Empty;

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        [StringLength(255)]
        public string? Deskripsi_Lokasi { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
