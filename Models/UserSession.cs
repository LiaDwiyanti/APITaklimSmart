using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APITaklimSmart.Models
{
    public class UserSession
    {
        public int Id_Session { get; set; }
        public int Id_User { get; set; }
        public DateTime LoginAt { get; set; }
        public DateTime? LogoutAt { get; set; }
        public string? DeviceInfo { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
