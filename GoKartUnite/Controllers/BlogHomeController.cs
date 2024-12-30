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
        private readonly NotificationHandler _notification;
        private readonly FollowerHandler _followerHandler;
        public BlogHomeController(TrackHandler tracks, FollowerHandler followerHandler, BlogHandler blog, KarterHandler karter, NotificationHandler notification)
        {
            _blog = blog;
            _karter = karter;
            _notification = notification;
            _followerHandler = followerHandler;
            _tracks = tracks;
        }
        public async Task<IActionResult> Index(int page = 1, string? track = null)
        {
            ViewBag.TotalPages = await _blog.getTotalPageCount();
            page = Math.Min(page, ViewBag.TotalPages);
            page = Math.Max(page, 1);
            ViewBag.page = page;


            List<BlogPost> allPosts = await _blog.GetAllPosts(page, getTaggedTrack: true, trackFilter: track);

            if (allPosts.Count == 0) return View(await _blog.getModelToView(await _blog.GetAllPosts()));

            return View(await _blog.getModelToView(allPosts));
        }


        [HttpGet]
        [Microsoft.AspNetCore.Authorization.Authorize]
        [AccountConfirmed]
        public async Task<IActionResult> Create()
        {
            ViewBag.TrackTitles = await _tracks.GetAllTrackTitles();
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
                return RedirectToAction("Create");
            }

            string GoogleId = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            Karter k = await _karter.getUserByGoogleId(GoogleId);
            if (post.TaggedTrack != "")
            {
                Track taggedT = await _tracks.getSingleTrackByTitle(post.TaggedTrack);
                await _blog.addPost(post, k, taggedT);
            }
            else
            {
                await _blog.addPost(post, k);
            }
            int track = await _tracks.getTrackIdByTitle(post.TaggedTrack);

            List<int> kartersWhoNeedNotif = await _followerHandler.AllUserIdsWhoFollowTrack(track);
            if (post.TaggedTrack != "")
            {
                foreach (int kar in kartersWhoNeedNotif)
                {
                    await _notification.CreateNotification(NotificationType.Blog, kar);
                }
            }
            return RedirectToAction("Index");
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [AccountConfirmed]
        [HttpPost]
        public async Task UpvoteBlog(int id)
        {
            string GoogleId = User.Claims
    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            BlogPost post = await _blog.getPost(id, inclUpvotes: true);
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
