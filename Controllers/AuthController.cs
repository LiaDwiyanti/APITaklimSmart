using APITaklimSmart.Models;
using APITaklimSmart.Services;
using Microsoft.AspNetCore.Mvc;
using static APITaklimSmart.Models.Enums;

namespace APITaklimSmart.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserContext _userContext;
        private readonly UserSessionContext _sessionContext;
        private readonly LokasiContext _lokasiContext;
        private readonly MapBoxService _mapbox;

        public AuthController(UserContext userContext, UserSessionContext sessionContext, LokasiContext lokasiContext, MapBoxService mapbox)
        {
            _userContext = userContext;
            _sessionContext = sessionContext;
            _lokasiContext = lokasiContext;
            _mapbox = mapbox;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] Register input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            User existingUser = _userContext.getUserByNoHP(input.No_hp);
            if (existingUser != null)
            {
                return BadRequest("Nomor HP sudah digunakan.");
            }

            double lat = 0, lon = 0;
            if (input.Latitude.HasValue && input.Longitude.HasValue)
            {
                lat = input.Latitude.Value;
                lon = input.Longitude.Value;
            }
            else
            {
                var coords = _mapbox.GetKordinatLokasi(input.Alamat);
                lat = coords.lat;
                lon = coords.lon;
            }

            if (lat == 0 && lon == 0)
            {
                return BadRequest("Alamat tidak valid atau tidak dapat ditemukan, dan tidak ada lokasi manual diberikan.");
            }

            var user = new User
            {
                Username = input.Username,
                Email = input.Email,
                No_hp = input.No_hp,
                Alamat = input.Alamat,
                Password = BCrypt.Net.BCrypt.HashPassword(input.Password),
                User_Role = UserRole.user,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var lokasi = new Lokasi
            {
                Nama_Lokasi = "Rumah " + input.Username,
                Alamat = input.Alamat,
                Latitude = (decimal)lat,
                Longitude = (decimal)lon,
                Deskripsi_Lokasi = "Lokasi dari user baru",
                CreatedAt = DateTime.UtcNow
            };

            bool isRegistered = _userContext.RegistUser(user, lokasi);

            if (isRegistered)
            {
                return Ok(new { message = "Registrasi berhasil." });
            }
            else
            {
                return StatusCode(500, new { message = "Registrasi gagal" });
            }
        }
    }
}
