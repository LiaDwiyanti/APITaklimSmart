using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APITaklimSmart.Models;
using Microsoft.AspNetCore.Authorization;

namespace APITaklimSmart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PenjadwalanController : ControllerBase
    {
        private readonly PenjadwalanContext _penjadwalanContext;
        public PenjadwalanController(PenjadwalanContext penjadwalanContext)
        {
            _penjadwalanContext = penjadwalanContext;
        }

        [HttpGet("read")]
        [Authorize]
        public IActionResult ReadPenjadwalan()
        {
            try
            {
                var data = _penjadwalanContext.ReadPenjadwalan();
                if (data == null)
                {
                    return NotFound(new { status = false, message = "Data penjadwalan tidak ditemukan" });
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("read/{id}")]
        [Authorize]
        public ActionResult<Penjadwalan> GetPenjadwalanById(int id)
        {
            try
            {
                var data = _penjadwalanContext.ReadPenjadwalanById(id);

                if (data == null)
                {
                    return NotFound(new {status = false, message = "Data penjadwalan tidak ditemukan"});
                }

                return data;
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("create")]
        [Authorize(Roles = "admin")]
        public IActionResult TambahJadwal([FromBody] PenjadwalanRequest input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new {success = false, message = "Data tidak valid"});
            }

            int userId = int.Parse(User.FindFirst("id_user")?.Value ?? "0");

            var penjadwalan = new Penjadwalan
            {
                Nama_Penjadwalan = input.Nama_Penjadwalan,
                Tanggal_Penjadwalan = input.Tanggal_Penjadwalan,
                Waktu_Penjadwalan = input.Waktu_Penjadwalan,
                Id_Lokasi = input.Id_Lokasi,
                Deskripsi_Penjadwalan = input.Deskripsi_Penjadwalan,
                Status_Penjadwalan = StatusPenjadwalan.Diproses,
                Created_By = userId,
                Created_At = DateTime.UtcNow,
            };

            var riwayat = new Riwayat
            { 
                Status_Lama = null,
                Status_Baru = StatusPenjadwalan.Diproses,
                Changed_By = userId,
                Alasan = null,
                Changed_At = DateTime.UtcNow
            };

            bool isSuccess = _penjadwalanContext.CreatePenjadwalan(penjadwalan, riwayat);

            if (isSuccess)
            {
                return Ok(new { status = true, message = "Tambah data penjadwalan berhasil." });
            }
            else
            {
                return StatusCode(500, new { status = false, message = "Gagal menambah data penjadwalan." });
            }
        }

        [HttpPut("edit/{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Update(int id, Penjadwalan input)
        {
            try
            {

                var jadwal = new Penjadwalan
                {
                    Id_Penjadwalan = id,
                    Nama_Penjadwalan = input.Nama_Penjadwalan,
                    Tanggal_Penjadwalan = input.Tanggal_Penjadwalan,
                    Waktu_Penjadwalan = input.Waktu_Penjadwalan,
                    Id_Lokasi = input.Id_Lokasi,
                    Deskripsi_Penjadwalan = input.Deskripsi_Penjadwalan,
                    Status_Penjadwalan = input.Status_Penjadwalan,
                    Updated_At = DateTime.UtcNow
                };

                bool isSuccess = _penjadwalanContext.UpdatePenjadwalan(jadwal);

                if (isSuccess)
                {
                    return Ok(new { status = true, message = "Penjadwalan berhasil diperbarui" });
                }
                else
                {
                    return NotFound(new { status = false, message = "Data gagal diperbarui" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = "Terjadi kesalahan", error = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = _penjadwalanContext.DeletePenjadwalanById(id);

                if (success)
                {
                    return Ok(new { status = true, message = "Penjadwalan berhasil dihapus"});
                }
                else
                {
                    return NotFound(new { status = false, message = "Penjadwalan tidak ditemukan atau sudah dihapus"});
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = "Terjadi kesalahan saat menghapus penjadwalan", error = ex.Message});
            }
        }
    }
}
