using GoKartUnite.CustomAttributes;
using GoKartUnite.Handlers;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace GoKartUnite.Controllers
{
    public class BlogHomeController : Controller
    {
        private readonly BlogHandler _blog;
        private readonly KarterHandler _karter;

        private readonly TrackHandler _tracks;
        public BlogHomeController(BlogHandler blog, KarterHandler karter)
        {
            _blog = blog;
            _karter = karter;
        }
        public async Task<IActionResult> Index()
        {
            List<BlogPost> allPosts = await _blog.getAllPosts();
            


            return View(await _blog.getModelToView(allPosts));
        }
        [HttpGet]
        [Microsoft.AspNetCore.Authorization.Authorize]
        [AccountConfirmed]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Authorization.Authorize]
        [AccountConfirmed]
        public async Task<ActionResult> Create(BlogPostView post)
        {
            if (!ModelState.IsValid)
            {
                return View("Create");
            }

            string email = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;

            Karter k = await _karter.getUserByEmail(email);
            
            _blog.addPost(post, k);

            return RedirectToAction("Index");
        }
    }
}
