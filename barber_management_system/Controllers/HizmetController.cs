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
            var hizmet = new Hizmet()
            {
                HizmetAd = viewModel.Ad,
                Fiyat = viewModel.Fiyat,
               
            };
            await dbContext.Hizmetler.AddAsync(hizmet);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("List", "Hizmet");
            
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
              

                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("List", "Hizmet");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Hizmet viewModel)
        {
            var student = await dbContext.Hizmetler
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.HizmetID == viewModel.HizmetID);
            if (student is not null)
            {
                dbContext.Hizmetler.Remove(viewModel);
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("List", "Hizmet");

        }
    }
}
