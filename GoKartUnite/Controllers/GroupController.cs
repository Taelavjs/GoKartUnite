using GoKartUnite.Interfaces;
using GoKartUnite.Models;
using GoKartUnite.Models.Groups;
using GoKartUnite.Projection;
using GoKartUnite.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Web.Helpers;

namespace GoKartUnite.Controllers
{
    public class GroupController : Controller
    {
        private readonly IGroupHandler _groups;
        private readonly IKarterHandler _karters;
        private readonly IBlogHandler _blog;

        public GroupController(IKarterHandler karters, IGroupHandler groups, IBlogHandler blog)
        {
            _groups = groups;
            _karters = karters;
            _blog = blog;
        }

        public async Task<ActionResult> Index()
        {
            Karter? k = await _karters.GetUserByGoogleId(User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value, withTrack: true);

            List<ListedGroupView> groups = await _groups.GetAllGroups(k);

            var groupPage = new GroupPageView
            {
                Groups = groups,
                PostageGroup = new ListedGroupView()
            };
            return View(groupPage);
        }

        [HttpPost]
        public async Task<ActionResult> CreateGroup(GroupPageView model)
        {
            ListedGroupView listedGroup = new ListedGroupView();
            if (ModelState.IsValid)
            {
                listedGroup = model.PostageGroup;  // This refers to ListedGroupView within GroupPageView
            }
            Karter k = await _karters.GetUserByGoogleId(User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value, withTrack: true);
            await _groups.CreateNewGroup(listedGroup, k);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JoinGroup(int GroupId)
        {
            Karter? k = await _karters.GetUserByGoogleId(User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value, withTrack: true);
            if (k == null) return RedirectToAction("Index");


            await _groups.JoinGroup(GroupId, k);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LeaveGroup(int GroupId)
        {
            Karter? k = await _karters.GetUserByGoogleId(User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value, withTrack: true);
            if (k == null) return RedirectToAction("Index");

            await _groups.LeaveGroup(GroupId, k);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Home(int GroupId)
        {
            Group? g = await _groups.GetGroupById(GroupId);
            if (g == null) return NotFound();

            GroupHomeView returnObj = new GroupHomeView
            {
                Posts = await _blog.GetModelToView(await _blog.GetAllPosts(new DataFilterOptions.BlogFilterOptions())),
                Group = await _groups.ToDTO(g),
                Members = await _groups.GetAllMembersProjection(GroupId),
            };
            return View(returnObj);
        }

        public async Task<JsonResult> GroupStats(int GroupId)
        {
            List<GroupStatDisplay> statsToClient = await _groups.GetStatsForGroupGraph(GroupId, "newbuckmore");





            return Json(statsToClient);
        }
    }
}