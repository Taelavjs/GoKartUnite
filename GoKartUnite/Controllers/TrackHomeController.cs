using GoKartUnite.Data;
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
        public TrackHomeController(GoKartUniteContext context)
        {
            _context = context;
        }
        
        // GET: TrackHomeController
        public async Task<ActionResult> Index()
        {
            return View();
        }

        // GET: TrackHomeController/Create
        public async Task<ActionResult> Create()
        {
            return View();
        }

        // POST: TrackHomeController/Create
        [HttpPost]
        public async Task<IActionResult> Create(Track track)
        {
            var prevTrackRecord = await _context.Track.SingleOrDefaultAsync(t => t.Id == track.Id);

            if (!ModelState.IsValid)
            {
                ViewBag.Locations = Locations.GetNames(typeof(Locations));
                return View("Create");
            }
            if (prevTrackRecord == null)
            {
                _context.Track.Add(track);
            }
            else
            {
                prevTrackRecord.Title = track.Title;
                prevTrackRecord.Description = track.Description;
                prevTrackRecord.Location = track.Location;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details");
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            ViewBag.Locations = Locations.GetNames(typeof(Locations));
            if (id != null)
            {
                var trackInDb = await _context.Track.SingleOrDefaultAsync(x => x.Id == id);
                ViewData["Title"] = "Editing Track Details";
                return View(trackInDb);
            }
            ViewData["Title"] = "Creating Karter Profile";
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var track = await _context.Track.SingleAsync(t => t.Id == id);
            _context.Track.Remove(track);
            await _context.SaveChangesAsync();
            return View("Details");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            // Find the track with its related karters
            var track = await _context.Track
                .Include(t => t.Karters)
                .FirstOrDefaultAsync(t => t.Id == id);

            if(track == null) {
                RedirectToAction("Details");
            }

            foreach(var karter in track.Karters)
            {
                karter.Track = null;
                karter.TrackId = null;
            }
            _context.Track.Remove(track);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return View(_context.Track.ToList());

            var karters = await _context.Karter
                .Where(k => k.TrackId == id)
                .ToListAsync();


            return View("KartersLocalTrack", karters);
        }

    }
}
