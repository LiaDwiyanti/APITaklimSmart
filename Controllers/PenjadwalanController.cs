using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APITaklimSmart.Models;

namespace APITaklimSmart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PenjadwalanController : ControllerBase
    {
        private readonly DBContext _context;

        public PenjadwalanController(DBContext context)
        {
            _context = context;
        }

        // GET: api/penjadwalan
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Penjadwalan>>> GetAll()
        {
            return await _context.Penjadwalans.ToListAsync();
        }

        // GET: api/penjadwalan/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Penjadwalan>> GetById(int id)
        {
            var data = await _context.Penjadwalans.FindAsync(id);

            if (data == null)
                return NotFound();

            return data;
        }

        // POST: api/penjadwalan
        [HttpPost]
        public async Task<ActionResult<Penjadwalan>> Create(Penjadwalan item)
        {
            item.Created_At = DateTime.Now;
            item.Updated_At = DateTime.Now;

            _context.Penjadwalans.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = item.Id_Penjadwalan }, item);
        }

        // PUT: api/penjadwalan/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Penjadwalan item)
        {
            if (id != item.Id_Penjadwalan)
                return BadRequest();

            var existing = await _context.Penjadwalans.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.Nama_Penjadwalan = item.Nama_Penjadwalan;
            existing.Tanggal_Penjadwalan = item.Tanggal_Penjadwalan;
            existing.Waktu_Penjadwalan = item.Waktu_Penjadwalan;
            existing.Id_Lokasi = item.Id_Lokasi;
            existing.Deskripsi_Penjadwalan = item.Deskripsi_Penjadwalan;
            existing.Status_Penjadwalan = item.Status_Penjadwalan;
            existing.Created_By = item.Created_By;
            existing.Updated_At = DateTime.Now;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/penjadwalan/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var data = await _context.Penjadwalans.FindAsync(id);
            if (data == null)
                return NotFound();

            _context.Penjadwalans.Remove(data);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
