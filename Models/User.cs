using System.ComponentModel.DataAnnotations;
using static APITaklimSmart.Models.Enums;

namespace APITaklimSmart.Models
{
    public class User
    {
        public int Id_User { get; set; }
        
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string No_hp { get; set; }

        [Required]
        public string Alamat { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public UserRole User_Role { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
