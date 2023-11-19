using Bongo.Data;
using Bongo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bongo.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<BongoUser> _userManager;
        private readonly IRepositoryWrapper _repo;

        public HomeController(IRepositoryWrapper repo, UserManager<BongoUser> userManager)
        {
            _repo = repo;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            TempData["HasTimetable"] = !User.Identity.IsAuthenticated ? false :
                _repo.Timetable.GetUserTimetable(User.Identity.Name) != null;

            return View();
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Notice()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            user.Notified = true;
            await _userManager.UpdateAsync(user);
            Response.Cookies.Append("Notified", user.Notified.ToString().ToLower(),
                            new CookieOptions { Expires = DateTime.Now.AddDays(90) }
                            );
            return RedirectToAction("Index");
        }
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            return View(user);
        }
    }
}
