using GoKartUnite.CustomAttributes;
using GoKartUnite.Data;
using GoKartUnite.Handlers;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using NuGet.Protocol.Core.Types;
using System.Reflection.Metadata;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace GoKartUnite.Controllers
{
    public class TrackHomeController : Controller
    {
        private readonly GoKartUniteContext _context;
        private readonly TrackHandler _tracks;
        private readonly KarterHandler _karters;
        private readonly FollowerHandler _follows;
        public TrackHomeController(GoKartUniteContext context, TrackHandler tracks, FollowerHandler follows, KarterHandler karters)
        {
            _context = context;
            _tracks = tracks;
            _karters = karters;
            _follows = follows;
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
            _ = await _tracks.addTrack(track);
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
                var trackInDb = await _tracks.getTrack(id.Value, false);
                ViewData["Title"] = "Editing Track Details";
                return View(trackInDb);
            }
            ViewData["Title"] = "Creating Karter Profile";
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Track, Admin")]
        [AccountConfirmed]
        public async Task<IActionResult> Edit(int id)
        {
            _tracks.updateTrack(id);
            return View("Details");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [AccountConfirmed]
        public async Task<ActionResult> Delete(int id)
        {
            // Find the track with its related karters
            bool res = await _tracks.deleteTrack(id);

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
            var tracksRender = _tracks.modelToView(await _tracks.getAllTracks());
            if (id == null) return View(await tracksRender);

            return View("KartersLocalTrack", await _karters.getAllUsersByTrackId(id.Value));
        }

        [HttpGet]
        [Authorize]
        [AccountConfirmed]
        public async Task<IActionResult> SearchTracks(string trackSearched, List<Locations> location)
        {
            string GoogleId = User.Claims
    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            Karter usr = await _karters.getUserByGoogleId(GoogleId);

            List<TrackView> tracks = await _tracks.modelToView(await _tracks.getTracksByTitle(trackSearched, location));
            foreach (TrackView track in tracks)
            {
                if (await _follows.doesUserFollow(usr.Id, await _tracks.getTrackIdByTitle(track.Title)))
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

    }
}
