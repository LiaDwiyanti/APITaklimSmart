using APITaklimSmart.Helpers;
using APITaklimSmart.Models;
using APITaklimSmart.Services;
using Microsoft.AspNetCore.Identity.Data;
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
        private readonly IConfiguration _config;

        public AuthController(UserContext userContext, UserSessionContext sessionContext, LokasiContext lokasiContext, MapBoxService mapbox, IConfiguration config)
        {
            _userContext = userContext;
            _sessionContext = sessionContext;
            _lokasiContext = lokasiContext;
            _mapbox = mapbox;
            _config = config;
        }

        [HttpPost("api/register")]
        public IActionResult Register([FromBody] Register input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User existingUser = _userContext.getUserByNoHP(input.No_hp);
            if (existingUser != null)
            {
                return Conflict(new { success = false, message = "Nomor Handphone sudah pernah digunakan." });
            }

            decimal lat = 0, lon = 0;
            string alamatFinal = input.Alamat;

            if (input.Latitude.HasValue && input.Longitude.HasValue)
            {
                lat = input.Latitude.Value;
                lon = input.Longitude.Value;

                alamatFinal = _mapbox.GetAlamatDariKoordinat(lat, lon);
                if (string.IsNullOrWhiteSpace(alamatFinal) || alamatFinal == "Alamat tidak ditemukan")
                {
                    return BadRequest(new { success = false, message = "Koordinat tidak valid atau tidak ditemukan di peta." });
                }
            }
            else if (!string.IsNullOrWhiteSpace(input.Alamat))
            {
                var coords = _mapbox.GetKordinatLokasi(input.Alamat);
                lat = coords.lat;
                lon = coords.lon;

                if (lat == 0 && lon == 0)
                {
                    return BadRequest(new { success = false, message = "Alamat tidak dapat ditemukan, mohon pilih alamat di peta." });
                }
            }
            else
            {
                return BadRequest(new { success = false, message = "Alamat atau lokasi harus diisi." });
            }

            var user = new User
            {
                Username = input.Username,
                Email = input.Email,
                No_hp = input.No_hp,
                Alamat = alamatFinal,
                Password = BCrypt.Net.BCrypt.HashPassword(input.Password),
                User_Role = UserRole.user,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var lokasi = new Lokasi
            {
                Nama_Lokasi = "Rumah " + input.Username,
                Alamat = alamatFinal,
                Latitude = (decimal)lat,
                Longitude = (decimal)lon,
                Deskripsi_Lokasi = "Lokasi dari user baru",
                CreatedAt = DateTime.UtcNow
            };

            bool isRegistered = _userContext.RegistUser(user, lokasi);

            if (isRegistered)
            {
                return Ok(new { success = true, message = "Registrasi berhasil." });
            }
            else
            {
                return StatusCode(500, new { success = false, message = "Registrasi gagal. Silakan coba lagi." });
            }
        }

        [HttpPost("api/login")]
        public IActionResult Login([FromBody]Login input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            User user = _userContext.getUserByUsername(input.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(input.Password, user.Password))
            {
                return Unauthorized(new { success = false, message = "Username atau password salah." });
            }

            // Mengambil device info dari header
            string deviceInfo = Request.Headers["User-Agent"].ToString();

            int sessionId = _sessionContext.CreateLoginSession(user.Id_User, deviceInfo);

            if (sessionId == 0)
            {
                return StatusCode(500, new { success = false, message = "Gagal menyimpan sesi login." });
            }

            JWTHelper jwtHelper = new JWTHelper(_config);
            var token = jwtHelper.GenerateToken(user);

            return Ok(new
            {
                token,
                session_id = sessionId,
                user = new
                {
                    user.Id_User,
                    user.Username,
                    user.Email,
                    user.No_hp,
                    user.Alamat,
                    user.Password
                }
            });
        }
    }
}
