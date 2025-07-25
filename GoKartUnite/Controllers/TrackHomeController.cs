﻿using GoKartUnite.CustomAttributes;
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.DotNet.MSIdentity.Shared;
namespace GoKartUnite.Controllers
{
    [AccountConfirmed]
    [Authorize]
    public class TrackHomeController : Controller
    {
        private readonly GoKartUniteContext _context;
        private readonly ITrackHandler _tracks;
        private readonly IKarterHandler _karter;
        private readonly IFollowerHandler _follows;
        private readonly IBlogHandler _blog;
        private readonly IRoleHandler _role;

        public TrackHomeController(IRoleHandler role, IBlogHandler blog, GoKartUniteContext context, ITrackHandler tracks, IFollowerHandler follows, IKarterHandler karters)
        {
            _role = role;
            _context = context;
            _tracks = tracks;
            _karter = karters;
            _follows = follows;
            _blog = blog;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Track, Admin")]
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

        [HttpPost]

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

            return View("KartersLocalTrack", await _karter.GetAllUsersByTrackId(id.Value));
        }

        [HttpGet]
        [Authorize]
        [AccountConfirmed]
        public async Task<IActionResult> SearchTracks(string trackSearched, List<Locations> location)
        {
            string GoogleId = await _karter.GetCurrentUserNameIdentifier(User);
            Karter usr = await _karter.GetUserByGoogleId(GoogleId);

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
            string userClaimsId = await _karter.GetCurrentUserNameIdentifier(User);

            Karter userId = await _karter.GetUserByGoogleId(userClaimsId);
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
            string GoogleId = await _karter.GetCurrentUserNameIdentifier(User);
            return await _karter.GetUserByGoogleId(GoogleId);
        }
        [HttpGet]
        public async Task<IActionResult> GetPlaces(string query)
        {
            string filePath = "C:\\Users\\taela\\source\\repos\\NewUnite\\GoKartUnite\\secrets.txt";

            var keyValuePairs = new Dictionary<string, string>();
            foreach (var line in System.IO.File.ReadLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                var parts = line.Split('=');
                if (parts.Length == 2)
                {
                    keyValuePairs[parts[0].Trim()] = parts[1].Trim();
                }
            }

            string apiKey = "";
            if (keyValuePairs.ContainsKey("GoogleAPIPlacesKey"))
            {
                apiKey = keyValuePairs["GoogleAPIPlacesKey"];
            }
            var url = "https://places.googleapis.com/v1/places:searchText";

            var requestBody = new
            {
                textQuery = query,
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
            using HttpClient client = new HttpClient();

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("X-Goog-Api-Key", apiKey);
            client.DefaultRequestHeaders.Add("X-Goog-FieldMask", "places.displayName,places.formattedAddress,places.id,places.location");

            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(response.StatusCode.ToString());
            }
            string jsonResponse = await response.Content.ReadAsStringAsync();
            PlacesApiResponse? placesResponse = JsonConvert.DeserializeObject<PlacesApiResponse>(jsonResponse);

            if (placesResponse == null) return Content(jsonResponse, "application/json");

            var placeDTOs = placesResponse.Places.Select(p => new PlaceDTO
            {
                Name = p.DisplayName.Text,
                Location = p.FormattedAddress,
                GeoCoordinates = p.GeoCoordinates,
                Id = p.Id
            }).ToList();

            TempData["GooglePlacesResponse"] = JsonConvert.SerializeObject(placeDTOs);
            return Content(jsonResponse, "application/json");
        }
        [HttpPost]
        public async Task<IActionResult> SelectSearchedResult(string id)
        {
            bool res = false;
            try
            {
                var placesSerialized = TempData["GooglePlacesResponse"] as string;
                var placeDTOs = JsonConvert.DeserializeObject<List<PlaceDTO>>(placesSerialized);
                var placeSelected = placeDTOs.FirstOrDefault(p => p.Id == id);

                if (placeSelected == null)
                {
                    return BadRequest(new { success = false, message = "Track ID is not recognised in session storage" });
                }
                res = await _tracks.SetTrackToBeVerified(placeSelected.Name, placeSelected.Id, placeSelected.Location, placeSelected.GeoCoordinates, "Short Description as an example");

            }
            catch (Exception err)
            {
                return BadRequest("Data is not recognised in session storage");
            }

            if (res) return Ok(new { success = true });
            return BadRequest(new { success = false, message = "Data failed to be added to the db" });

        }
    }

    public class PlacesApiResponse
    {
        [JsonProperty("places")]
        public List<Place> Places { get; set; } = new List<Place>();
    }
    public class Place
    {
        [JsonProperty("displayName")]
        public required DisplayName DisplayName { get; set; }

        [JsonProperty("formattedAddress")]
        public required string FormattedAddress { get; set; }

        [JsonProperty("id")]
        public required string Id { get; set; }
        [JsonProperty("location")]
        public Coordinates GeoCoordinates { get; set; }
    }
    public class DisplayName
    {
        [JsonProperty("text")]
        public required string Text { get; set; }

        [JsonProperty("languageCode")]
        public required string LanguageCode { get; set; }
    }

    public class Coordinates
    {
        public Coordinates()
        {
            Latitude = 0;
            Longitude = 0;
        }
        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }
    }
    [Serializable]
    public class PlaceDTO
    {
        public required string Name { get; set; }
        public required string Location { get; set; }
        public required string Id { get; set; }
        public required Coordinates GeoCoordinates { get; set; }
    }

}
