using barber_management_system.Data;
using barber_management_system.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace barber_management_system.Controllers
{
    
    public class CalismaSaatiController:Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CalismaSaatiController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }



        // Çalışma saatlerini listele
        [Authorize(Roles = "Calisan,Admin")]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var calisanlar = await _dbContext.Calisanlar
                     .Include(c => c.CalismaSaatleri) // Çalışma saatlerini dahil et
                     .ToListAsync();
            return View(calisanlar);
        }


        // GET: CalismaSaati/Add/{calisanId}
        [Authorize(Roles = "Admin")]
        [HttpGet("CalismaSaati/Add/{calisanId}")]
        public IActionResult Add(int calisanId)
        {
            var calisan = _dbContext.Calisanlar.Find(calisanId);
            if (calisan == null)
            {
                return NotFound("Çalışan bulunamadı.");
            }

            var model = new CalismaSaati
            {
                CalisanId = calisanId
            };

            
           
            return View(model); // View'a çalışan id'si ile birlikte model gönderilir
        }


        // POST: CalismaSaati/Add/{calisanId}
        [Authorize(Roles = "Admin")]
        [HttpPost("CalismaSaati/Add/{calisanId}")]
        public async Task<IActionResult> Add(int calisanId, [FromForm] CalismaSaati calismaSaati)
        {
            var calisan = await _dbContext.Calisanlar.FindAsync(calisanId);
            if (calisan == null)
            {
                return NotFound("Çalışan bulunamadı.");
            }

          
            // Aynı gün için çakışma kontrolü (sadece saat ve dakika dikkate alınarak)
            var isOverlapping = await _dbContext.CalismaSaatleri
                .AnyAsync(cs => cs.CalisanId == calisanId && cs.Gun == calismaSaati.Gun &&
                                (calismaSaati.BaslangicSaati.TimeOfDay < cs.BitisSaati.TimeOfDay) &&
                                (calismaSaati.BitisSaati.TimeOfDay > cs.BaslangicSaati.TimeOfDay));

            var issame = await _dbContext.CalismaSaatleri
               .AnyAsync(cs => cs.CalisanId == calisanId && cs.Gun == calismaSaati.Gun &&
                               (calismaSaati.BaslangicSaati.TimeOfDay == cs.BaslangicSaati.TimeOfDay) &&
                               (calismaSaati.BitisSaati.TimeOfDay == cs.BitisSaati.TimeOfDay));

            if(issame)
            {
                TempData["ErrorMessage"] = "Bu kayıt zaten mevcut.";
                return View(calismaSaati); // Çakışma varsa formu tekrar göster
            }



            if (isOverlapping)
            {
                TempData["ErrorMessage"] = "Bu gün için çalışma saatleri çakışıyor.";
                return View(calismaSaati); // Çakışma varsa formu tekrar göster
            }

            calismaSaati.CalisanId = calisanId;

            if (calisan.CalismaSaatleri == null)
            {
                calisan.CalismaSaatleri = new List<CalismaSaati>();
            }
            calisan.CalismaSaatleri.Add(calismaSaati);

            await _dbContext.CalismaSaatleri.AddAsync(calismaSaati);
            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Çalışma saati başarıyla eklendi.";
            return RedirectToAction("List", "CalismaSaati"); // Listeleme sayfasına yönlendir
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("CalismaSaati/Edit/{calismaSaatiId}/{calisanId}")]
        public IActionResult Edit(int calismaSaatiId,int calisanId)
        {
            var calisan = _dbContext.Calisanlar.Find(calisanId);
            if (calisan == null)
            {
                return NotFound("Çalışan bulunamadı.");
            }

            var model = new CalismaSaati
            {
                CalisanId = calisanId // Formu çalışanla ilişkilendirecek
            };

            return View(model); // Çalışma saati formunu render eder
        }


        // POST: CalismaSaati/Edit/{calisanId}
        [Authorize(Roles = "Admin")]
        [HttpPost("CalismaSaati/Edit/{calismaSaatiId}/{calisanId}")]
        public async Task<IActionResult> Edit(int calismaSaatiId,int calisanId, [FromForm] CalismaSaati vievModel)
        {
            var calisan = await _dbContext.Calisanlar.FindAsync(calisanId);
            if (calisan == null)
            {
                return NotFound("Çalışan bulunamadı.");
            }


            var issame = await _dbContext.CalismaSaatleri
                .AnyAsync(cs => cs.CalisanId == calisanId && cs.Gun == vievModel.Gun &&
                                             vievModel.BaslangicSaati.TimeOfDay == cs.BaslangicSaati.TimeOfDay &&
                                              vievModel.BitisSaati.TimeOfDay == cs.BitisSaati.TimeOfDay);
            if(issame)
            {
                TempData["ErrorMessage"] = "Bu kayıt zaten mevcut.";
                return View(vievModel); // Çakışma varsa formu tekrar göster

            }

            var calismaSaati = await _dbContext.CalismaSaatleri.FindAsync(calismaSaatiId);
            calismaSaati.Gun = vievModel.Gun;
            calismaSaati.BaslangicSaati = vievModel.BaslangicSaati;
            calismaSaati.BitisSaati = vievModel.BitisSaati;
            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Çalışma saati başarıyla güncellendi.";

            return RedirectToAction("List", "CalismaSaati"); // Listeleme sayfasına yönlendir
        }


        // Çalışma saati sil
        [Authorize(Roles = "Admin")]
        [HttpGet("/CalismaSaati/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var calismaSaati = await _dbContext.CalismaSaatleri.FindAsync(id);
            if (calismaSaati == null)
            {
                return NotFound("Çalışma saati bulunamadı.");
            }
            var calisan = await _dbContext.Calisanlar.FindAsync(calismaSaati.CalisanId);
            calisan.CalismaSaatleri.Remove(calismaSaati);

            _dbContext.CalismaSaatleri.Remove(calismaSaati);
            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Çalışma saati başarıyla silindi.";

            return RedirectToAction("List", "CalismaSaati", new { calisanId = calismaSaati.CalisanId }); // Listeleme sayfasına yönlendir
        }





        // Çalışma saati güncelle
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
