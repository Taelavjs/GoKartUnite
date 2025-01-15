using GoKartUnite.CustomAttributes;
using GoKartUnite.Handlers;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

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
        [HttpGet]
        [Authorize]
        [AccountConfirmed]
        public async Task<IActionResult> Index(int page = 1, string? track = null)
        {
            ViewBag.TotalPages = await _blog.getTotalPageCount();

            string GoogleId = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            Karter k = await _karter.getUserByGoogleId(GoogleId);
            List<BlogNotifications> notifications = await _notification.GetUserBlogNotifications(k.Id);
            await _notification.setAllBlogNotifsViewed(k.Id);
            ViewBag.Notifcount = notifications.Count;
            ViewBag.NotifiedTracks = await _notification.GetAllUsersUnseenPosts(k.Id);
            page = Math.Min(page, ViewBag.TotalPages);
            page = Math.Max(page, 1);
            ViewBag.page = page;


            List<BlogPost> allPosts = await _blog.GetAllPosts(page, getTaggedTrack: true, trackFilter: track);
            List<BlogPost> notifiedPosts = await _notification.GetAllUsersUnseenPosts(k.Id);
            await _notification.setAllBlogNotifsViewed(k.Id);
            BlogPage blogPage = new BlogPage
            {
                posts = await _blog.getModelToView(allPosts),
                notifiedPosts = await _blog.getModelToView(notifiedPosts)
            };

            return View(blogPage);
        }


        [HttpGet]
        [Authorize]
        [AccountConfirmed]
        public async Task<IActionResult> Create()
        {
            ViewBag.TrackTitles = await _tracks.GetAllTrackTitles();
            return View();
        }

        [HttpPost]
        [Authorize]
        [AccountConfirmed]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BlogPostView post)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Create");
            }

            string GoogleId = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            Karter k = await _karter.getUserByGoogleId(GoogleId);
            int postId;
            if (post.TaggedTrack != "")
            {
                Track taggedT = await _tracks.getSingleTrackByTitle(post.TaggedTrack);
                postId = await _blog.addPost(post, k, taggedT);
            }
            else
            {
                postId = await _blog.addPost(post, k);
            }
            int track = await _tracks.getTrackIdByTitle(post.TaggedTrack);

            List<int> kartersWhoNeedNotif = await _followerHandler.AllUserIdsWhoFollowTrack(track);
            if (post.TaggedTrack != "")
            {
                foreach (int kar in kartersWhoNeedNotif)
                {
                    await _notification.CreateBlogNotification(kar, postId);
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize]
        [AccountConfirmed]
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


        [HttpGet]
        [Authorize]
        [AccountConfirmed]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsForBlog(int blogId, int lastCommentId)
        {
            List<Comment> comments = await _blog.GetAllCommentsForPost(blogId, lastCommentId);
            return Ok(await _blog.CommentModelToView(comments));
        }

        [HttpGet]
        [Authorize]
        [AccountConfirmed]
        public ActionResult CreateComment(int blogId)
        {
            CommentView cv = new CommentView();
            cv.blogId = blogId;
            return PartialView("CreateComment", cv);
        }

        [HttpPost]
        [Authorize]
        [AccountConfirmed]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment(CommentView cv)
        {
            string GoogleId = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            Karter k = await _karter.getUserByGoogleId(GoogleId);

            Comment comment = new Comment
            {
                AuthorId = k.Id,
                BlogPostId = cv.blogId ?? 0,
                Text = cv.Text,
            };

            await _blog.CreateComment(comment);

            string previousUrl = Request.Headers["Referer"].ToString();

            return Redirect(previousUrl);
        }
    }
}
