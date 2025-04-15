using GoKartUnite.CustomAttributes;
using GoKartUnite.DataFilterOptions;
using GoKartUnite.Handlers;
using GoKartUnite.Interfaces;
using GoKartUnite.Models;
using Microsoft.AspNetCore.Mvc;
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
    }
}
