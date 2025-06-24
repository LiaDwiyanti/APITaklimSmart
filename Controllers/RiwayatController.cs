using APITaklimSmart.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APITaklimSmart.Controllers
{
    public class RiwayatController : Controller
    {
        private readonly RiwayatContext _riwayatContext;
 
        public RiwayatController(RiwayatContext riwayatContext)
        {
            _riwayatContext = riwayatContext;
        }

        [HttpGet("riwayat/read")]
        [Authorize]
        public IActionResult ReadListRiwayat()
        {
            try
            {
                _riwayatContext.UpdateStatusDiprosesKeDisetujuiJikaSudahLewat();

                var data = _riwayatContext.ReadRiwayatAll();
                if (data == null)
                {
                    return NotFound(new { status = false, message = "Data riwayat tidak ditemukan" });
                }
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPatch("update-status/{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult UpdateStatus(int id, [FromBody] UpdateRiwayatRequest input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = false, message = "Data tidak valid" });
            }

            try
            {
                int userId = int.Parse(User.FindFirst("id_user")?.Value ?? "0");

                var riwayat = new UpdateRiwayatRequest
                {
                    Status_Baru = input.Status_Baru,
                    Changed_By = userId,
                    Changed_At = input.Changed_At
                };

                bool isSuccess = _riwayatContext.UpdateStatusRiwayat(id, input);

                if (isSuccess)
                {
                    return Ok(new { status = true, message = "Status riwayat berhasil diperbarui." });
                }
                else
                {
                    return NotFound(new { status = false, message = "Riwayat tidak ditemukan atau gagal diperbarui." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = "Terjadi kesalahan.", error = ex.Message });
            }
        }
    }
}
