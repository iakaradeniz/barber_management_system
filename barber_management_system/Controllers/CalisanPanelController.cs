using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace barber_management_system.Controllers
{
    [Authorize(Roles = "Calisan")]
    public class CalisanPanelController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}