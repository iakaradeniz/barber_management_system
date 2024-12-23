using barber_management_system.Data;
using barber_management_system.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace barber_management_system.Controllers
{
    [Authorize(Roles = "Admin, Calisan, Musteri")]
    public class MüsteriController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public MüsteriController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }




        [HttpGet]
        [Authorize(Roles = "Admin, Musteri")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Musteri")]
        public async Task<IActionResult> Add(MusteriViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(viewModel.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Bu e-posta adresi zaten kullanılıyor.");
                    return View(viewModel);
                }

                var user = new IdentityUser { UserName = viewModel.Email, Email = viewModel.Email };
                var result = await _userManager.CreateAsync(user, viewModel.Password);

                if (result.Succeeded)
                {
                    var musteri = new Musteri()
                    {
                        MusteriAd = viewModel.Ad,
                        MusteriSoyAd = viewModel.SoyAd,
                        Email = viewModel.Email,
                        Sifre = viewModel.Password,
                        IdentityUserId = user.Id,
                        
                    };
                    await _dbContext.Musteriler.AddAsync(musteri);
                    await _dbContext.SaveChangesAsync();

                    await _userManager.AddToRoleAsync(user, "Musteri");

                    return RedirectToAction("List", "Müsteri");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(viewModel);
           

        }

        [HttpGet]
        [Authorize(Roles = "Admin, Calisan, Musteri")]

        public async Task<IActionResult> List()
        {
            var musteri = await _dbContext.Musteriler.ToListAsync();
            return View(musteri);
        }

        [Authorize(Roles = "Admin, Calisan")]
        [HttpGet("Müsteri/Edit/{musteriId}")]
        public async Task<IActionResult> Edit(int musteriId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var musteri = await _dbContext.Musteriler.FirstOrDefaultAsync(m => m.MusteriID == musteriId);
            if (musteri == null)
            {
                return NotFound();
            }
            if (User.IsInRole("Musteri") && musteri.IdentityUserId != userId)
            {
                return Forbid();
            }

            // Identity kullanıcı bilgilerini getir
            var user = await _userManager.FindByIdAsync(musteri.IdentityUserId);
            if (user == null)
            {
                return NotFound("İlgili kullanıcı bulunamadı.");
            }

            var viewModel = new MusteriViewModel
            {
                MusteriId = musteri.MusteriID,
                Ad = musteri.MusteriAd,
                SoyAd = musteri.MusteriSoyAd,
                Email = musteri.Email,
                Password = musteri.Sifre
            };
            return View(viewModel);
        }

        [Authorize(Roles = "Admin, Calisan")]
        [HttpPost("Müsteri/Edit/{musteriId}")]
        public async Task<IActionResult> Edit(MusteriViewModel viewModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var musteri = await _dbContext.Musteriler.FirstOrDefaultAsync(m => m.MusteriID == viewModel.MusteriId);
                if (musteri != null)
                {
                    if (User.IsInRole("Musteri") && musteri.IdentityUserId != userId)
                    {
                        return Forbid();
                    }
                    // Email adresinin başka bir kullanıcı tarafından kullanılıp kullanılmadığını kontrol et
                    var existingUserWithEmail = await _userManager.FindByEmailAsync(viewModel.Email);
                    if (existingUserWithEmail != null && existingUserWithEmail.Id != musteri.IdentityUserId)
                    {
                        ModelState.AddModelError("Email", "Bu email adresi zaten kullanımda.");
                        return View(viewModel);
                    }

                    // Identity kullanıcı bilgilerini güncelle
                    var user = await _userManager.FindByIdAsync(musteri.IdentityUserId);
                    if (user != null)
                    {
                        // E-posta adresini güncelle
                        user.Email = viewModel.Email;
                        user.UserName = viewModel.Email;

                        var updateResult = await _userManager.UpdateAsync(user);
                        if (!updateResult.Succeeded)
                        {
                            foreach (var error in updateResult.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                            return View(viewModel); // Hataları göster ve formu tekrar yükle
                        }

                        // Şifre değişikliği yapılmışsa şifreyi güncelle
                        if (!string.IsNullOrEmpty(viewModel.Password))
                        {
                            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                            var passwordResult = await _userManager.ResetPasswordAsync(user, token, viewModel.Password);

                            if (!passwordResult.Succeeded)
                            {
                                foreach (var error in passwordResult.Errors)
                                {
                                    ModelState.AddModelError("", error.Description);
                                }
                                return View(viewModel); // Hataları göster ve formu tekrar yükle
                            }
                        }
                    }

                    //Müsteri Bilgilerini güncelle
                    musteri.MusteriAd = viewModel.Ad;
                    musteri.MusteriSoyAd = viewModel.SoyAd;
                    musteri.Email = viewModel.Email;
                    musteri.Sifre = viewModel.Password;

                    // Değişiklikleri kaydet ve transaction'ı tamamla
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return RedirectToAction("List", "Müsteri");
                }
                else
                {
                    ModelState.AddModelError("", "Müşteri bulunamadı.");
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda transaction'ı geri al
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Bir hata oluştu. Lütfen tekrar deneyiniz.");
                return View(viewModel);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Müsteri/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Veritabanından müşteriyi bul
            var musteri = await _dbContext.Musteriler
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MusteriID == id);
            if (musteri != null)
            {
                var userId = musteri.IdentityUserId;
                _dbContext.Musteriler.Remove(musteri);
                await _dbContext.SaveChangesAsync();

                // Identity kullanıcı hesabını da sil
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }
                return RedirectToAction("List", "Müsteri");
            }
            else
            {
                TempData["ErrorMessage"] = "Müşteri mevcut değil.";
                return RedirectToAction("List", "Müsteri");
            }
        }
    }
}


