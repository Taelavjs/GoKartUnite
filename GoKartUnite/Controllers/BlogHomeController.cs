using GoKartUnite.CustomAttributes;
using GoKartUnite.Handlers;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

            string GoogleId = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            Karter k = await _karter.getUserByGoogleId(GoogleId);
            
            await _blog.addPost(post, k);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task UpvoteBlog(int id)
        {
            string GoogleId = User.Claims
    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            BlogPost post = await _blog.getPost(id, inclUpvotes : true);
            Karter karter = await _karter.getUserByGoogleId(GoogleId);

            bool alreadyUpvoted = post.Upvotes
                .Any(upvote => upvote.VoterId == karter.Id);

            if (alreadyUpvoted)
            {

                return;
            }
            var upvote = new Upvotes();
            upvote.PostId = id;
            upvote.VoterId = karter.Id;
            await _blog.upvotePost(id, upvote);
        }
    }
}
