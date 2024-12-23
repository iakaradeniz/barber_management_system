using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using barber_management_system.Models;
using barber_management_system.Data;

namespace barber_management_system.Controllers
{
    //Burası 
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Kullanıcıyı varsayılan olarak "Musteri" rolüne atıyoruz
                    await _userManager.AddToRoleAsync(user, "Musteri");

                    // Müşteri bilgilerini kaydediyoruz
                    var musteri = new Musteri
                    {
                        MusteriAd = model.Ad,
                        MusteriSoyAd = model.Soyad,
                        IdentityUserId = user.Id,
                        Email = model.Email,
                        Sifre = model.Password
                    };

                    _context.Musteriler.Add(musteri);
                    await _context.SaveChangesAsync();

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Kullanıcının rolünü kontrol et
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("Index", "Home");
                        //// Admin için admin paneline yönlendir
                        //return RedirectToAction("Index", "AdminPanel");
                    }
                    else if (roles.Contains("Calisan"))
                    {
                        return RedirectToAction("Index", "Home");
                        //// Çalışan için çalışan paneline yönlendir
                        //return RedirectToAction("Index", "CalisanPanel");
                    }
                    else if (roles.Contains("Musteri"))
                    {
                        return RedirectToAction("Index", "Home");
                        //// Müşteri için ana sayfaya yönlendir
                        //return RedirectToAction("Index", "Home");
                    }
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
