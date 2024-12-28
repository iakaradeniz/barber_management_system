using barber_management_system.Data;
using barber_management_system.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace barber_management_system.Controllers
{
    [Authorize(Roles = "Admin, Calisan, Musteri")]
    public class RandevuController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        
        public RandevuController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _userManager = userManager;
           
        }

   

      

        [HttpGet("Randevu/Add")]
        public async Task<IActionResult> Add()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var musteri = await _dbContext.Musteriler.FirstOrDefaultAsync(m => m.IdentityUserId == user.Id);
            if (musteri == null)
            {
                return NotFound("Müşteri bulunamadı.");
            }

            var hizmetler = await _dbContext.Hizmetler.ToListAsync();
            var model = new RandevuViewModel
            {
                MusteriId = musteri.MusteriID,
                Hizmetler = hizmetler
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(RandevuViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var musteri = await _dbContext.Musteriler.FirstOrDefaultAsync(m => m.IdentityUserId == user.Id);
                if (musteri == null)
                {
                    ModelState.AddModelError("", "Müşteri bulunamadı.");
                    viewModel.Hizmetler = await _dbContext.Hizmetler.ToListAsync();
                    return View(viewModel);
                }

                // Seçilen çalışanı kontrol et
                if (viewModel.CalisanId == 0)
                {
                    ModelState.AddModelError("", "Lütfen bir çalışan seçiniz.");
                    viewModel.Hizmetler = await _dbContext.Hizmetler.ToListAsync();
                    return View(viewModel);
                }

                var secilenCalisan = await _dbContext.Calisanlar.FindAsync(viewModel.CalisanId);
                if (secilenCalisan == null)
                {
                    ModelState.AddModelError("", "Seçilen çalışan bulunamadı.");
                    viewModel.Hizmetler = await _dbContext.Hizmetler.ToListAsync();
                    return View(viewModel);
                }

                var hizmet = await _dbContext.Hizmetler.FindAsync(viewModel.HizmetId);
                if (hizmet == null)
                {
                    ModelState.AddModelError("", "Seçilen hizmet bulunamadı.");
                    viewModel.Hizmetler = await _dbContext.Hizmetler.ToListAsync();
                    return View(viewModel);
                }

                // Seçilen çalışanın bu hizmeti verip veremediğini kontrol et
                var calisanHizmetVar = await _dbContext.CalisanHizmetler
                    .AnyAsync(ch => ch.CalisanId == viewModel.CalisanId && ch.HizmetId == viewModel.HizmetId);

                if (!calisanHizmetVar)
                {
                    ModelState.AddModelError("", "Seçilen çalışan bu hizmeti verememektedir.");
                    viewModel.Hizmetler = await _dbContext.Hizmetler.ToListAsync();
                    return View(viewModel);
                }

                // Çalışanın müsaitlik kontrolü
                var calismaSaatleri = await _dbContext.CalismaSaatleri
                    .Where(cs => cs.CalisanId == viewModel.CalisanId && cs.Gun == viewModel.RandevuTarihi.DayOfWeek)
                    .ToListAsync();

                var hizmetSüresi = (int)hizmet.Dakika;
                var randevuBaslangic = viewModel.RandevuTarihi.TimeOfDay;
                var randevuBitis = randevuBaslangic.Add(TimeSpan.FromMinutes(hizmetSüresi));

                bool calisanMusait = false;
                foreach (var calismaSaati in calismaSaatleri)
                {
                    if (randevuBaslangic >= calismaSaati.BaslangicSaati.TimeOfDay &&
                        randevuBitis <= calismaSaati.BitisSaati.TimeOfDay)
                    {
                        // Çalışanın mevcut randevularını kontrol et
                        var mevcutRandevular = await _dbContext.Randevular
                            .Where(r => r.CalisanId == viewModel.CalisanId &&
                                   r.RandevuTarihi.Date == viewModel.RandevuTarihi.Date)
                            .ToListAsync();

                        calisanMusait = true;
                        foreach (var randevu in mevcutRandevular)
                        {
                            var mevcutRandevuBaslangic = randevu.RandevuTarihi.TimeOfDay;
                            var mevcutRandevuBitis = mevcutRandevuBaslangic.Add(TimeSpan.FromMinutes(randevu.Dakika));

                            if (randevuBaslangic < mevcutRandevuBitis && randevuBitis > mevcutRandevuBaslangic)
                            {
                                calisanMusait = false;
                                break;
                            }
                        }
                        break;
                    }
                }

                if (!calisanMusait)
                {
                    ModelState.AddModelError("", "Seçilen çalışan bu saatte müsait değil.");
                    viewModel.Hizmetler = await _dbContext.Hizmetler.ToListAsync();
                    return View(viewModel);
                }

                // Yeni randevu oluşturma
                var yeniRandevu = new Randevu
                {
                    MusteriId = musteri.MusteriID,
                    CalisanId = viewModel.CalisanId, // Müşterinin seçtiği çalışan
                    HizmetId = viewModel.HizmetId,
                    RandevuTarihi = viewModel.RandevuTarihi,
                    Dakika = (int)hizmet.Dakika,
                    Ucret = hizmet.Fiyat
                };

                _dbContext.Randevular.Add(yeniRandevu);
                await _dbContext.SaveChangesAsync();

                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    TempData["SuccessMessage"] = "Randevu başarıyla eklendi.";
                    return RedirectToAction("List", "Randevu");
                }
                else if (await _userManager.IsInRoleAsync(user, "Calisan"))
                {
                    TempData["SuccessMessage"] = "Randevu başarıyla eklendi.";
                    return RedirectToAction("List", "Randevu");
                }
                else if (await _userManager.IsInRoleAsync(user, "Musteri"))
                {
                    TempData["SuccessMessage"] = "Randevu başarıyla eklendi.";
                    return RedirectToAction("Musteri_Randevu_List", "Randevu");
                }
            }

            viewModel.Hizmetler = await _dbContext.Hizmetler.ToListAsync();
            return View(viewModel);
        }

        //[HttpPost("Randevu/GetCalisanlar/{randevuId}")]

        [HttpPost("Randevu/GetCalisanlar")]
        public async Task<IActionResult> GetCalisanlar(RandevuViewModel viewModel, string returnUrl)
        {
            if (viewModel.HizmetId == 0)
            {
                ModelState.AddModelError("", "Hizmet ID geçersiz.");
                return RedirectToAction("Add");
            }

            if (viewModel.RandevuTarihi == default)
            {
                ModelState.AddModelError("", "Randevu tarihi geçersiz.");
                return RedirectToAction("Add");
            }

            // Bu hizmeti verebilen çalışanları bul
            var calisanHizmetler = await _dbContext.CalisanHizmetler
                .Where(ch => ch.HizmetId == viewModel.HizmetId)
                .Select(ch => ch.CalisanId)
                .ToListAsync();

            var uygunCalisanListesi = new List<Calisan>();

            foreach (var calisanId in calisanHizmetler)
            {
                var calisanMusait = true;

                // Çalışanın çalışma saatlerini kontrol et
                var calismaSaatleri = await _dbContext.CalismaSaatleri
                    .Where(cs => cs.CalisanId == calisanId && cs.Gun == viewModel.RandevuTarihi.DayOfWeek)
                    .ToListAsync();

                var hizmetSüresi = (int)(await _dbContext.Hizmetler.FindAsync(viewModel.HizmetId)).Dakika;
                var randevuBaslangic = viewModel.RandevuTarihi.TimeOfDay;
                var randevuBitis = randevuBaslangic.Add(TimeSpan.FromMinutes(hizmetSüresi));
                //Burayı Kontrol et
                foreach (var calismaSaati in calismaSaatleri)
                {
                    if (randevuBaslangic >= calismaSaati.BaslangicSaati.TimeOfDay && randevuBitis <= calismaSaati.BitisSaati.TimeOfDay)
                    {
                        // Çalışan çalışma saatleri içerisinde ise, randevularını kontrol et
                        var mevcutRandevular = await _dbContext.Randevular
                            .Where(r => r.CalisanId == calisanId && r.RandevuTarihi.Date == viewModel.RandevuTarihi.Date)
                            .ToListAsync();

                        foreach (var randevu in mevcutRandevular)
                        {
                            var mevcutRandevuBaslangic = randevu.RandevuTarihi.TimeOfDay;
                            var mevcutRandevuBitis = mevcutRandevuBaslangic.Add(TimeSpan.FromMinutes(randevu.Dakika));

                            if (randevuBaslangic < mevcutRandevuBitis && randevuBitis > mevcutRandevuBaslangic)
                            {
                                calisanMusait = false;
                                break;
                            }
                        }

                        if (calisanMusait)
                        {
                            
                            var calisan = await _dbContext.Calisanlar.FindAsync(calisanId);
                            uygunCalisanListesi.Add(calisan);
                        }

                        break;
                    }
                }
            }

            if (!uygunCalisanListesi.Any())
            {
                ModelState.AddModelError("", "Seçilen tarihte uygun çalışan bulunamadı.");
                viewModel.Hizmetler = await _dbContext.Hizmetler.ToListAsync();
                return View("Add", viewModel);
            }

            viewModel.Calisanlar = uygunCalisanListesi;
            viewModel.Hizmetler = await _dbContext.Hizmetler.ToListAsync();
            var hizmet = await _dbContext.Hizmetler.FindAsync(viewModel.HizmetId);
            if (hizmet != null)
            {
                viewModel.Dakika = (int)hizmet.Dakika;
                viewModel.Fiyat = hizmet.Fiyat;
            }
            else
            {
                // Hizmet bulunamadığında yapılacak işlemler
                ModelState.AddModelError("", "Seçilen hizmet bulunamadı.");
                viewModel.Calisanlar = new List<Calisan>();
                viewModel.Hizmetler = await _dbContext.Hizmetler.ToListAsync();
                return View("Add", viewModel);
            }
            //return RedirectToAction("Add", viewModel);
          

            return View(returnUrl == "edit" ? "Edit" : "Add", viewModel);

        }

        [HttpGet("Randevu/List")]
        public async Task<IActionResult> List()
        {
            var randevular = await _dbContext.Randevular
                .Include(r => r.musteri)
                .Include(r => r.calisan)
                .Include(r => r.hizmet)
                .ToListAsync();

            return View(randevular);
        }

        [HttpGet("Randevu/Musteri_Randevu_List")]
        public async Task<IActionResult> Musteri_Randevu_List()
        {
            // Oturum açmış kullanıcıyı alıyoruz
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Kullanıcının müşteri bilgilerini alıyoruz
            var musteri = await _dbContext.Musteriler.FirstOrDefaultAsync(m => m.IdentityUserId == user.Id);
            if (musteri == null)
            {
                return NotFound("Müşteri bulunamadı.");
            }

            // Sadece bu müşteriye ait randevuları filtreliyoruz
            var randevular = await _dbContext.Randevular
                .Include(r => r.musteri)
                .Include(r => r.calisan)
                .Include(r => r.hizmet)
                .Where(r => r.MusteriId == musteri.MusteriID) // Filtreleme işlemi burada
                .ToListAsync();

            return View(randevular);
        }


        [Authorize(Roles = "Admin, Calisan")]
        [HttpPost("Randevu/Onayla/{randevuId}")]
        public async Task<IActionResult> Onayla(int randevuId)
        {
            var randevu = await _dbContext.Randevular.FindAsync(randevuId);
            if (randevu != null)
            {
                randevu.OnayDurumu = true;
                await _dbContext.SaveChangesAsync();
            }
            TempData["SuccessMessage"] = "Randevu başarıyla onaylandı";
            return RedirectToAction("List");
        }

        [HttpGet("Randevu/Edit/{randevuId}")]
        public async Task<IActionResult> Edit(int randevuId)
        {
            var randevu = await _dbContext.Randevular.FindAsync(randevuId);
            if (randevu == null)
            {
                return NotFound("Randevu bulunamadı.");
            }

            var model = new RandevuViewModel
            {
               RandevuId = randevuId,
                MusteriId = randevu.MusteriId,
                CalisanId = randevu.CalisanId,
                HizmetId = randevu.HizmetId,
                RandevuTarihi = randevu.RandevuTarihi,
                Dakika = randevu.Dakika,
                Fiyat = randevu.Ucret,
                Calisanlar = await _dbContext.Calisanlar.ToListAsync(),
                Hizmetler = await _dbContext.Hizmetler.ToListAsync()
            };

            ViewBag.RandevuId = randevuId;

            return View(model);
        }

        //[HttpPost("Randevu/Edit/{randevuId}")]
        [HttpPost]
        public async Task<IActionResult> Edit(int randevuId, [FromForm]RandevuViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var calisan = await _dbContext.Calisanlar.FindAsync(viewModel.CalisanId);
               
                
                var user = await _userManager.FindByIdAsync(calisan.IdentityUserId);
                
                var randevu = await _dbContext.Randevular.FindAsync(randevuId);
                if (randevu == null)
                {
                    return NotFound("Randevu bulunamadı.");
                }

                var calisanHizmetler = await _dbContext.CalisanHizmetler
                    .Where(ch => ch.CalisanId == viewModel.CalisanId)
                    .Select(ch => ch.HizmetId)
                    .ToListAsync();

                if (!calisanHizmetler.Contains(viewModel.HizmetId))
                {
                    ModelState.AddModelError("", "Çalışan bu hizmeti vermemektedir.");
                    viewModel.Calisanlar = await _dbContext.Calisanlar.ToListAsync();
                    viewModel.Hizmetler = await _dbContext.Hizmetler.ToListAsync();
                    return View(viewModel);
                }

                var mevcutRandevular = await _dbContext.Randevular
                    .Where(r => r.CalisanId == viewModel.CalisanId && r.RandevuTarihi.Date == viewModel.RandevuTarihi.Date)
                    .ToListAsync();

                foreach (var existingRandevu in mevcutRandevular)
                {
                    if (viewModel.RandevuTarihi < existingRandevu.RandevuTarihi.AddMinutes(existingRandevu.Dakika) && viewModel.RandevuTarihi.AddMinutes((int)viewModel.Dakika) > existingRandevu.RandevuTarihi)
                    {
                        ModelState.AddModelError("", "Bu saatte başka bir randevu var.");
                        viewModel.Calisanlar = await _dbContext.Calisanlar.ToListAsync();
                        viewModel.Hizmetler = await _dbContext.Hizmetler.ToListAsync();
                        return View(viewModel);
                    }
                }

                var hizmet = await _dbContext.Hizmetler.FindAsync(viewModel.HizmetId);

                randevu.MusteriId = viewModel.MusteriId;
                randevu.CalisanId = viewModel.CalisanId;
                randevu.HizmetId = viewModel.HizmetId;
                randevu.RandevuTarihi = viewModel.RandevuTarihi;
                randevu.Dakika = (int)hizmet.Dakika;
                randevu.Ucret = hizmet.Fiyat;
                randevu.OnayDurumu = false;

                await _dbContext.SaveChangesAsync();

                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    TempData["SuccessMessage"] = "Randevu başarıyla güncellendi.";
                    return RedirectToAction("List", "Randevu");
                }
                else if (await _userManager.IsInRoleAsync(user, "Calisan"))
                {
                    TempData["SuccessMessage"] = "Randevu başarıyla güncellendi.";
                    return RedirectToAction("List", "Randevu");
                }
                else if (await _userManager.IsInRoleAsync(user, "Musteri"))
                {
                    TempData["SuccessMessage"] = "Randevu başarıyla güncellendi.";
                    return RedirectToAction("Musteri_Randevu_List", "Randevu");
                }
            }

            viewModel.Calisanlar = await _dbContext.Calisanlar.ToListAsync();
            viewModel.Hizmetler = await _dbContext.Hizmetler.ToListAsync();
            return View(viewModel);
        }

      

        [Authorize(Roles = "Admin , Musteri")]
        [HttpGet("Randevu/Delete/{randevuId}")]
        public async Task<IActionResult> Delete(int randevuId)
        {
            var randevu = await _dbContext.Randevular.FindAsync(randevuId);
            if (randevu == null)
            {
                return NotFound("Randevu bulunamadı.");
            }

            _dbContext.Randevular.Remove(randevu);
            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Randevu başarıyla silindi.";
            return RedirectToAction("List");
        }

    }
}





































































//using barber_management_system.Data;
//using barber_management_system.Models;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace barber_management_system.Controllers
//{
//    public class RandevuController : Controller
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly UserManager<IdentityUser> _userManager;

//        public RandevuController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
//        {
//            _context = context;
//            _userManager = userManager;
//        }
//        [HttpGet]
//        public async Task<IActionResult> Add()
//        {
//            var calisanlar = await _context.Calisanlar.ToListAsync();
//            var hizmetler = await _context.Hizmetler.ToListAsync();

//            var viewModel = new RandevuCreateViewModel
//            {
//                Calisanlar = calisanlar,
//                Hizmetler = hizmetler,
//                TarihSaat = DateTime.Now
//            };

//            return View(viewModel);
//        }




//        [HttpPost]
//        public async Task<IActionResult> Add(RandevuCreateViewModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(model);
//            }

//            // Çalışan müsaitlik kontrolü
//            if (!await CalisanMusaitMi(model.CalisanId, model.TarihSaat, model.HizmetId))
//            {
//                ModelState.AddModelError("", "Seçilen tarih ve saatte çalışan müsait değil.");
//                return View(model);
//            }

//            var user = await _userManager.GetUserAsync(User);
//            var musteri = await _context.Musteriler
//                .FirstOrDefaultAsync(m => m.IdentityUserId == user.Id);

//            var randevu = new Randevu
//            {
//                MusteriId = musteri.MusteriID,
//                CalisanId = model.CalisanId,
//                HizmetId = model.HizmetId,
//                RandevuTarihi = model.TarihSaat.Date,
//                RandevuSaati = model.TarihSaat.TimeOfDay,
//                Ucret = await _context.Hizmetler
//                    .Where(h => h.HizmetID == model.HizmetId)
//                    .Select(h => h.Fiyat)
//                    .FirstOrDefaultAsync(),
//                Durumu = RandevuDurumu.Beklemede
//            };

//            _context.Randevular.Add(randevu);
//            await _context.SaveChangesAsync();

//            return RedirectToAction(nameof(Index));
//        }
//    }
//}
