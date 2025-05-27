using GoKartUnite.Data;
using GoKartUnite.Handlers;
using GoKartUnite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using GoKartUnite.CustomAttributes;
using GoKartUnite.Interfaces;

namespace GoKartUnite.Controllers
{
    [Authorize]
    [AccountConfirmed]
    public class FollowTrackController : Controller
    {
        private readonly IFollowerHandler _follows;
        private readonly IKarterHandler _karter;
        private readonly ITrackHandler _tracks;
        private readonly IBlogHandler _blogs;

        public FollowTrackController(IFollowerHandler follows, IBlogHandler blogs, IKarterHandler karters, ITrackHandler tracks)
        {
            _follows = follows;
            _karter = karters;
            _tracks = tracks;
            _blogs = blogs;
        }

        [HttpGet]

        public async Task<IActionResult> Index(string track)
        {
            string GoogleId = await _karter.GetCurrentUserNameIdentifier(User);
            Karter k = await _karter.GetUserByGoogleId(GoogleId);
            List<Track> T = await _tracks.GetTracksByTitle(track);

            if (k == null || T.Count == 0)
            {
                return NotFound(new { success = false, message = "Bad Inputs" });
            }
            bool success = await _follows.CreateFollow(k.Id, T[0].Id);
            if (success) return Ok(new { success = true, message = "Successful follow requerst" });
            return NotFound(new { success = false, message = "Bad Inputs" });
        }
    }
}
