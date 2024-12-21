using GoKartUnite.CustomAttributes;
using GoKartUnite.Data;
using GoKartUnite.Handlers;
using GoKartUnite.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;

namespace GoKartUnite.Controllers
{
    public class TrackHomeController : Controller
    {
        private readonly GoKartUniteContext _context;
        private readonly TrackHandler _tracks;
        private readonly KarterHandler _karters;
        public TrackHomeController(GoKartUniteContext context, TrackHandler tracks, KarterHandler karters)
        {
            _context = context;
            _tracks = tracks;
            _karters = karters;
        }

        // GET: TrackHomeController

        [HttpGet]
        [AccountConfirmed]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult> Index()
        {
            return View();
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [AccountConfirmed]
        public async Task<ActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize]
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
        [Microsoft.AspNetCore.Authorization.Authorize]
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
        [Microsoft.AspNetCore.Authorization.Authorize]
        [AccountConfirmed]
        public async Task<IActionResult> Edit(int id)
        {
            _tracks.updateTrack(id);
            return View("Details");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Authorization.Authorize]
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
        [Microsoft.AspNetCore.Authorization.Authorize]
        [AccountConfirmed]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return View(await _tracks.getAllTracks());




            return View("KartersLocalTrack", await _karters.getAllUsersByTrackId(id.Value));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Authorization.Authorize]
        [AccountConfirmed]
        public async Task<IActionResult> SearchTracks(string trackSearched, List<Locations> location)
        {
            List<Track> tracks = await _tracks.getTrackByTitle(trackSearched, location);

            return View("Details", tracks);

        }




    }
}
