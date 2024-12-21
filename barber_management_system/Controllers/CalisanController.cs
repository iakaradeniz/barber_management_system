using barber_management_system.Data;
using barber_management_system.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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

            // Çalışan için bir Identity kullanıcı hesabı oluşturma
            var user = new IdentityUser { UserName = model.CalisanAd, Email = $"{model.CalisanAd}@example.com" };
            var result = await _userManager.CreateAsync(user, "DefaultPassword123!");

            if (result.Succeeded)
            {
                // Çalışan rolünü kullanıcıya atama
                await _userManager.AddToRoleAsync(user, "Calisan");
            }


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
            var calisan = await _dbContext.Calisanlar.FindAsync(viewModel.CalisanID);
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
                    // Eski kullanıcı adını bul
                    var oldUserName = calisan.CalisanAd;

                    calisan.CalisanAd = viewModel.CalisanAd;
                    calisan.CalisanSoyad = viewModel.CalisanSoyad;
                    // Değişiklikleri kaydet
                    await _dbContext.SaveChangesAsync();

                    // Identity kullanıcısını güncelle
                    var user = await _userManager.FindByNameAsync(oldUserName);
                    if (user != null)
                    {
                        user.UserName = viewModel.CalisanAd;
                        user.Email = $"{viewModel.CalisanAd}@example.com";
                        await _userManager.UpdateAsync(user);
                    }
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

                // Identity kullanıcı hesabını da sil
                var user = await _userManager.FindByNameAsync(calisan.CalisanAd);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }
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



























