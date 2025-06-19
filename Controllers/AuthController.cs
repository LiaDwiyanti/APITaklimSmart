using APITaklimSmart.Helpers;
using APITaklimSmart.Models;
using APITaklimSmart.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static APITaklimSmart.Models.Enums;

namespace APITaklimSmart.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserContext _userContext;
        private readonly UserSessionContext _sessionContext;
        private readonly MapBoxService _mapbox;
        private readonly IConfiguration _config;

        public AuthController(UserContext userContext, UserSessionContext sessionContext, MapBoxService mapbox, IConfiguration config)
        {
            _userContext = userContext;
            _sessionContext = sessionContext;
            _mapbox = mapbox;
            _config = config;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] Register input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Data tidak valid",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            User existingUser = _userContext.getUserByNoHP(input.No_hp);
            if (existingUser != null)
            {
                return Conflict(new { success = false, message = "Nomor HP sudah digunakan." });
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
                    return BadRequest(new { success = false, message = "Alamat tidak dapat ditemukan, mohon isi alamat dengan benar atau pilih di peta." });
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
                Latitude = lat,
                Longitude = lon,
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

        [HttpPost("login")]
        public IActionResult Login([FromBody] Login input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Data tidak valid",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            User user = _userContext.getUserByUsername(input.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(input.Password, user.Password))
            {
                return Unauthorized(new { success = false, message = "Username atau password salah." });
            }

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
                success = true,
                token,
                session_id = sessionId,
                user = new
                {
                    user.Id_User,
                    user.Username,
                    user.Email,
                    user.No_hp,
                    user.Alamat
                }
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            int userId = int.Parse(User.FindFirst("id_user")?.Value ?? "0");

            var session = _sessionContext.GetSessionById(userId);
            if (session == null)
            {
                return NotFound(new { success = false, message = "Sesi tidak ditemukan." });
            }

            if (session.LogoutAt != null)
            {
                return BadRequest(new { success = false, message = "Sesi sudah logout sebelumnya." });
            }

            bool updated = _sessionContext.LogoutSession(session.Id_Session);
            if (!updated)
            {
                return StatusCode(500, new { success = false, message = "Gagal logout." });
            }

            return Ok(new { success = true, message = "Logout berhasil." });
        }
    }
}
