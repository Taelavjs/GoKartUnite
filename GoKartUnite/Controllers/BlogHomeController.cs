using GoKartUnite.CustomAttributes;
using GoKartUnite.Handlers;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;
using GoKartUnite.DataFilterOptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using GoKartUnite.DataFilterOptions;
using System.Drawing.Printing;
using GoKartUnite.Interfaces;

namespace GoKartUnite.Controllers
{
    public class BlogHomeController : Controller
    {
        private readonly IBlogHandler _blog;
        private readonly IKarterHandler _karter;

        private readonly ITrackHandler _tracks;
        private readonly INotificationHandler _notification;
        private readonly IRoleHandler _roles;
        private readonly IFollowerHandler _followerHandler;
        public BlogHomeController(IRoleHandler roles, ITrackHandler tracks, IFollowerHandler followerHandler, IBlogHandler blog, IKarterHandler karter, INotificationHandler notification)
        {
            _blog = blog;
            _karter = karter;
            _notification = notification;
            _followerHandler = followerHandler;
            _tracks = tracks;
            _roles = roles;
        }
        [HttpGet]
        [Authorize]
        [AccountConfirmed]
        public async Task<IActionResult> Index(int page = 1, string? track = null)
        {

            ViewBag.TotalPages = await _blog.GetTotalPageCount();
            page = Math.Max(0, Math.Min(page, ViewBag.TotalPages));
            string GoogleId = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            Karter k = await _karter.GetUserByGoogleId(GoogleId);
            List<BlogNotifications> notifications = await _notification.GetUserBlogNotifications(k.Id);
            ViewBag.Notifcount = notifications.Count;
            ViewBag.NotifiedTracks = await _notification.GetAllUsersUnseenPosts(k.Id);
            page = Math.Min(page, ViewBag.TotalPages);
            page = Math.Max(page, 1);
            ViewBag.page = page;

            BlogFilterOptions blogFilter = new BlogFilterOptions
            {
                PageSize = 10,
                PageNo = page,
                TrackNameFilter = track,
                IncludeAuthor = true
            };

            List<BlogPost> allPosts = await _blog.GetAllPosts(blogFilter);
            List<BlogPost> notifiedPosts = await _notification.GetAllUsersUnseenPosts(k.Id);
            await _notification.SetAllBlogNotifsViewed(k.Id);
            BlogPage blogPage = new BlogPage
            {
                posts = await _blog.GetModelToView(allPosts),
                notifiedPosts = await _blog.GetModelToView(notifiedPosts)
            };

            return View(blogPage);
        }


        [HttpGet]
        [Authorize]
        [AccountConfirmed]
        public async Task<IActionResult> Create()
        {
            ViewBag.AllTracks = await _tracks.ModelToView(await _tracks.GetAllTracks());
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

            Karter k = await _karter.GetUserByGoogleId(GoogleId);
            int postId;
            if (post.TaggedTrackTitle != string.Empty)
            {
                Track taggedT = await _tracks.GetSingleTrackByTitle(post.TaggedTrackTitle);
                post.authorId = k.Id;
                post.Author = k;
                post.TaggedTrack = taggedT;
                postId = await _blog.AddPost(post);
            }
            else
            {
                post.authorId = k.Id;
                post.Author = k;
                postId = await _blog.AddPost(post);
                return RedirectToAction("Index");
            }

            List<int> kartersWhoNeedNotif = await _followerHandler.AllUserIdsWhoFollowTrack(post.TaggedTrack.Id);
            if (post.TaggedTrackTitle != string.Empty)
            {
                foreach (int kar in kartersWhoNeedNotif)
                {
                    await _notification.CreateBlogNotification(kar, postId);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        [AccountConfirmed]
        public async Task<IActionResult> CreateAdminPost(string trackTitle)
        {
            ViewBag.TrackTitle = trackTitle;
            return View();
        }

        [HttpPost]
        [Authorize]
        [AccountConfirmed]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdminPost(BlogPostView post)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            string GoogleId = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            Karter k = await _karter.GetUserByGoogleId(GoogleId);
            int trackAdminIds = await _roles.GetTrackUserTrackId(k.Id);
            Track taggedT = await _tracks.GetTrackById(trackAdminIds);
            post.blogType = BlogType.TrackNews;

            post.TaggedTrackTitle = taggedT.Title;
            post.TaggedTrack = taggedT;
            int postId = await _blog.AddPost(post);

            return RedirectToAction("Index", "BlogHome");
        }

        [HttpPost]
        [Authorize]
        [AccountConfirmed]
        public async Task UpvoteBlog(int id)
        {
            string GoogleId = User.Claims
    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            BlogPost post = await _blog.GetPost(id, inclUpvotes: true);
            Karter karter = await _karter.GetUserByGoogleId(GoogleId);

            bool alreadyUpvoted = post.Upvotes
                .Any(upvote => upvote.VoterId == karter.Id);

            if (alreadyUpvoted)
            {

                return;
            }
            var upvote = new Upvotes();
            upvote.PostId = id;
            upvote.VoterId = karter.Id;
            await _blog.UpvotePost(id, upvote);
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

            Karter k = await _karter.GetUserByGoogleId(GoogleId);

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
