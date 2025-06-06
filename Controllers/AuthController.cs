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
        public IActionResult Register([FromBody] User input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            User existingUser = _userContext.getUserByNoHP(input.No_hp);
            if (existingUser != null)
            {
                return BadRequest("Nomor HP sudah digunakan.");
            }

            var (lat, lon) = _mapbox.GetKordinatLokasi(input.Alamat);
            if (lat == 0 && lon == 0)
            {
                return BadRequest("Alamat tidak valid atau tidak dapat ditemukan.");
            }

            input.Password = BCrypt.Net.BCrypt.HashPassword(input.Password);
            input.User_Role = UserRole.user;
            input.CreatedAt = DateTime.UtcNow;
            input.UpdatedAt = DateTime.UtcNow;

            var id = _userContext.RegistUser(input);

            bool lokasiSaved = _lokasiContext.SaveLokasi(new Lokasi
            {
                Nama_Lokasi = "Rumah " + input.Username,
                Alamat = input.Alamat,
                Latitude = (decimal)lat,
                Longitude = (decimal)lon,
                Deskripsi_Lokasi = "Lokasi dari user baru",
                CreatedAt = DateTime.UtcNow
            });

            if (!lokasiSaved)
            {
                return StatusCode(500, "Gagal menyimpan data lokasi.");
            }

            bool isRegistered = _userContext.RegistUser(input);

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
