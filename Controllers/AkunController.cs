using APITaklimSmart.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace APITaklimSmart.Controllers
{
    public class AkunController : ControllerBase
    {
        private readonly UserContext _userContext;

        public AkunController(UserContext userContext)
        {
            _userContext = userContext;
        }

        [HttpGet("akun")]
        [Authorize]
        public IActionResult GetProfile()
        {
            try
            {
                int userId = int.Parse(User.FindFirst("id_user")?.Value ?? "0");

                var user = _userContext.getUserById(userId);

                if (user == null)
                {
                    return NotFound(new { status = false, message = "User tidak ditemukan" });
                }

                return Ok(new
                {
                    status = true,
                    data = new
                    {
                        user.Id_User,
                        user.Username,
                        user.Alamat,
                        user.Email,
                        user.No_hp,
                        user.User_Role
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = "Terjadi kesalahan", error = ex.Message });
            }
        }
    }
}
