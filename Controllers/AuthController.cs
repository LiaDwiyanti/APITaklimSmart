using APITaklimSmart.Helpers;
using APITaklimSmart.Models;
using APITaklimSmart.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static APITaklimSmart.Models.Enums;

namespace APITaklimSmart.Controllers
{
    public class AuthController : Controller
    {
        private readonly DBContext _context;
        private readonly MapBoxService _mapbox;
        private readonly IConfiguration _config;

        public AuthController(DBContext context, MapBoxService mapbox, IConfiguration config)
        {
            _context = context;
            _mapbox = mapbox;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.No_hp == input.No_hp);
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

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
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

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                var lokasi = new Lokasi
                {
                    Nama_Lokasi = "Rumah " + input.Username,
                    Alamat = alamatFinal,
                    Latitude = (decimal)lat,
                    Longitude = (decimal)lon,
                    Deskripsi_Lokasi = "Lokasi dari user baru",
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Lokasis.AddAsync(lokasi);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { success = true, message = "Registrasi berhasil." });
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
                await transaction.RollbackAsync();
                return StatusCode(500, new { success = false, message = "Registrasi gagal. Silakan coba lagi." });
            }
        }

        [HttpPost("login")]
        public  async Task<IActionResult> Login([FromBody] Login input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == input.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(input.Password, user.Password))
            {
                return Unauthorized(new { success = false, message = "Username atau password salah." });
            }

            // Mengambil device info dari header
            string deviceInfo = Request.Headers["User-Agent"].ToString();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var session = new UserSession
                {
                    Id_User = user.Id_User,
                    LoginAt = DateTime.UtcNow,
                    DeviceInfo = deviceInfo
                };

                await _context.UserSessions.AddAsync(session);
                await _context.SaveChangesAsync();

                JWTHelper jwtHelper = new JWTHelper(_config);
                var token = jwtHelper.GenerateToken(user);

                await transaction.CommitAsync();

                return Ok(new
                {
                    token,
                    session_id = session.Id_Session,
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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await transaction.RollbackAsync();
                return StatusCode(500, new { success = false, message = "Login gagal. Silakan coba lagi." });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] int session_id)
        {
            var userId = int.Parse(User.FindFirst("id_user").Value);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var session = await _context.UserSessions
                    .FirstOrDefaultAsync(s => s.Id_Session == session_id && s.Id_User == userId);

                if (session == null)
                {
                    return NotFound(new { success = false, message = "Sesi tidak ditemukan." });
                }

                if (session.LogoutAt != null)
                {
                    return BadRequest(new { success = false, message = "Sesi sudah logout sebelumnya." });
                }

                session.LogoutAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { success = true, message = "Logout berhasil." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await transaction.RollbackAsync();
                return StatusCode(500, new { success = false, message = "Logout gagal. Silakan coba lagi." });
            }
        }
    }
}
