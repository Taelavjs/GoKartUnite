﻿using GoKartUnite.CustomAttributes;
using GoKartUnite.Handlers;
using GoKartUnite.Models;
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
using GoKartUnite.ViewModel;

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
        public async Task<IActionResult> Index(int page = 1, string track = null, string filterBy = "Recent")
        {

            ViewBag.TotalPages = await _blog.GetTotalPageCount();
            page = Math.Max(0, Math.Min(page, ViewBag.TotalPages));
            string GoogleId = await _karter.GetCurrentUserNameIdentifier(User);

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
                IncludeAuthor = true,
                IncludeTrack = true,
                SortByPopular = filterBy == "Popular"
            };

            List<BlogPost> allPosts = await _blog.GetAllPosts(blogFilter);
            List<BlogPost> notifiedPosts = await _notification.GetAllUsersUnseenPosts(k.Id);
            await _notification.SetAllBlogNotifsViewed(k.Id);
            BlogPage blogPage = new BlogPage
            {
                posts = await _blog.GetModelToView(allPosts),
                notifiedPosts = await _blog.GetModelToView(notifiedPosts),
                SortedBy = filterBy,
                FilteredTrack = track
            };
            ViewBag.AllTracks = await _tracks.ModelToView(await _tracks.GetAllTracks());


            return View(blogPage);
        }


        [HttpGet]
        [Authorize]
        [AccountConfirmed]
        public async Task<IActionResult> Create(int id = -1)
        {
            ViewBag.AllTracks = await _tracks.ModelToView(await _tracks.GetAllTracks());
            if (id != -1)
            {
                BlogPostView retrievedPost = await _blog.GetModelToView(await _blog.GetPostById(id));

                return View(retrievedPost);
            }


            return View();
        }



        [HttpPost]
        [Authorize]
        [AccountConfirmed]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] JsonObjectForCreatingPosts post, int id = -1)
        {

            string GoogleId = await _karter.GetCurrentUserNameIdentifier(User);
            Karter k = await _karter.GetUserByGoogleId(GoogleId);

            if (id != -1)
            {
                // EDITING AN ALREADY EXISTING POST
                var postToSned = new BlogPostView
                {
                    Title = post.Title,
                    TaggedTrackTitle = post.TaggedTrackTitle,
                    Description = post.Description,
                };
                bool success = await _blog.UpdatePost(postToSned, id, k.Id);
                if(success)return Ok(new { status = "success", message = "Updated Blog post" });
                return BadRequest(new { status = "fail", message = "Invalid Model State" });
            }

            int postId;
            if (post.TaggedTrackTitle != string.Empty)
            {
                Track taggedTrack = await _tracks.GetSingleTrackByTitle(post.TaggedTrackTitle);
                var postToSned = new BlogPostView
                {
                    Title = post.Title,
                    TaggedTrackTitle = post.TaggedTrackTitle,
                    Description = post.Description,
                    Author = k,
                    authorId = k.Id,
                    TaggedTrack = taggedTrack,
                };
                postId = await _blog.AddPost(postToSned);
            }
            else
            {
                var postToSned = new BlogPostView
                {
                    Title = post.Title,
                    Description = post.Description,
                    Author = k,
                    authorId = k.Id,
                };
                return Ok(new { status = "success", message = "Created Blog Post" });
            }
            Track taggedT = await _tracks.GetSingleTrackByTitle(post.TaggedTrackTitle);

            List<int> kartersWhoNeedNotif = await _followerHandler.AllUserIdsWhoFollowTrack(taggedT.Id);
            if (post.TaggedTrackTitle != string.Empty)
            {
                foreach (int kar in kartersWhoNeedNotif)
                {
                    await _notification.CreateBlogNotification(kar, postId);
                }
            }
            return Ok(new { status = "success", message = "Created Blog Post With Tagged Track ANd Notifs" });
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

            string GoogleId = await _karter.GetCurrentUserNameIdentifier(User);
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
        public async Task<IActionResult> UpvoteBlog(int id)
        {
            string GoogleId = await _karter.GetCurrentUserNameIdentifier(User);
            BlogPost post = await _blog.GetPost(id, new BlogPostFilterOptions { IncludeUpvotes = true });
            Karter karter = await _karter.GetUserByGoogleId(GoogleId);

            Upvotes upvote = post.Upvotes
                .SingleOrDefault(upvote => upvote.VoterId == karter.Id);

            bool alreadyUpvoted = upvote != null;

            if (alreadyUpvoted)
            {
                await _blog.DeleteUpvote(upvote);
                return Ok(new { status = "success", message = -1 });
            }
            upvote = new Upvotes();
            upvote.PostId = id;
            upvote.VoterId = karter.Id;
            bool success = await _blog.UpvotePost(id, upvote);

            if (success) return Ok(new { status = "success", message = 1 });
            return BadRequest(new { status = "fail", message = "Upvote could not be applied" });
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
        public ActionResult CreateComment([FromBody] int blogId)
        {
            CommentView cv = new CommentView();
            cv.blogId = blogId;
            return PartialView("CreateComment", cv);
        }

        [HttpPost]
        [Authorize]
        [AccountConfirmed]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest comment)
        {
            string GoogleId = await _karter.GetCurrentUserNameIdentifier(User);
            Karter k = await _karter.GetUserByGoogleId(GoogleId);

            Comment commentModel = new Comment
            {
                AuthorId = k.Id,
                BlogPostId = comment.BlogId,
                Text = comment.Comment,
            };

            await _blog.CreateComment(commentModel);
            return Ok(new { status = "success", message = "Comment Created Successfully" });
        }



        public class CreateCommentRequest
        {
            public int BlogId { get; set; }
            public string Comment { get; set; }
        }
    }

    public class JsonObjectForCreatingPosts
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string TaggedTrackTitle { get; set; }
    }
}
