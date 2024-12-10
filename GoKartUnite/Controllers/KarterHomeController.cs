using AspNetCoreGeneratedDocument;
using GoKartUnite.Data;
using GoKartUnite.Handlers;
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
        private readonly KarterHandler _karters;
        public KarterHomeController(GoKartUniteContext context, KarterHandler karters) { 
            _context = context;
            _karters = karters;
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
            return View(await _karters.getAllUsers(true));
        }
        [HttpGet]
        public async Task<ActionResult> Create(int? id)
        {
            ViewBag.tracks = _context.Track.ToList();

            if (id != null)
            {
                var karterInDb = await _karters.getUser(id.Value);
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
            var prevKarterRecord =  await _karters.getUser(karter.Id);

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
        public async Task<ActionResult> Delete(int id)
        {
            var karter = await _karters.getUser(id);

            if (karter != null)
            {
                await _karters.deleteUser(karter.Id);
            }

            return RedirectToAction("Details");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendFriendRequest(string friendsName)
        {
            if (!await _karters.sendFriendRequest(friendsName))
            {
                return View("Index");
            }


            return View("Index");


        }


    }
}
