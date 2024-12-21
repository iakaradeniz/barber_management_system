using barber_management_system.Data;
using barber_management_system.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace barber_management_system.Controllers
{
    public class CalisanController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public CalisanController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult Add()
        {
            // Yeni çalışan ekleme formunu döner
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Add(CalisanViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Hatalı form gönderimi varsa tekrar formu göster
            }

            // Aynı ad-soyad kontrolü
            bool isSameNameExists = await _dbContext.Calisanlar
                .AnyAsync(c => c.CalisanAd == model.CalisanAd && c.CalisanSoyad == model.CalisanSoyad);

            if (isSameNameExists)
            {
                ModelState.AddModelError("", "Bu ad ve soyada sahip bir çalışan zaten mevcut.");
                return View(model);
            }

         

            // Yeni çalışan oluşturma
            var calisan = new Calisan
            {
               
                CalisanAd = model.CalisanAd,
                CalisanSoyad = model.CalisanSoyad,
                
            };
           
            await _dbContext.Calisanlar.AddAsync(calisan);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("List", "Calisan"); // İşlem sonrası listeleme sayfasına yönlendirme
        }


        // Tüm çalışanları listele
       
        
            [HttpGet]
            public async Task<IActionResult> List()
            {
                var calisanlar = await _dbContext.Calisanlar
                    //.Include(c => c.CalismaSaatleri) // Çalışma saatlerini dahil et
                    .ToListAsync();
                return View(calisanlar);

            }
        

        


       

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var calisan = await _dbContext.Calisanlar.FindAsync(id);
                


            if (calisan == null)
            {
                return NotFound("Çalışan bulunamadı.");
            }

            return View(calisan); 
        }


      
      

        [HttpPost]
       
        public async Task<IActionResult> Edit(Calisan viewModel)
        {
            var calisan = await _dbContext.Calisanlar
                .FindAsync(viewModel.CalisanID);

           

            if (calisan is not null)
            {
                // Ad ve soyad kontrolü
                if (_dbContext.Calisanlar.Any(c => c.CalisanAd == viewModel.CalisanAd && c.CalisanSoyad == viewModel.CalisanSoyad && c.CalisanID != viewModel.CalisanID))
                {
                    ModelState.AddModelError("", "Bu ad ve soyadla başka bir çalışan mevcut.");
                    return View(viewModel);
                }
                else
                {
                    calisan.CalisanAd = viewModel.CalisanAd;
                    calisan.CalisanSoyad = viewModel.CalisanSoyad;
                    // Değişiklikleri kaydet
                    await _dbContext.SaveChangesAsync();

                }

            }

            return RedirectToAction("List", "Calisan");
        }





        [HttpPost]
        public async Task<IActionResult> Delete(Calisan viewmodel)
        {
            var calisan = await _dbContext.Calisanlar
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CalisanID == viewmodel.CalisanID);
            if (calisan is not null)
            {
                _dbContext.Calisanlar.Remove(viewmodel);
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("List", "Calisan");
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetCalisanById(int id)
        {
            var calisan = await _dbContext.Calisanlar
                .Include(c => c.CalismaSaatleri) // Çalışma saatlerini dahil et
                .FirstOrDefaultAsync(c => c.CalisanID == id);

            if (calisan == null) return NotFound("Çalışan bulunamadı.");

            return Ok(calisan);

        }

    }
}



























//[HttpPost]
//public async Task<IActionResult> Edit(int id, CalisanViewModel model)
//{
//    if (!ModelState.IsValid)
//    {
//        return View(model); // Hatalı form gönderimi varsa formu tekrar göster
//    }

//    // Veritabanında düzenlenecek çalışanı bul
//    var mevcutCalisan = await _dbContext.Calisanlar
//        .Include(c => c.CalismaSaatleri) // Çalışma saatlerini yükle
//        .FirstOrDefaultAsync(c => c.CalisanID == id);

//    if (mevcutCalisan == null)
//    {
//        return NotFound(); // Çalışan bulunamadıysa 404 döndür
//    }

//    // 1. Ad ve soyad kontrolü
//    var ayniAdSoyadliCalisan = await _dbContext.Calisanlar
//        .FirstOrDefaultAsync(c => c.CalisanAd == model.CalisanAd
//                                && c.CalisanSoyad == model.CalisanSoyad
//                                && c.CalisanI != id); // Aynı kişi değilse kontrol et

//    if (ayniAdSoyadliCalisan != null)
//    {
//        ModelState.AddModelError(string.Empty, "Bu ad ve soyada sahip başka bir çalışan mevcut.");
//        return View(model); // Hata mesajıyla birlikte formu tekrar göster
//    }

//    // 2. Çalışma saatleri çakışma kontrolü
//    if (model.CalismaSaatleri != null && model.CalismaSaatleri.Any())
//    {
//        foreach (var yeniSaat in model.CalismaSaatleri)
//        {
//            bool cakismaVar = mevcutCalisan.CalismaSaatleri.Any(cs =>
//                cs.Gun == yeniSaat.Gun && // Aynı gün
//                (yeniSaat.BaslangicSaati < cs.BitisSaati && yeniSaat.BitisSaati > cs.BaslangicSaati)); // Çakışan aralık

//            if (cakismaVar)
//            {
//                ModelState.AddModelError(string.Empty, $"Çalışma saatleri çakışması: {yeniSaat.Gun} günü için aralığı kontrol edin.");
//                return View(model); // Hata mesajıyla birlikte formu tekrar göster
//            }
//        }
//    }

//    // 3. Çalışanın bilgilerini güncelle
//    mevcutCalisan.CalisanAd = model.CalisanAd;
//    mevcutCalisan.CalisanSoyad = model.CalisanSoyad;

//    // 4. Çalışma saatlerini güncelle
//    mevcutCalisan.CalismaSaatleri.Clear(); // Önce eski saatleri temizle
//    if (model.CalismaSaatleri != null)
//    {
//        mevcutCalisan.CalismaSaatleri = model.CalismaSaatleri.Select(cs => new CalismaSaati
//        {
//            Gun = cs.Gun,
//            BaslangicSaati = cs.BaslangicSaati,
//            BitisSaati = cs.BitisSaati
//        }).ToList();
//    }

//    // 5. Veritabanını kaydet
//    _dbContext.Calisanlar.Update(mevcutCalisan);
//    await _dbContext.SaveChangesAsync();

//    return RedirectToAction("List", "Calisan"); // İşlem sonrası liste sayfasına yönlendir
//}


//[HttpPost]
//[ValidateAntiForgeryToken]
//public async Task<IActionResult> Edit(CalisanViewModel viewModel)
//{
//    var calisan = await _dbContext.Calisanlar
//        .Include(c => c.CalismaSaatleri)
//        .FirstOrDefaultAsync(c => c.CalisanID == viewModel.CalisanId);

//    if (calisan is not null)
//    {
//        // Ad ve soyad kontrolü
//        if (_dbContext.Calisanlar.Any(c => c.CalisanAd == viewModel.CalisanAd && c.CalisanSoyad == viewModel.CalisanSoyad && c.CalisanID != viewModel.CalisanId))
//        {
//            ModelState.AddModelError("", "Bu ad ve soyadla başka bir çalışan mevcut.");
//            return View(viewModel);
//        }

//        calisan.CalisanAd = viewModel.CalisanAd;
//        calisan.CalisanSoyad = viewModel.CalisanSoyad;

//        //foreach (var calismaSaati in viewModel.CalismaSaatleri)
//        //{
//        //    // Çalışma saati çakışma kontrolü
//        //    if (calisan.CalismaSaatleri.Any(existing =>
//        //        existing.Gun == calismaSaati.Gun &&
//        //        ((calismaSaati.BaslangicSaati >= existing.BaslangicSaati && calismaSaati.BaslangicSaati < existing.BitisSaati) ||
//        //        (calismaSaati.BitisSaati > existing.BaslangicSaati && calismaSaati.BitisSaati <= existing.BitisSaati) ||
//        //        (calismaSaati.BaslangicSaati <= existing.BaslangicSaati && calismaSaati.BitisSaati >= existing.BitisSaati))))
//        //    {
//        //        ModelState.AddModelError("", $"Çalışma saati çakışması tespit edildi: {calismaSaati.Gun}, {calismaSaati.BaslangicSaati}-{calismaSaati.BitisSaati}");
//        //        return View(viewModel);
//        //    }
//        //}


//        // Mevcut çalışma saatlerini güncelle veya yeni saat ekle
//        foreach (var calismaSaati in viewModel.CalismaSaatleri)
//        {
//            var existingSaati = calisan.CalismaSaatleri
//                .FirstOrDefault(c => c.CalismaSaatiID == calismaSaati.CalismaSaatiId);


//            // Yeni çalışma saati ekle
//            calisan.CalismaSaatleri.Add(new CalismaSaati
//            {
//                Gun = calismaSaati.Gun,
//                BaslangicSaati = calismaSaati.BaslangicSaati,
//                BitisSaati = calismaSaati.BitisSaati
//            });

//        }

//        // Çalışma saati formdan kaldırıldıysa sil
//        var removedHours = calisan.CalismaSaatleri
//            .Where(existing => !viewModel.CalismaSaatleri
//                .Any(vm => vm.CalismaSaatiId == existing.CalismaSaatiID)) // viewModel'de bulunmayan
//            .ToList();

//        // Kaldırılmış olan çalışma saatlerini sil
//        foreach (var removedHour in removedHours)
//        {
//            _dbContext.CalismaSaatleri.Remove(removedHour);
//        }

//        try
//        {
//            // Veritabanı güncellenmesi
//            await _dbContext.SaveChangesAsync();
//            TempData["SuccessMessage"] = "Çalışan başarıyla güncellendi.";
//            return RedirectToAction("List", "Calisan");
//        }
//        catch (DbUpdateException)
//        {
//            ModelState.AddModelError("", "Veritabanı güncellemesi sırasında bir hata oluştu.");
//            return View(viewModel);
//        }
//    }

//    return NotFound("Çalışan bulunamadı.");
//}







