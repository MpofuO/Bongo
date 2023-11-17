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
        private static Session[,] mergedSessions;
        public MergerController(IRepositoryWrapper _repository, UserManager<BongoUser> _userManager)
        {
            repository = _repository;
            userManager = _userManager;
        }
        public IActionResult ChooseSemester()
        {
            return View();
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
            var usersKeyValuePairs = (IEnumerable<KeyValuePair<string, string>>)userManager.Users
                .Select(user => new KeyValuePair<string, string>(user.UserName, user.MergeKey));

            var users = new Dictionary<string, string>(usersKeyValuePairs);
            users.Remove(User.Identity.Name);

            return View(new MergerIndexViewModel
            {
                Sessions = mergedSessions,
                MergedUsers = mergedUsers,
                Users = users
            });
        }
        public IActionResult AddUserTimetable(string username)
        {
            if (mergedUsers.Contains(username))
            {
                TempData["Message"] = $"{username}'s timetable has already been merged with.";
            }
            else
            {
                var timetable = repository.Timetable.GetUserTimetable(username);
                if (timetable != null)
                {
                    if (timetable.TimetableText == "")
                    {
                        TempData["Message"] = $"Please note {username}'s timetable is empty. It has no classes, practicals or tutorials.";
                        goto AddUser;
                    }

                    processor = new TimetableProcessor(timetable.TimetableText, _isForFirstSemester);
                    Session[,] newUserSessions = processor.GetSessionsArray(out clashes, out groups);

                    if (clashes.Count > 0 || groups.Count > 0)
                    {
                        if (username == User.Identity.Name)
                        {
                            TempData["Message"] = "Please ensure that you have managed your clashes and/groups before merging with others.";

                            CookieOptions cookieOptions = new CookieOptions { Expires = DateTime.Now.AddDays(90) };
                            Response.Cookies.Append("isForFirstSemester", _isForFirstSemester.ToString(), cookieOptions);
                            return RedirectToAction("Manage", "Session");
                        }
                        else
                        {
                            TempData["Message"] = $"Could not merge with {username}'s timetable.\n" +
                                $"Please ensure that {username} has managed their clashes and/or groups before merging with them.";
                            return RedirectToAction("Index");
                        }
                    }
                    if (mergedUsers.Count == 0)
                        mergedSessions = newUserSessions.SplitSessions();
                    else
                        MergerControlHelpers.Merge(mergedSessions, newUserSessions);

                    AddUser:
                    mergedUsers.Add(username);
                    return RedirectToAction("Index");
                }

                TempData["Message"] = $"Could not merge with {username}'s timetable.\n" +
                                $"Please ensure that {username} has created their timetable before merging with them.";
            }

            return RedirectToAction("Index");
        }
        public IActionResult RemoveUserTimetable(string username)
        {
            if (mergedUsers.Contains(username))
            {
                var timetable = repository.Timetable.GetUserTimetable(username);
                if (timetable.TimetableText != "")
                {
                    processor = new TimetableProcessor(timetable.TimetableText, _isForFirstSemester);
                    MergerControlHelpers.UnMerge(mergedSessions, processor.GetSessionsArray(out clashes, out groups));
                }
                mergedUsers.Remove(username);
            }
            return RedirectToAction("Index");
        }
    }
}
