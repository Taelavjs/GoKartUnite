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
    public class FollowTrackController : Controller
    {
        private readonly IFollowerHandler _follows;
        private readonly IKarterHandler _karters;
        private readonly ITrackHandler _tracks;
        private readonly IBlogHandler _blogs;

        public FollowTrackController(IFollowerHandler follows, IBlogHandler blogs, IKarterHandler karters, ITrackHandler tracks)
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
            Karter k = await _karters.GetUserByGoogleId(GoogleId);
            List<Track> T = await _tracks.GetTracksByTitle(track);

            if (k == null && T.Count == 0)
            {
                return Ok();
            }
            await _follows.CreateFollow(k.Id, T[0].Id);
            return Redirect(fullUrl);

        }
    }
}
