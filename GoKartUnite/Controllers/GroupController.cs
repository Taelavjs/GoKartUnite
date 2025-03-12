using GoKartUnite.Interfaces;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GoKartUnite.Controllers
{
    public class GroupController : Controller
    {
        private readonly IGroupHandler _groups;
        private readonly IKarterHandler _karters;

        public GroupController(IKarterHandler karters, IGroupHandler groups)
        {
            _groups = groups;
            _karters = karters;
        }


        public async Task<ActionResult> Index()
        {
            List<GroupView> groups = await _groups.GetAllGroups();

            var groupPage = new GroupPageView
            {
                Groups = groups,
            };
            return View(groupPage);
        }

        [HttpPost]
        public async Task<ActionResult> CreateGroup(GroupView group)
        {
            if (!ModelState.IsValid)
            {
                return View("Index");
            }
            Karter k = await _karters.GetUserByGoogleId(User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value, withTrack: true);
            await _groups.CreateNewGroup(group, k);


            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult JoinGroup(int GroupId)
        {
            return Json(new { success = true, newMemberCount = 6 });
        }

    }
}
