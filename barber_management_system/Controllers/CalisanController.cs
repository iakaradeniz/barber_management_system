using barber_management_system.Data;
using barber_management_system.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace barber_management_system.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalisanController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CalisanController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Tüm çalışanları listele
        [HttpGet]
        public async Task<IActionResult> GetCalisanlar()
        {
            var calisanlar = await _dbContext.Calisanlar
                .Include(c => c.CalismaSaatleri) // Çalışma saatlerini dahil et
                .ToListAsync();
            return Ok(calisanlar);
            
        }

        // Yeni çalışan ekle
        [HttpPost]
        public async Task<IActionResult> AddCalisan([FromBody] Calisan calisan)
        {
            if (calisan == null) return BadRequest("Çalışan bilgileri eksik.");

            await _dbContext.Calisanlar.AddAsync(calisan);
            await _dbContext.SaveChangesAsync();

            return Ok("Çalışan başarıyla eklendi.");
        }

        // Çalışan ve çalışma saatlerini getir
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCalisanById(int id)
        {
            var calisan = await _dbContext.Calisanlar
                .Include(c => c.CalismaSaatleri) // Çalışma saatlerini dahil et
                .FirstOrDefaultAsync(c => c.CalisanID == id);

            if (calisan == null) return NotFound("Çalışan bulunamadı.");

            return Ok(calisan);
            return Ok();

        }

        // Çalışan güncelle
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCalisan(int id, [FromBody] Calisan calisan)
        {
            var existingCalisan = await _dbContext.Calisanlar
                .FirstOrDefaultAsync(c => c.CalisanID == id);

            if (existingCalisan == null) return NotFound("Çalışan bulunamadı.");

            // Çalışan bilgilerini güncelle
            existingCalisan.CalisanAd = calisan.CalisanAd;
            existingCalisan.CalisanSoyad = calisan.CalisanSoyad;

            await _dbContext.SaveChangesAsync();

            return Ok("Çalışan başarıyla güncellendi.");
        }

        // Çalışan sil
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCalisan(int id)
        {
            var calisan = await _dbContext.Calisanlar
                .Include(c => c.CalismaSaatleri) // Çalışma saatleri de dahil
                .FirstOrDefaultAsync(c => c.CalisanID == id);

            if (calisan == null) return NotFound("Çalışan bulunamadı.");

            // Çalışma saatleri otomatik olarak silinir çünkü ilişkili tablo için Cascade silme etkin
            _dbContext.Calisanlar.Remove(calisan);
            await _dbContext.SaveChangesAsync();

            return Ok("Çalışan başarıyla silindi.");
        }
    }





}
