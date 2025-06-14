using System.Text.Json.Serialization;

namespace APITaklimSmart.Models
{
    public class Register
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        
        [JsonPropertyName("no_hp")]
        public string No_hp { get; set; }
        public string Alamat { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
