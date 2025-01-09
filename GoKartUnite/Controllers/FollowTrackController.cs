using GoKartUnite.Data;
using GoKartUnite.Handlers;
using GoKartUnite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using GoKartUnite.CustomAttributes;

namespace GoKartUnite.Controllers
{
    public class FollowTrackController : Controller
    {
        private readonly FollowerHandler _follows;
        private readonly KarterHandler _karters;
        private readonly TrackHandler _tracks;
        private readonly BlogHandler _blogs;
        public FollowTrackController(FollowerHandler follows, BlogHandler blogs, KarterHandler karters, TrackHandler tracks)
        {
            _follows = follows;
            _karters = karters;
            _tracks = tracks;
            _blogs = blogs;
        }

        [HttpGet]
        [Authorize]
        [AccountConfirmed]
        public async Task<IActionResult> Index(string track, string fullUrl)
        {
            string GoogleId = User.Claims
    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            Karter k = await _karters.getUserByGoogleId(GoogleId);
            List<Track> T = await _tracks.getTracksByTitle(track);

            if (k == null && T.Count == 0)
            {
                return Ok();
            }
            await _follows.CreateFollow(k.Id, T[0].Id);
            return Redirect(fullUrl);

        }
    }
}
