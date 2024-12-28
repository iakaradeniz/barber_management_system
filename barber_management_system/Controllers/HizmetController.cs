using barber_management_system.Data;
using barber_management_system.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace barber_management_system.Controllers
{
    
    public class HizmetController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        public HizmetController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult>Add(AddHizmetViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var hizmet = new Hizmet
                {
                    HizmetAd = viewModel.Ad,
                    Fiyat = viewModel.Fiyat,
                    Dakika = viewModel.Dakika
                };
                await dbContext.Hizmetler.AddAsync(hizmet);
                await dbContext.SaveChangesAsync();
                return RedirectToAction("List", "Hizmet");

            }
            else
            {
                return View(viewModel);
            }



        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var hizmet = await dbContext.Hizmetler.ToListAsync();
            return View(hizmet);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int Id)
        {
            var hizmet = await dbContext.Hizmetler.FindAsync(Id);
            return View(hizmet);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Hizmet viewModel)
        {
            var hizmet = await dbContext.Hizmetler.FindAsync(viewModel.HizmetID);

            if (hizmet is not null)
            {
                hizmet.HizmetAd = viewModel.HizmetAd;
                hizmet.Fiyat = viewModel.Fiyat;
                hizmet.Dakika = viewModel.Dakika;


                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("List", "Hizmet");
        }

        [HttpGet("/Hizmet/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Veritabanında silinmek istenen hizmeti buluyoruz
            var hizmet = await dbContext.Hizmetler
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.HizmetID == id);

            // Eğer hizmet varsa silme işlemini gerçekleştiriyoruz
            if (hizmet is not null)
            {
                dbContext.Hizmetler.Remove(hizmet);
                await dbContext.SaveChangesAsync();
            }

            // Silme işleminden sonra listeye yönlendiriyoruz
            return RedirectToAction("List", "Hizmet");
        }


       

    }
}
