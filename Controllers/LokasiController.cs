using APITaklimSmart.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APITaklimSmart.Controllers
{
    public class LokasiController : ControllerBase
    {
        private readonly PenjadwalanContext _penjadwalanContext;
        private readonly LokasiContext _lokasiContext;
        public LokasiController(PenjadwalanContext penjadwalanContext, LokasiContext lokasiContext)
        {
            _penjadwalanContext = penjadwalanContext;
            _lokasiContext = lokasiContext;
        }
        [HttpGet("lokasi-terdekat")]
        [Authorize]
        public IActionResult GetLokasiTerdekat()
        {
            try
            {
                var penjadwalan = _penjadwalanContext.GetPenjadwalanTerdekat();
                if (penjadwalan == null)
                {
                    return NotFound(new { status = false, message = "Tidak ada penjadwalan terdekat." });
                }

                var lokasi = _lokasiContext.ReadLokasiById(penjadwalan.Id_Lokasi);
                if (lokasi == null)
                {
                    return NotFound(new { status = false, message = "Lokasi tidak ditemukan." });
                }

                return Ok(new
                {
                    status = true,
                    data = new
                    {
                        lokasi,
                        penjadwalan = new
                        {
                            penjadwalan.Id_Penjadwalan,
                            penjadwalan.Nama_Penjadwalan,
                            penjadwalan.Tanggal_Penjadwalan,
                            penjadwalan.Waktu_Penjadwalan
                        }
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
