using AspNetCoreGeneratedDocument;
using GoKartUnite.CustomAttributes;
using GoKartUnite.Data;
using GoKartUnite.Handlers;
using GoKartUnite.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace GoKartUnite.Controllers
{
    public class KarterHomeController : Controller
    {
        private readonly KarterHandler _karters;
        private readonly TrackHandler _tracks;
        public KarterHomeController(KarterHandler karters, TrackHandler tracks) { 
            _karters = karters;
            _tracks = tracks;
        }
        [HttpGet]
        [Microsoft.AspNetCore.Authorization.Authorize]
        [AccountConfirmed]
        public async Task<ActionResult> Index(string kartersName)
        {


            if (!String.IsNullOrEmpty(kartersName))
            {
                var karters = await _karters.getUser(kartersName);
                return View("Details", new List<Karter> { karters });
            }
            return View();
        }
        [HttpGet]
        [Microsoft.AspNetCore.Authorization.Authorize]
        [AccountConfirmed]
        public async Task<IActionResult> Details(int? id)
        {
            Console.WriteLine(User.Claims);

            return View(await _karters.getAllUsers(true));
        }
        [HttpGet]
        [Microsoft.AspNetCore.Authorization.Authorize]
        [AccountConfirmed]
        public async Task<ActionResult> Create(int? id)
        {
            ViewBag.tracks = await _tracks.getAllTracks();

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
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult> Create(Karter karter)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.tracks = await _tracks.getAllTracks();
                return View("Create");  
            }
            await _karters.createUser(karter, User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value);
            return RedirectToAction("Details");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Authorization.Authorize]
        [AccountConfirmed]
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
        [Microsoft.AspNetCore.Authorization.Authorize]
        [AccountConfirmed]
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
