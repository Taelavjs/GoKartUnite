using GoKartUnite.CustomAttributes;
using GoKartUnite.Data;
using GoKartUnite.Handlers;
using GoKartUnite.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using NuGet.Protocol.Core.Types;
using System.Reflection.Metadata;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using GoKartUnite.Interfaces;
using GoKartUnite.ViewModel;

namespace GoKartUnite.Controllers
{
    public class TrackHomeController : Controller
    {
        private readonly GoKartUniteContext _context;
        private readonly ITrackHandler _tracks;
        private readonly IKarterHandler _karters;
        private readonly IFollowerHandler _follows;
        private readonly IBlogHandler _blog;
        private readonly IRoleHandler _role;

        public TrackHomeController(IRoleHandler role, IBlogHandler blog, GoKartUniteContext context, ITrackHandler tracks, IFollowerHandler follows, IKarterHandler karters)
        {
            _role = role;
            _context = context;
            _tracks = tracks;
            _karters = karters;
            _follows = follows;
            _blog = blog;
        }

        // GET: TrackHomeController

        [HttpGet]
        [AccountConfirmed]
        [Authorize]
        public async Task<ActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Track, Admin")]
        [AccountConfirmed]
        public async Task<IActionResult> Create(Track track)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Locations = Locations.GetNames(typeof(Locations));
                return View("Create");
            }
            _ = await _tracks.AddTrack(track);
            return RedirectToAction("Details");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [AccountConfirmed]
        public async Task<IActionResult> Create(int? id)
        {
            ViewBag.Locations = Locations.GetNames(typeof(Locations));
            if (id != null)
            {
                var trackInDb = await _tracks.GetTrack(id.Value, false);
                ViewData["Title"] = "Editing Track Details";
                return View(trackInDb);
            }
            ViewData["Title"] = "Creating Karter Profile";
            return View();
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Track, Admin")]
        //[AccountConfirmed]
        //public async Task<IActionResult> Edit(int id)
        //{
        //    _tracks.UpdateTrack(id);
        //    return View("Details");
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [AccountConfirmed]
        public async Task<ActionResult> Delete(int id)
        {
            // Find the track with its related karters
            bool res = await _tracks.DeleteTrack(id);

            if (res)
            {
                return RedirectToAction("Details");
            }
            return RedirectToAction("Details");
        }

        [HttpGet]
        [Authorize]
        [AccountConfirmed]
        public async Task<IActionResult> Details(int? id)
        {
            var tracksRender = _tracks.ModelToView(await _tracks.GetAllTracks());
            if (id == null) return View(await tracksRender);

            return View("KartersLocalTrack", await _karters.GetAllUsersByTrackId(id.Value));
        }

        [HttpGet]
        [Authorize]
        [AccountConfirmed]
        public async Task<IActionResult> SearchTracks(string trackSearched, List<Locations> location)
        {
            string GoogleId = User.Claims
    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            Karter usr = await _karters.GetUserByGoogleId(GoogleId);

            List<TrackView> tracks = await _tracks.ModelToView(await _tracks.GetTracksByTitle(trackSearched, location));
            foreach (TrackView track in tracks)
            {
                if (await _follows.DoesUserFollow(usr.Id, await _tracks.GetTrackIdByTitle(track.Title)))
                {
                    track.isFollowed = true;
                }
            }


            return View("Details", tracks);

        }

        [HttpGet]
        [Authorize]
        [AccountConfirmed]
        public async Task<IActionResult> FilteredIndex(string track = "")
        {

            return RedirectToAction("Index", "BlogHome", new { track });
        }

        [HttpGet]
        [Authorize]
        [AccountConfirmed]
        public async Task<IActionResult> FilteredKarterIndex(string track = "")
        {

            return RedirectToAction("DetailsByTrack", "KarterHome", new { track });
        }

        [HttpGet]
        [Authorize]
        [AccountConfirmed]
        public async Task<IActionResult> Profile(string track)
        {
            Track toReply = await _tracks.GetSingleTrackByTitle(track);
            if (toReply == null) return RedirectToAction("index");
            ViewData["Title"] = toReply.Title;

            TrackView trackView = await _tracks.ModelToView(toReply);

            List<BlogPost> posts = await _blog.GetPostsForTrack(track, 10);

            List<BlogPostView> postViews = await _blog.GetModelToView(posts);
            string userClaimsId = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            Karter userId = await _karters.GetUserByGoogleId(userClaimsId);
            bool isAdminAtTrack = await _role.IsAdminAtTrack(track, userId.Id);

            TrackProfile trackProfile = new TrackProfile
            {
                trackView = trackView,
                posts = postViews,
                isAdminAtTrack = isAdminAtTrack
            };
            return View(trackProfile);

        }

        private async Task<Karter> GetKarterFromUUID()
        {
            string GoogleId = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            return await _karters.GetUserByGoogleId(GoogleId);
        }

    }
}
