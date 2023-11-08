using Bongo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bongo.Areas.TimetableArea.Controllers
{
    [AllowAnonymous]
    [Area("TimetableArea")]
    public class HowItWorksController : Controller
    {
        private readonly IRepositoryWrapper _repo;

        public HowItWorksController(IRepositoryWrapper repo)
        {
            _repo = repo; 
        }
        public IActionResult CreateTimetable()
        {
            return View(_repo.UserReview.FindAll().ToList());
        }
        public IActionResult MergeTimetables()
        {
            return View();
        }
    }
}