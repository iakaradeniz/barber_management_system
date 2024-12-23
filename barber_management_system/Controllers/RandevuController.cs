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
