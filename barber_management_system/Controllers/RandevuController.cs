using Microsoft.AspNetCore.Mvc;

namespace barber_management_system.Controllers
{
    public class RandevuController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
