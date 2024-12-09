using GoKartUnite.Data;
using GoKartUnite.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace GoKartUnite.Controllers
{
    public class KarterHomeController : Controller
    {
        private readonly GoKartUniteContext _context;
        public KarterHomeController(GoKartUniteContext context) { 
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult> Index(string kartersName)
        {
            var karters = from m in _context.Karter
                         select m;

            if (!String.IsNullOrEmpty(kartersName))
            {
                karters = karters.Where(s=>s.Name.ToUpper() == kartersName.ToUpper());
                return View("Details", karters);
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            return View(_context.Karter.Include(k => k.Track).ToList());
        }
        [HttpGet]
        public async Task<ActionResult> Create(int? id)
        {
            ViewBag.tracks = _context.Track.ToList();

            if (id != null)
            {
                var karterInDb = await _context.Karter.SingleOrDefaultAsync(x => x.Id == id);
                ViewData["Title"] = "Editing Karter Details";
                return View(karterInDb);
            }
            ViewData["Title"] = "Creating Karter Profile";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Karter karter)
        {
            karter.Track = await _context.Track.SingleOrDefaultAsync(t => t.Id == karter.TrackId);
            var prevKarterRecord =  await _context.Karter.SingleOrDefaultAsync(t => t.Id == karter.Id);

            if (!ModelState.IsValid)
            {
                ViewBag.tracks = _context.Track.ToList();
                return View("Create");  
            }
            if (prevKarterRecord == null)
            {
                _context.Karter.Add(karter);
            } else
            {
                prevKarterRecord.Name = karter.Name;
                prevKarterRecord.Track = karter.Track;
                prevKarterRecord.TrackId = karter.TrackId;
                prevKarterRecord.TrackId = karter.TrackId;
                prevKarterRecord.YearsExperience = karter.YearsExperience;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var karter = await _context.Karter.SingleAsync(t => t.Id == id);
            _context.Karter.Remove(karter);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            var karter = await _context.Karter
                .FirstOrDefaultAsync(k => k.Id == id);

            if (karter != null)
            {
                var friendshipsToRemove = await _context.Friendships
                    .Where(f => f.KarterFirstId == karter.Id || f.KarterSecondId == karter.Id).ToListAsync();

                _context.Friendships.RemoveRange(friendshipsToRemove);
                _context.Karter.Remove(karter);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendFriendRequest(string friendsName)
        {
            var karterSecond2 = await _context.Karter.SingleOrDefaultAsync(k => k.Name.ToLower() == "taela");

            var karter = await _context.Karter.SingleOrDefaultAsync(k => k.Name.ToLower() == friendsName.ToLower());
            if(karter == null || karterSecond2 == null)
            {
                return View("Index");
            }
            var ifExists = await _context.Friendships.FindAsync(karter.Id, karterSecond2.Id);  
            if (ifExists != null)
            {
                return View("Index");
            }

            var friendship = new Friendships(karter.Id, karterSecond2.Id);
            _context.Attach(karter);
            _context.Attach(karterSecond2);

            await _context.Friendships.AddAsync(friendship);
            await _context.SaveChangesAsync();

            return View("Index");
        }


    }
}
