using Microsoft.AspNetCore.Mvc;

namespace Bongo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
