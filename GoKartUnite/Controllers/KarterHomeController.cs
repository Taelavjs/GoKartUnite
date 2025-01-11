using AspNetCoreGeneratedDocument;
using GoKartUnite.CustomAttributes;
using GoKartUnite.Data;
using GoKartUnite.Handlers;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace GoKartUnite.Controllers
{
    public class KarterHomeController : Controller
    {
        private readonly KarterHandler _karters;
        private readonly TrackHandler _tracks;
        private readonly RoleHandler _roles;
        private readonly RelationshipHandler _friendships;
        public KarterHomeController(RelationshipHandler friendships, KarterHandler karters, TrackHandler tracks, RoleHandler roles)
        {
            _karters = karters;
            _tracks = tracks;
            _roles = roles;
            _friendships = friendships;
        }
        [HttpGet]
        [Authorize]
        [AccountConfirmed]
        public async Task<ActionResult> Index(string kartersName)
        {
            if (!String.IsNullOrEmpty(kartersName))
            {
                var karters = await _karters.getUser(kartersName);
                return View("Details", new List<Karter> { karters });
            }

            Karter k = await _karters.getUserByGoogleId(User.Claims
    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value, withTrack: true);

            KarterIndex store = new KarterIndex
            {
                karter = k,
                karterFriends = await _karters.karterModelToView(await _friendships.getAllFriends(k.Id)),
                karterFriendRequests = await _karters.karterModelToView(await _friendships.getAllFriendRequests(k.Id)),
                sentFriendRequests = await _karters.karterModelToView(await _friendships.getAllSentRequests(k.Id))
            };

            foreach (var kar in store.karterFriendRequests)
            {
                kar.FriendStatus = FriendshipStatus.Requested;
            }

            foreach (var kar in store.karterFriends)
            {
                kar.FriendStatus = FriendshipStatus.Friend;
            }

            foreach (var kar in store.sentFriendRequests)
            {
                kar.FriendStatus = FriendshipStatus.Received;
            }
            return View(store);
        }


        [HttpGet]
        [Authorize]
        [AccountConfirmed]
        public async Task<IActionResult> Details(int? id)
        {
            string googleId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            Karter k = await _karters.getUserByGoogleId(googleId);

            List<Karter> karters = await _karters.getAllUsers(true);
            List<KarterView> karterViews = await _karters.karterModelToView(karters);

            karterViews = await _friendships.AddStatusToKarters(karterViews, k.Id);
            return View(karterViews);
        }


        [HttpGet]
        [Authorize]
        [AccountConfirmed]
        public async Task<IActionResult> DetailsByTrack(string? track)
        {
            string googleId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            Karter k = await _karters.getUserByGoogleId(googleId);
            List<Karter> karters = await _karters.getAllUsers(false, track);
            List<KarterView> karterViews = await _karters.karterModelToView(karters);

            karterViews = await _friendships.AddStatusToKarters(karterViews, k.Id);
            return View("Details", karterViews);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Create(int? id)
        {
            ViewBag.TrackTitles = await _tracks.getAllTracks();

            if (id != null)
            {
                var karterInDb = await _karters.getUser(id.Value);
                ViewData["Title"] = "Editing Karter Details";

                return View(await _karters.karterModelToView(karterInDb));
            }
            ViewData["Title"] = "Creating Karter Profile";

            return View();
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(KarterView kv)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.tracks = await _tracks.getAllTracks();
                return View("Create");
            }

            string NameIdentifier = User.Claims
    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            Karter karter = new Karter();
            karter.Id = kv.Id == 0 ? 0 : kv.Id;
            karter.Name = kv.Name;
            karter.TrackId = kv.TrackId;
            karter.YearsExperience = kv.YearsExperience;
            karter.NameIdentifier = NameIdentifier;



            await _karters.createUser(karter,
                User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value);
            await _roles.AddRoleToUser(karter.Id, "User");

            return RedirectToAction("Details");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [AccountConfirmed]
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
        [Authorize]
        [AccountConfirmed]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendFriendRequestByName(string friendId)
        {
            string googleId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            if (!await _karters.sendFriendRequestByName(friendId, googleId))
            {
                return View("Index");
            }


            return View("Index");
        }
        [HttpPost]
        [Authorize]
        [AccountConfirmed]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendFriendRequestById(int friendId)
        {
            string googleId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            if (!await _karters.sendFriendRequestById(friendId, googleId))
            {
                return View("Index");
            }


            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> HandleFriendRequest(int friendId, string action)
        {
            if (action == "Accept")
            {
                await AcceptFriendRequest(friendId);
            }
            else if (action == "Add")
            {
                await SendFriendRequestById(friendId);
            }
            else if (action == "Remove")
            {
                await RemoveFriendRequest(friendId);
            }
            else if (action == "Cancel")
            {
                await RemoveFriendRequest(friendId);
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize]
        [AccountConfirmed]
        [ValidateAntiForgeryToken]
        private async Task AcceptFriendRequest(int friendId)
        {
            string googleId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            Karter k = await _karters.getUserByGoogleId(googleId);

            await _friendships.AcceptFriendRequest(k.Id, friendId);
        }
        [HttpPost]
        [Authorize]
        [AccountConfirmed]
        [ValidateAntiForgeryToken]
        public async Task RemoveFriendRequest(int friendId)
        {
            string googleId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            Karter k = await _karters.getUserByGoogleId(googleId);

            await _friendships.RemoveFriendShip(k.Id, friendId);
        }
    }
}
