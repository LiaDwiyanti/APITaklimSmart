using System;
using System.ComponentModel.DataAnnotations;

namespace APITaklimSmart.Models
{
    public enum StatusPenjadwalan
    {
        diproses,
        disetujui,
        dibatalkan
    }

    public class Penjadwalan
    {
        [Key]
        public int Id_Penjadwalan { get; set; }

        [Required]
        public string Nama_Penjadwalan { get; set; }

        [Required]
        public DateTime Tanggal_Penjadwalan { get; set; }

        [Required]
        public DateTime Waktu_Penjadwalan { get; set; }

        [Required]
        public int Id_Lokasi { get; set; }

        public string Deskripsi_Penjadwalan { get; set; }

        [Required]
        public StatusPenjadwalan Status_Penjadwalan { get; set; } = StatusPenjadwalan.diproses;

        [Required]
        public int Created_By { get; set; }

        public DateTime Created_At { get; set; } = DateTime.Now;
        public DateTime Updated_At { get; set; } = DateTime.Now;
    }
}
