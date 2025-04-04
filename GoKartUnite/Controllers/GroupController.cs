using Azure.Core;
using GoKartUnite.CustomAttributes;
using GoKartUnite.Interfaces;
using GoKartUnite.Models;
using GoKartUnite.Models.Groups;
using GoKartUnite.Projection;
using GoKartUnite.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Security.Claims;
using System.Web.Helpers;
using System.Web.Mvc;
using ActionResult = Microsoft.AspNetCore.Mvc.ActionResult;
using Controller = Microsoft.AspNetCore.Mvc.Controller;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using JsonResult = Microsoft.AspNetCore.Mvc.JsonResult;
using ValidateAntiForgeryTokenAttribute = Microsoft.AspNetCore.Mvc.ValidateAntiForgeryTokenAttribute;

namespace GoKartUnite.Controllers
{
    public class GroupController : Controller
    {
        private readonly IGroupHandler _groups;
        private readonly IKarterHandler _karter;
        private readonly IBlogHandler _blog;
        private readonly ITrackHandler _track;

        public GroupController(ITrackHandler track, IKarterHandler karters, IGroupHandler groups, IBlogHandler blog)
        {
            _groups = groups;
            _karter = karters;
            _blog = blog;
            _track = track;
        }
        [HttpGet]
        [Authorize]
        [AccountConfirmed]
        public async Task<ActionResult> Index()
        {
            Karter? k = await _karter.GetUserByGoogleId(
                await _karter.GetCurrentUserNameIdentifier(User),
                withTrack: true
            );

            List<ListedGroupView> groups = await _groups.GetAllGroups(k, null, "");

            var groupPage = new GroupPageView
            {
                Groups = groups,
                PostageGroup = new ListedGroupView()
            };
            return View(groupPage);
        }

        public async Task<IActionResult> SortedListOfGroups(string SortedBy, string query)
        {
            Karter? k = await _karter.GetUserByGoogleId(
                await _karter.GetCurrentUserNameIdentifier(User),
                withTrack: true
            );
            if (Enum.TryParse(SortedBy, true, out Filters filter))
            {
                List<ListedGroupView> groups = await _groups.GetAllGroups(k, filter, query);
                return PartialView("_groupSingular", groups);
            }
            return BadRequest();
        }

        [HttpPost]
        [Authorize]
        [AccountConfirmed]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateGroup(GroupPageView model)
        {
            ListedGroupView listedGroup = new ListedGroupView();
            if (ModelState.IsValid)
            {
                listedGroup = model.PostageGroup;  // This refers to ListedGroupView within GroupPageView
            }
            Karter k = await _karter.GetUserByGoogleId(
                await _karter.GetCurrentUserNameIdentifier(User),
                withTrack: true
            );
            await _groups.CreateNewGroup(listedGroup, k);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        [AccountConfirmed]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JoinGroup(int GroupId)
        {
            Karter? k = await _karter.GetUserByGoogleId(
                await _karter.GetCurrentUserNameIdentifier(User),
                withTrack: true
            );
            if (k == null) return RedirectToAction("Index");
            await _groups.JoinGroup(GroupId, k);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        [AccountConfirmed]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LeaveGroup(int GroupId)
        {
            Karter? k = await _karter.GetUserByGoogleId(await _karter.GetCurrentUserNameIdentifier(User), withTrack: true);
            if (k == null) return RedirectToAction("Index");

            bool success = await _groups.LeaveGroup(GroupId, k);
            if (success)
            {
                return Ok("Left Group");
            }
            return BadRequest("Invalid Group Id For User");
        }

        [HttpGet]
        public async Task<IActionResult> Home(int GroupId)
        {
            Group? g = await _groups.GetGroupById(GroupId);
            if (g == null) return NotFound();

            GroupHomeView returnObj = new GroupHomeView
            {
                Posts = await _blog.GetModelToView(await _blog.GetAllPosts(new DataFilterOptions.BlogFilterOptions())),
                Group = await _groups.ToDTO(g),
                Members = await _groups.GetAllMembersProjection(GroupId),
                TrackTitles = await _track.GetAllTrackTitles()
            };
            return View(returnObj);
        }

        [HttpPost]
        [Authorize]
        [AccountConfirmed]
        [ValidateAntiForgeryToken]
        [ValidGroupMember]
        public async Task<JsonResult> Home(int GroupId, [FromBody] string message)
        {
            Karter? k = await _karter.GetUserByGoogleId(await _karter.GetCurrentUserNameIdentifier(User));
            bool res = await _groups.CreateUserMessageInGroup(GroupId, message, k);

            if (res) return Json(new { success = true, userName = k.Name });
            return Json(new { success = false });
        }

        public async Task<JsonResult> GroupStats(int GroupId, string TrackTitle)
        {
            List<GroupStatDisplay> statsToClient = await _groups.GetStatsForGroupGraph(GroupId, TrackTitle);
            return Json(statsToClient);
        }
    }
}