using barber_management_system.Data;
using barber_management_system.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace barber_management_system.Controllers
{
    public class MüsteriController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public IActionResult Index()
        {
            return View();
        }
        public MüsteriController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddMüsteriViewModel viewModel)
        {
            var musteri = new Musteri()
            {
                MusteriAd = viewModel.Ad,
                MusteriSoyAd = viewModel.SoyAd,

            };
            await dbContext.Musteriler.AddAsync(musteri);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("List", "Müsteri");

        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var musteri = await dbContext.Musteriler.ToListAsync();
            return View(musteri);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            var musteri = await dbContext.Musteriler.FindAsync(Id);
            return View(musteri);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Musteri viewModel)
        {
            var musteri = await dbContext.Musteriler.FindAsync(viewModel.MusteriID);

            if (musteri is not null)
            {
                musteri.MusteriAd = viewModel.MusteriAd;
                musteri.MusteriSoyAd = viewModel.MusteriSoyAd;


                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("List", "Müsteri");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Musteri viewModel)
        {
            var student = await dbContext.Musteriler
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MusteriID == viewModel.MusteriID);
            if (student is not null)
            {
                dbContext.Musteriler.Remove(viewModel);
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("List", "Müsteri");

        }

    }
}
