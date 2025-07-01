using System.Diagnostics;
using GoKartUnite.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Mvc;
using GoKartUnite.CustomAttributes;
using Authroize = Microsoft.AspNetCore.Authorization;
using AllowAnonymousAttribute = Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using GoKartUnite.Handlers;
using System.Security.Claims;
using GoKartUnite.ViewModel;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using Microsoft.AspNetCore.RateLimiting;
using GoKartUnite.DataFilterOptions;
using Microsoft.Extensions.Configuration.UserSecrets;
using GoKartUnite.Interfaces;
using PartialViewResult = Microsoft.AspNetCore.Mvc.PartialViewResult;
using PagedList;

namespace GoKartUnite.Controllers
{
    public class HomeController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRelationshipHandler _friends;
        private readonly IBlogHandler _blog;
        private readonly IKarterHandler _karter;
        private readonly ITrackHandler _track;

        public HomeController(ITrackHandler track, ILogger<HomeController> logger, IKarterHandler karters, IRelationshipHandler friends, IBlogHandler blog)
        {
            _logger = logger;
            _friends = friends;
            _blog = blog;
            _karter = karters;
            _track = track;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 6)
        {
            if (User.Identity == null || User.Identity.IsAuthenticated == false)
            {
                return View();
            }
            List<BlogPost> blogPosts = new List<BlogPost>();

            Karter k = await _karter.GetUserByGoogleId(await _karter.GetCurrentUserNameIdentifier(User), withTrack: true);

            if (k != null)
            {
                List<Karter> friends = await _friends.GetAllFriends(k.Id);
                foreach (var friend in friends)
                {
                    BlogFilterOptions filter = new BlogFilterOptions
                    {
                        UserIdFilter = friend.Id,
                        IncludeUpvotes = true

                    };
                    blogPosts.AddRange(await _blog.GetAllPosts(filter));
                }
                blogPosts = blogPosts.OrderByDescending(x => x.DateTimePosted).ToList();
            }


            List<int> trackIdsRecommended = await _track.CalculateRecommendedTracksForUser(k.Id);
            List<PartialViewResult> views = new List<PartialViewResult>();
            List<Track> tracks = new List<Track>();

            foreach (var id in trackIdsRecommended)
            {
                var track = await _track.GetTrackById(id);
                if (track == null) { continue; }
                tracks.Add(track);
            }
            var t = await _track.ModelToView(tracks);

            var homepg = new HomePageData
            {
                Posts = await _blog.GetModelToView(blogPosts),
                Tracks = t.ToPagedList(pageNumber, pageSize),
            };


            return View(homepg);

        }

        [HttpPost]
        [Authorize]
        [AccountConfirmed]
        [EnableRateLimiting("slidingPolicy")]
        public async Task<IActionResult> InfiniteScroll(int pagesScrolled = 0)
        {
            Karter k = await _karter.GetUserByGoogleId(await _karter.GetCurrentUserNameIdentifier(User), withTrack: true);
            List<Karter> friends = await _friends.GetAllFriends(k.Id);
            List<BlogPost> blogPosts = new List<BlogPost>();

            foreach (var friend in friends)
            {
                BlogFilterOptions filter = new BlogFilterOptions
                {
                    UserIdFilter = friend.Id,
                    PageNo = pagesScrolled,
                    IncludeUpvotes = true
                };
                blogPosts.AddRange(await _blog.GetAllPosts(filter));
            }
            blogPosts = blogPosts.OrderByDescending(x => x.DateTimePosted).ToList();
            return PartialView("~/Views/BlogHome/_Posts.cshtml", await _blog.GetModelToView(blogPosts));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet("/login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            // Trigger Google login
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("Create", "KarterHome")
            };

            return Challenge(authenticationProperties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("/logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }


    }

    class HomePageData
    {
        public List<BlogPostView> Posts { get; set; } = new List<BlogPostView>();
        public IPagedList<TrackView> Tracks { get; set; }
    }
}
