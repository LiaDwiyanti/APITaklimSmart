using APITaklimSmart.Models;
using APITaklimSmart.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static APITaklimSmart.Models.Enums;

namespace APITaklimSmart.Controllers
{
    public class DocumentationController : Controller
    {
        private readonly DokumentasiContext _dokumentasiContext;
        public DocumentationController(DokumentasiContext dokumentasiContext)
        {
            _dokumentasiContext = dokumentasiContext;
        }

        [HttpPost("dokumentasi/create")]
        public IActionResult TambahDokumentasi([FromBody] Dokumentasi input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dokumentasi = new Dokumentasi
            {
                Id_Penjadwalan = input.Id_Penjadwalan,
                Uploaded_By = input.Uploaded_By,
                File_Name = input.File_Name,
                File_Url = input.File_Url,
                File_Path = input.File_Path,
                Caption_Dokumentasi = input.Caption_Dokumentasi,
                Upload_Date = DateTime.UtcNow
            };
            bool isSuccess = _dokumentasiContext.InsertDokumentasi(dokumentasi);

            if (isSuccess)
            {
                return Ok(new { message = "Tambah data dokumentasi berhasil." });
            }
            else
            {
                return StatusCode(500, new { message = "Tambah data dokumentasi gagal" });
            }
        }

        [HttpGet("dokumentasi/read")]
        public IActionResult ReadDokumentasi()
        {
            try
            {
                var data = _dokumentasiContext.ReadDokumentasi();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("dokumentasi/uploadfile")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var savePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok(new
                {
                    file_name = uniqueFileName,
                    file_url = $"{Request.Scheme}://{Request.Host}",
                    file_path = $"/uploads/"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "File upload failed: " + ex.Message });
            }
        }

        [HttpPut("dokumentasi/edit")]
        public IActionResult EditDokumentasi([FromBody] Dokumentasi input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            bool result = _dokumentasiContext.EditDokumentasi(input);
            if (result)
                return Ok(new { message = "Berhasil mengedit data dokumentasi" });
            else
                return StatusCode(500, new { message = "Gagal mengedit data dokumentasi" });
        }

        [HttpDelete("dokumentasi/delete/{id}")]
        public IActionResult DeleteDokumentasi(int id)
        {
            bool result = _dokumentasiContext.DeleteDokumentasi(id);
            if (result)
                return Ok(new { message = "Berhasil menghapus data dan file dokumentasi" });
            else
                return StatusCode(500, new { message = "Gagal menghapus data dokumentasi" });
        }

    }
}
