using Bongo.Areas.TimetableArea.Infrastructure;
using Bongo.Areas.TimetableArea.Models;
using Bongo.Areas.TimetableArea.Models.ViewModels;
using Bongo.Data;
using Bongo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bongo.Areas.TimetableArea.Controllers
{
    [Authorize]
    [Area("TimetableArea")]
    public class MergerController : Controller
    {
        private static IRepositoryWrapper repository;
        private static UserManager<BongoUser> userManager;
        private static TimetableProcessor processor;
        private static bool _isForFirstSemester;
        private static List<List<Session>> clashes;
        private static List<Lecture> groups;
        private static List<string> mergedUsers;
        private static string mergedText; 
        public MergerController(IRepositoryWrapper _repository, UserManager<BongoUser> _userManager)
        {
            repository = _repository;
            userManager = _userManager;
        }
        public IActionResult SetSemester(bool isForFirstSemester)
        {
            _isForFirstSemester = isForFirstSemester;
            mergedUsers = new List<string>();
            return RedirectToAction("ReviewCurrentUser");
        }
        public IActionResult ReviewCurrentUser()
        {
            var timetabe = repository.Timetable.GetUserTimetable(User.Identity.Name);
            if (timetabe != null)
                return RedirectToAction("AddUserTimetable", new { username = User.Identity.Name });

            TempData["Message"] = "Please create your timetable before you can merge with others.";
            return RedirectToAction("Upload", "Timetable");
        }
        public IActionResult Index()
        {
            processor = new TimetableProcessor(mergedText, _isForFirstSemester);
            Session[,] Sessions = processor.GetSessionsArray(out clashes, out groups);
            Sessions.SplitSessions();

            if (clashes.Count > 0)
                MergerControlHelpers.HandleClashes(Sessions, clashes);
            if (groups.Count > 0)
                MergerControlHelpers.HandleGroups(Sessions, groups);

            return View(new MergerIndexViewModel
            {
                Sessions = Sessions,
                MergedUsers = mergedUsers,
                Users = userManager.Users.Select(u => u.UserName).ToList()
            });
        }

        public IActionResult AddUserTimetable(string username)
        {
            var timetable = repository.Timetable.GetUserTimetable(username);
            if (timetable != null)
            {
                processor = new TimetableProcessor(timetable.TimetableText, _isForFirstSemester);
                processor.GetSessionsArray(out clashes, out groups);

                if(clashes.Count>0 || groups.Count > 0)
                {
                    if (username == User.Identity.Name)
                    {
                        TempData["Message"] = "Please ensure that you have managed your clashes and/groups before merging with others.";
                        return RedirectToAction("Upload", "Timetable");
                    }
                    else
                    {
                        TempData["Message"] = $"Could not merge with {username}'s timetable.\n" +
                            $"Please ensure that {username} has managed their clashes and/groups before merging with them.";
                        return RedirectToAction("Upload", "Timetable");
                    }
                }

                mergedUsers.Add(username);
                mergedText += timetable.TimetableText;
                return RedirectToAction("Index");
            }

            TempData["Message"] = $"Could not merge with {username}'s timetable.\n" +
                            $"Please ensure that {username} has created their timetable before merging with them."; 
            return RedirectToAction("Index");
        }
        public IActionResult RemoveUserTimetable(string username)
        {
            var timetable = repository.Timetable.GetUserTimetable(username);
            mergedText = mergedText.Replace(timetable.TimetableText, "");
            mergedUsers.Remove(username);
            return View("Index");
        }
    }
}
