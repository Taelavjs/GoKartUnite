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

namespace GoKartUnite.Controllers
{
    public class HomeController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RelationshipHandler _friends;
        private readonly BlogHandler _blog;
        private readonly KarterHandler _karters;

        public HomeController(ILogger<HomeController> logger, KarterHandler karters, RelationshipHandler friends, BlogHandler blog)
        {
            _logger = logger;
            _friends = friends;
            _blog = blog;
            _karters = karters;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (User.Identity == null || User.Identity.IsAuthenticated == false)
            {
                return View();
            }
            Karter k = await _karters.getUserByGoogleId(User.Claims
.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value, withTrack: true);
            List<Karter> friends = await _friends.getAllFriends(k.Id);
            List<BlogPost> blogPosts = new List<BlogPost>();

            foreach (var friend in friends)
            {
                blogPosts.AddRange(await _blog.GetAllPostsFromUser(friend.Id));
            }
            blogPosts = blogPosts.OrderByDescending(x => x.DateTimePosted).ToList();



            return View(await _blog.getModelToView(blogPosts));

        }

        [HttpPost]
        [Authorize]
        [AccountConfirmed]
        [EnableRateLimiting("slidingPolicy")]
        public async Task<IActionResult> InfiniteScroll(int pagesScrolled = 0)
        {
            Karter k = await _karters.getUserByGoogleId(User.Claims
.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value, withTrack: true);
            List<Karter> friends = await _friends.getAllFriends(k.Id);
            List<BlogPost> blogPosts = new List<BlogPost>();

            foreach (var friend in friends)
            {
                blogPosts.AddRange(await _blog.GetAllPostsFromUser(friend.Id, pagesScrolled));
            }
            blogPosts = blogPosts.OrderByDescending(x => x.DateTimePosted).ToList();
            return PartialView("~/Views/BlogHome/_Posts.cshtml", await _blog.getModelToView(blogPosts));
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
}
