namespace APITaklimSmart.Models
{
    public class Register
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string No_hp { get; set; }
        public string Alamat { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
