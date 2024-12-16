using barber_management_system.Data;
using barber_management_system.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace barber_management_system.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalismaSaatiController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CalismaSaatiController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Çalışma saatlerini listele
        [HttpGet]
        public async Task<IActionResult> GetCalismaSaatleri()
        {
            var calismaSaatleri = await _dbContext.CalismaSaatleri
                .Include(cs => cs.Calisan) // Çalışan bilgilerini dahil et
                .ToListAsync();
            return Ok(calismaSaatleri);
        }

        // Yeni çalışma saati ekle
        [HttpPost("{calisanId}")]
        public async Task<IActionResult> AddCalismaSaati(int calisanId, [FromBody] CalismaSaati calismaSaati)
        {
            var calisan = await _dbContext.Calisanlar.FindAsync(calisanId);
            if (calisan == null) return NotFound("Çalışan bulunamadı.");

            // Aynı gün için çakışma kontrolü
            var existingCalismaSaatleri = await _dbContext.CalismaSaatleri
                .Where(cs => cs.CalisanId == calisanId && cs.Gun == calismaSaati.Gun)
                .ToListAsync();

            bool isOverlapping = existingCalismaSaatleri.Any(cs =>
                calismaSaati.BaslangicSaati < cs.BitisSaati && calismaSaati.BitisSaati > cs.BaslangicSaati);

            if (isOverlapping)
            {
                return BadRequest("Bu gün için çalışma saatleri çakışıyor.");
            }

            calismaSaati.CalisanId = calisanId;

            await _dbContext.CalismaSaatleri.AddAsync(calismaSaati);
            await _dbContext.SaveChangesAsync();

            return Ok("Çalışma saati başarıyla eklendi.");
        }

        // Çalışma saati güncelle
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCalismaSaati(int id, [FromBody] CalismaSaati calismaSaati)
        {
            var existingCalismaSaati = await _dbContext.CalismaSaatleri.FindAsync(id);
            if (existingCalismaSaati == null) return NotFound("Çalışma saati bulunamadı.");

            existingCalismaSaati.Gun = calismaSaati.Gun;
            existingCalismaSaati.BaslangicSaati = calismaSaati.BaslangicSaati;
            existingCalismaSaati.BitisSaati = calismaSaati.BitisSaati;

            await _dbContext.SaveChangesAsync();

            return Ok("Çalışma saati başarıyla güncellendi.");
        }

        // Çalışma saati sil
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCalismaSaati(int id)
        {
            var calismaSaati = await _dbContext.CalismaSaatleri.FindAsync(id);
            if (calismaSaati == null) return NotFound("Çalışma saati bulunamadı.");

            _dbContext.CalismaSaatleri.Remove(calismaSaati);
            await _dbContext.SaveChangesAsync();

            return Ok("Çalışma saati başarıyla silindi.");
        }
    }

}
