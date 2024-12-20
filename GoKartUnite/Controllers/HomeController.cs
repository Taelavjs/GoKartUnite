using System.Diagnostics;
using GoKartUnite.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Mvc;
using GoKartUnite.CustomAttributes;

namespace GoKartUnite.Controllers
{
    public class HomeController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
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

        [Microsoft.AspNetCore.Mvc.HttpGet("/login")]
        [AllowAnonymous] // Allow unauthenticated users to access this endpoint
        public async Task<IActionResult> Login()
        {
            // Trigger Google login
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("Create", "KarterHome") 
            };

            return Challenge(authenticationProperties, GoogleDefaults.AuthenticationScheme);
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("/logout")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
