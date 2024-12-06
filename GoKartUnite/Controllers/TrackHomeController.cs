using GoKartUnite.Data;
using GoKartUnite.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details");
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
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

        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            var track = await _context.Track.SingleAsync(t => t.Id == id);
            _context.Track.Remove(track);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            return View(_context.Track.ToList());
        }

    }
}
