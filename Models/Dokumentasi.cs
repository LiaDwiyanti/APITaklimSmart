using System.ComponentModel.DataAnnotations;

namespace APITaklimSmart.Models
{
    public class Dokumentasi
    {
        public int Id_Dokumentasi { get; set; }

        [Required]
        public int Id_Penjadwalan { get; set; }

        [Required]
        public int Uploaded_By { get; set; }

        [Required]
        public string File_Name { get; set; } = string.Empty;

        [Required]
        public string File_Url { get; set; } = string.Empty;

        [Required]
        public string File_Path { get; set; } = string.Empty;

        [Required]
        public string Caption_Dokumentasi { get; set; } = string.Empty;

        public DateTime Upload_Date { get; set; } = DateTime.UtcNow;
        public string Nama_Penjadwalan { get; set; } = string.Empty;
    }
}
