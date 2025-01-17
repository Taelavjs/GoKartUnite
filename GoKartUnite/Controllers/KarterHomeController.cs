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

            // Adds Status to KarterView Lists

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
        public async Task<IActionResult> DetailsByTrack(string? track, int page = 0, SortKartersBy sortby = SortKartersBy.Alphabetically)
        {
            string googleId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            Karter k = await _karters.getUserByGoogleId(googleId);
            ViewBag.TotalPages = await _karters.GetNumberOfUserPages(track);
            page = Math.Max(0, Math.Min(page, ViewBag.TotalPages));
            List<Karter> karters = await _karters.getAllUsers(false, track, page <= 0 ? 0 : page - 1, sort: sortby);
            List<KarterView> karterViews = await _karters.karterModelToView(karters);

            karterViews = await _friendships.AddStatusToKarters(karterViews, k.Id);


            ViewBag.page = page;

            ViewBag.CurrentSort = sortby;
            return View("Details", karterViews);
        }

        // ======================================
        // CREATE METHODS
        // ======================================
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Create(int id)
        {
            ViewBag.TrackTitles = await _tracks.getAllTracks();

            ViewData["ButtonValue"] = "Create";
            ViewData["Title"] = "Creating Karter Profile";

            return View();
        }
        [HttpGet]
        [Authorize]
        [AccountConfirmed]
        public async Task<ActionResult> EditUserDetails()
        {
            string googleId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            Karter k = await _karters.getUserByGoogleId(googleId);
            ViewData["Title"] = "Editing Karter Details";
            ViewData["ButtonValue"] = "UpdateUser";
            ViewBag.TrackTitles = await _tracks.getAllTracks();

            return View("Create", await _karters.karterModelToView(k));
        }
        [HttpPost]
        [Authorize]
        [AccountConfirmed]
        public async Task<ActionResult> UpdateUser(KarterView kv)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.TrackTitles = await _tracks.getAllTracks();
                return View("Create");
            }
            string googleId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            await _karters.UpdateUser(googleId, kv);
            return RedirectToAction("Index");
        }


        // Create accoount method for new users - Triggered with successful
        // Login with google but no user account found
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


        // ======================================
        // CREATE METHODS
        // ======================================

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

        // ======================================
        // FRIEND REQUEST METHODS
        // ======================================

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

        // Handles different button presses via partial karter view
        [HttpPost]
        [Authorize]
        [AccountConfirmed]
        [ValidateAntiForgeryToken]
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

    // ======================================
    // FRIEND REQUEST METHODS
    // ======================================
}
