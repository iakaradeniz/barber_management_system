using barber_management_system.Data;
using barber_management_system.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace barber_management_system.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CalisanController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public CalisanController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        //Çalışıyor
        [HttpGet]
        public IActionResult Add()
        {
            var viewModel = new CalisanViewModel
            {
                SelectedHizmetler = new List<int>(),
                SelectedUzmanlık = new List<int>(),
                Hizmetler = _dbContext.Hizmetler.ToList(),
                Uzmanlıklar = _dbContext.Hizmetler.ToList()
            };

            if (!viewModel.Hizmetler.Any())
            {
                ViewBag.Message = "Sistemde hizmet bulunmamaktadır.";
            }

            return View(viewModel);
        }

       
        //Çalışıyor
        [HttpPost]
        public async Task<IActionResult> Add(CalisanViewModel model)
        {
            model.Hizmetler = _dbContext.Hizmetler.ToList();
           

            if (!ModelState.IsValid)
            {
                return View(model); // Hatalı form gönderimi varsa tekrar formu göster
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "Bu e-posta adresi zaten kullanılıyor.");
                return View(model);
            }

            var user = new IdentityUser { UserName = model.CalisanAd, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var calisan = new Calisan
                {
                    CalisanAd = model.CalisanAd,
                    CalisanSoyad = model.CalisanSoyad,
                    Email = model.Email,
                    Sifre = model.Password,
                    IdentityUserId = user.Id
                };


                foreach (var hizmetId in model.SelectedHizmetler)
                {
                    var hizmet = await _dbContext.Hizmetler.FindAsync(hizmetId);
                    if (hizmet != null)
                    {
                        calisan.calisanhizmetlist.Add(new CalisanHizmet { Calisan = calisan, Hizmet = hizmet });
                    }
                }

                foreach (var hizmetId in model.SelectedUzmanlık)
                {
                    var hizmet = await _dbContext.Hizmetler.FindAsync(hizmetId);
                    if (hizmet != null)
                    {
                        calisan.calisanuzmanliklist.Add(new CalisanUzmanlik { Calisan = calisan, Hizmet = hizmet });
                    }
                }

                await _dbContext.Calisanlar.AddAsync(calisan);
                await _dbContext.SaveChangesAsync();

                await _userManager.AddToRoleAsync(user, "Calisan");

                return RedirectToAction("List", "Calisan");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }
        //Çalışıyor
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var calisanlar = await _dbContext.Calisanlar
                .Include(c => c.calisanhizmetlist)
                .ThenInclude(ch => ch.Hizmet)
                .Include(calisanlar => calisanlar.calisanuzmanliklist)
                .ThenInclude(calisanuzmanliklist => calisanuzmanliklist.Hizmet)
                .ToListAsync();
            return View(calisanlar);
        }


        //Çalışmıyor
        [HttpGet("Calisan/Edit/{calisanId}")]
        public async Task<IActionResult> Edit( int calisanId)
        {
            // Veritabanından çalışan bilgilerini ve ilişkili hizmetleri getir
            var calisan = await _dbContext.Calisanlar
                 .Include(c => c.calisanhizmetlist)
                 .ThenInclude(ch => ch.Hizmet)
                 .Include(c => c.calisanuzmanliklist)
                 .ThenInclude(ch => ch.Hizmet)
                 .FirstOrDefaultAsync(c => c.CalisanID == calisanId);

            if (calisan == null)
            {
                return NotFound("Çalışan bulunamadı.");
            }

            // Identity kullanıcı bilgilerini getir
            var user = await _userManager.FindByIdAsync(calisan.IdentityUserId);
            if (user == null)
            {
                return NotFound("İlgili kullanıcı bulunamadı.");
            }

            // Çalışan bilgilerini ViewModel'e aktar
            var viewModel = new CalisanViewModel
            {
                CalisanId = calisan.CalisanID,
                CalisanAd = calisan.CalisanAd,
                CalisanSoyad = calisan.CalisanSoyad,
                Email = user.Email,
                Hizmetler = calisan.calisanhizmetlist.Select(ch => ch.Hizmet).ToList(),
                Uzmanlıklar = calisan.calisanuzmanliklist.Select(ch => ch.Hizmet).ToList(),
                SelectedHizmetler = calisan.calisanhizmetlist.Select(ch => ch.HizmetId).ToList(),
                SelectedUzmanlık = calisan.calisanuzmanliklist.Select(ch => ch.HizmetId).ToList()   
            };

            // Mevcut hizmetleri getir ve ViewBag'e ekle
            var availableHizmetler = await _dbContext.Hizmetler.ToListAsync();
            ViewBag.AvailableHizmetler = availableHizmetler.Any() ? availableHizmetler : new List<Hizmet>();

            if (!availableHizmetler.Any())
            {
                ViewBag.Message = "Sistemde hizmet bulunmamaktadır.";
            }

            return View(viewModel);
        }

        //Çalışmıyor
        [HttpPost("Calisan/Edit/{calisanId}")]
        public async Task<IActionResult> Edit(CalisanViewModel model)
        {
            // Hizmet listesini doldurmak için yardımcı metod
            async Task HizmetleriDoldur()
            {
                var mevcutHizmetler = await _dbContext.Hizmetler.ToListAsync();
                ViewBag.AvailableHizmetler = mevcutHizmetler.Any()
                    ? mevcutHizmetler
                    : new List<Hizmet>();
            }

            // Model doğrulaması kontrolü
            if (!ModelState.IsValid)
            {
                await HizmetleriDoldur();
                return View(model);
            }

            // Veritabanı işlemleri için transaction başlat
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Çalışanı veritabanından bul
                var calisan = await _dbContext.Calisanlar
                    .Include(c => c.calisanhizmetlist)
                    .Include(c => c.calisanuzmanliklist)
                    .FirstOrDefaultAsync(c => c.CalisanID == model.CalisanId);

                if (calisan == null)
                {
                    ModelState.AddModelError("", "Çalışan bulunamadı.");
                    await HizmetleriDoldur();
                    return View(model);
                }

                // Email adresinin başka bir kullanıcı tarafından kullanılıp kullanılmadığını kontrol et
                var existingUserWithEmail = await _userManager.FindByEmailAsync(model.Email);
                if (existingUserWithEmail != null && existingUserWithEmail.Id != calisan.IdentityUserId)
                {
                    ModelState.AddModelError("Email", "Bu email adresi zaten kullanımda.");
                    await HizmetleriDoldur();
                    return View(model);
                }

                // Identity kullanıcı bilgilerini güncelle
                var user = await _userManager.FindByIdAsync(calisan.IdentityUserId);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.UserName = model.Email;

                    var updateResult = await _userManager.UpdateAsync(user);

                    if (!updateResult.Succeeded)
                    {
                        foreach (var error in updateResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        await HizmetleriDoldur();
                        return View(model);
                    }

                    // Eğer yeni şifre girilmişse şifreyi güncelle
                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var passwordResult = await _userManager.ResetPasswordAsync(user, token, model.Password);

                        if (!passwordResult.Succeeded)
                        {
                            foreach (var error in passwordResult.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                            await HizmetleriDoldur();
                            return View(model);
                        }
                    }
                }

                // Çalışan bilgilerini güncelle
                calisan.CalisanAd = model.CalisanAd;
                calisan.CalisanSoyad = model.CalisanSoyad;
                calisan.Email = model.Email;
                calisan.Sifre = model.Password;

                // Çalışanın hizmet listesini güncelle
                calisan.calisanhizmetlist.Clear();
                if (model.SelectedHizmetler != null)
                {
                    foreach (var hizmetId in model.SelectedHizmetler)
                    {
                        calisan.calisanhizmetlist.Add(new CalisanHizmet
                        {
                            HizmetId = hizmetId
                        });
                    }
                }

                // Çalışanın uzmanlık listesini güncelle
                calisan.calisanuzmanliklist.Clear();
                if (model.SelectedUzmanlık != null)
                {
                    foreach (var hizmetId in model.SelectedUzmanlık)
                    {
                        calisan.calisanuzmanliklist.Add(new CalisanUzmanlik
                        {
                            HizmetId = hizmetId
                        });
                    }
                }

                // Değişiklikleri kaydet ve transaction'ı tamamla
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction(nameof(List));
            }
            catch (Exception ex)
            {
                // Hata durumunda transaction'ı geri al
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Bir hata oluştu. Lütfen tekrar deneyiniz.");
                await HizmetleriDoldur();
                return View(model);
            }
        }

       



        //Çalışıyor
        [HttpGet("Calisan/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Veritabanından çalışanı bul
            var calisan = await _dbContext.Calisanlar
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CalisanID == id);
           

            if (calisan != null)
            {
                var userId = calisan.IdentityUserId;
                // Çalışanı veritabanından sil
                _dbContext.Calisanlar.Remove(calisan);
                await _dbContext.SaveChangesAsync();

                // Identity kullanıcı hesabını da sil
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }
                return RedirectToAction("List", "Calisan");
            }
            else
            {
                TempData["ErrorMessage"] = "Çalışan mevcut değil.";
                return RedirectToAction("List", "Calisan");
            }

            
        }



        

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCalisanById(int id)
        {
            var calisan = await _dbContext.Calisanlar
                .Include(c => c.CalismaSaatleri) // Çalışma saatlerini dahil et
                .Include(c => c.calisanhizmetlist).ThenInclude(ch => ch.Hizmet)
                .Include(c => c.calisanuzmanliklist).ThenInclude(ch => ch.Hizmet)
                .FirstOrDefaultAsync(c => c.CalisanID == id);

            if (calisan == null) return NotFound("Çalışan bulunamadı.");

            return Ok(calisan);
        }

        // Hizmet ekleme ve çıkarma işlemleri
        [HttpPost]
        public async Task<IActionResult> AddHizmetToCalisan(int calisanId, int hizmetId)
        {
            var calisan = await _dbContext.Calisanlar.Include(c => c.calisanhizmetlist).FirstOrDefaultAsync(c => c.CalisanID == calisanId);
            if (calisan == null) return NotFound("Çalışan bulunamadı.");

            var hizmet = await _dbContext.Hizmetler.FindAsync(hizmetId);
            if (hizmet == null) return NotFound("Hizmet bulunamadı.");

            if (!calisan.calisanhizmetlist.Any(ch => ch.HizmetId == hizmetId))
            {
                calisan.calisanhizmetlist.Add(new CalisanHizmet { CalisanId = calisanId, HizmetId = hizmetId });
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Edit", new { id = calisanId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveHizmetFromCalisan(int calisanId, int hizmetId)
        {
            var calisan = await _dbContext.Calisanlar.Include(c => c.calisanhizmetlist).FirstOrDefaultAsync(c => c.CalisanID == calisanId);
            if (calisan == null) return NotFound("Çalışan bulunamadı.");

            var calisanHizmet = calisan.calisanhizmetlist.FirstOrDefault(ch => ch.HizmetId == hizmetId);
            if (calisanHizmet != null)
            {
                calisan.calisanhizmetlist.Remove(calisanHizmet);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Edit", new { id = calisanId });
        }    
    }
}