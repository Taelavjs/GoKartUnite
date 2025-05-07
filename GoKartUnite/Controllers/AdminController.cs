using GoKartUnite.CustomAttributes;
using GoKartUnite.DataFilterOptions;
using GoKartUnite.Handlers;
using GoKartUnite.Interfaces;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;
using Microsoft.AspNetCore.Mvc;
using SendGrid.Helpers.Mail;
using System.Web.Helpers;
using System.Web.Mvc;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;

namespace GoKartUnite.Controllers
{
    [Authorize(Roles = "Admin")]
    [AccountConfirmed]
    public class AdminController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IKarterHandler _karter;
        private readonly ITrackHandler _track;

        public AdminController(IKarterHandler karter, ITrackHandler track)
        {
            _karter = karter;
            _track = track;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ManageUsers()
        {
            KarterGetAllUsersFilter filter = new KarterGetAllUsersFilter
            {
                IncludeTrack = true
            };
            List<Karter> karters = await _karter.GetAllUsers(filter);
            return View(karters);
        }

        [HttpGet]
        public async Task<IActionResult> VerifyTracks()
        {
            return View(await _track.GetUnverifiedTracksAdmin());
        }
        [HttpPost]
        public async Task<IActionResult> VerifyTracks(int id)
        {
            bool res = await _track.VerifyTrack(id);



            if (res) return Ok("Successfully verified");
            return BadRequest("ID not found");
        }

        [HttpPost]
        public async Task<IActionResult> DeVerifyTracks(int id)
        {
            bool res = await _track.DeleteTrack(id);

            if (res) return Ok("Successfully verified");
            return BadRequest("ID not found");
        }

        [HttpGet]
        public async Task<IActionResult> ManageKarters()
        {
            var AllKarters = await _karter.GetAllUsersAdmin();
            return View(AllKarters);
        }

        [HttpGet]
        public async Task<IActionResult> ManageUser(int id)
        {
            var getKarter = await _karter.GetUser(id);
            return View(getKarter);
        }

        [HttpGet]
        public async Task<IActionResult> GetKartersBlogPosts(int karterId)
        {
            return PartialView("_KarterBlogPosts", await _karter.GetAllUserPostsAdmin(karterId));
        }


        [HttpGet]
        public async Task<IActionResult> GetKartersComments(int karterId)
        {
            return PartialView("_KarterComments", await _karter.GetAllUsersCommentsAdmin(karterId));
        }

        [HttpGet]
        public async Task<IActionResult> GetKartersGroups(int karterId)
        {
            return PartialView("_KarterGroups", await _karter.GetAllUsersGroupsAdmin(karterId));
        }

        [HttpGet]
        public async Task<IActionResult> GetKarterMessagesInGroup(int karterId, int groupId)
        {
            return PartialView("_KarterMessagesByGroup", await _karter.GetUsersMessagesByGroup(karterId, groupId));
        }


        [HttpGet]
        public async Task<IActionResult> GetListKartersGroups(int karterId, int groupId)
        {
            return Json(new { succes = true, message = await _karter.GetUserGroupsList(karterId) });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteKarterById(int id)
        {
            try
            {
                bool res = await _karter.DeleteUserAdmin(id);
                return Json(new { success = res, message = "User Deleted Successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest("Server Error");
            }

        }

        private async Task<List<KarterAdminView>> KarterModelToAdminView(List<Karter> kartersList)
        {
            List<KarterAdminView> toReturn = new List<KarterAdminView>();
            foreach (Karter k in kartersList)
            {
                toReturn.Add(new KarterAdminView
                {
                    Name = k.Name,
                    Email = k.Email,
                    Id = k.Id,
                    Track = k.Track,
                    TrackId = k.TrackId,
                    BlogPosts = k.BlogPosts.Count,
                    NameIdentifier = k.NameIdentifier,
                    Friendships = k.Friendships.Count,
                    Notification = k.Notification.Count,
                    YearsExperience = k.YearsExperience,
                });

            }
            return toReturn;

        }
    }
}