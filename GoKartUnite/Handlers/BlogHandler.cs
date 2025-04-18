﻿using GoKartUnite.Data;
using GoKartUnite.DataFilterOptions;
using GoKartUnite.Interfaces;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Drawing.Printing;
using System.Web.Mvc;
using X.PagedList;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GoKartUnite.Handlers
{
    public class BlogHandler : IBlogHandler
    {
        private readonly GoKartUniteContext _context;
        private readonly IKarterHandler _karter;
        public BlogHandler(GoKartUniteContext context, IKarterHandler karter)
        {
            _karter = karter;
            _context = context;
        }

        public async Task<List<BlogPost>> GetAllPosts(BlogFilterOptions filterOptions)
        {
            IQueryable<BlogPost> query = _context.BlogPosts.Include(k => k.Upvotes).AsQueryable();

            if (!string.IsNullOrEmpty(filterOptions.TrackNameFilter))
            {
                query = query.Include(k => k.TaggedTrack)
                             .Where(k => k.TaggedTrack != null && k.TaggedTrack.Title == filterOptions.TrackNameFilter);
            }

            if (filterOptions.IncludeTrack)
            {
                query = query.Include(k => k.TaggedTrack);
            }

            if (filterOptions.IncludeUpvotes)
            {
                query = query.Include(k => k.Upvotes);
            }

            if (filterOptions.IncludeAuthor)
            {
                query = query.Include(k => k.Karter);
            }


            if (filterOptions.UserIdFilter != null)
            {
                query = query.Where(x => x.KarterId == filterOptions.UserIdFilter);
            }

            if (filterOptions.SortByAscending)
            {
                query = query.OrderBy(i => i.DateTimePosted);
            }
            else
            {
                query = query.OrderByDescending(i => i.DateTimePosted);
            }

            if (filterOptions.SortByPopular)
            {
                query = query.OrderByDescending(i => i.Upvotes.Count);
            }

            if (filterOptions.PreDateFilter != null)
            {
                query = query.Where(x => x.DateTimePosted < filterOptions.PreDateFilter);
            }

            if (filterOptions.AfterDateFilter != null)
            {
                query = query.Where(x => x.DateTimePosted > filterOptions.AfterDateFilter);
            }

            return await query
                .Skip((filterOptions.PageNo - 1) * filterOptions.PageSize)
                .Take(filterOptions.PageSize)
                .ToListAsync();
        }


        public async Task<int> AddPost(BlogPostView post)
        {
            if (post.Author == null || post.Author.Id < 0)
            {
                return -1;
            }

            BlogPost dbPost = new BlogPost();

            dbPost.Karter = post.Author;
            dbPost.Title = post.Title;
            dbPost.KarterId = post.Author.Id;
            dbPost.Description = post.Description;
            dbPost.TaggedTrack = post.TaggedTrack ?? null;


            await _context.BlogPosts.AddAsync(dbPost);

            await _context.SaveChangesAsync();
            return dbPost.Id;
        }
        public async Task<List<BlogPostView>> GetModelToView(List<BlogPost> posts)
        {
            if (posts == null || posts.Count == 0) return new List<BlogPostView>();

            List<BlogPostView> retPosts = new List<BlogPostView>();
            foreach (BlogPost bp in posts)
            {
                BlogPostView post = new BlogPostView();
                post.Id = bp.Id;
                post.Title = bp.Title;
                post.Description = bp.Description;
                post.Author = bp.Karter;
                post.Upvotes = bp.Upvotes.Count;
                post.TaggedTrack = bp.TaggedTrack;
                post.authorId = bp.KarterId;
                retPosts.Add(post);
            }

            return retPosts;
        }

        public async Task<BlogPostView> GetModelToView(BlogPost posts)
        {
            if (posts == null) return null;

            BlogPostView post = new BlogPostView();
            post.Id = posts.Id;
            post.Title = posts.Title;
            post.Description = posts.Description;
            post.Author = posts.Karter;
            post.Upvotes = posts.Upvotes.Count;
            post.TaggedTrack = posts.TaggedTrack;
            post.authorId = posts.KarterId;

            return post;
        }

        public async Task<BlogPost> GetPost(int Id, BlogPostFilterOptions? options = null)
        {
            if (options == null) options = new BlogPostFilterOptions();

            IQueryable<BlogPost> query = _context.BlogPosts.AsQueryable();
            if (options.IncludeComments)
            {
                query = query.Include(k => k.Comments);
            }
            if (options.IncludeUpvotes)
            {
                query = query.Include(k => k.Upvotes);
            }
            if (options.IncludeAuthor)
            {
                query = query.Include(x => x.Comments).ThenInclude(x => x.Author);
            }
            return await query.SingleOrDefaultAsync(k => k.Id == Id);
        }

        public async Task<bool> UpvotePost(int Id, Upvotes upvoteToAdd)
        {
            try
            {
                BlogPost post = await GetPost(Id);
                post.Upvotes.Add(upvoteToAdd);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public async Task DeleteUpvote(Upvotes upvoteToDelete)
        {
            _context.Remove(upvoteToDelete);

            _context.SaveChanges();
        }

        public async Task<int> GetTotalPageCount(int pageSize = 10)
        {
            int totalCount = await _context.BlogPosts.CountAsync();
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            return totalPages;
        }

        public async Task<List<Comment>> GetAllCommentsForPostAfterId(int blogPostId, int lastIdSent)
        {
            BlogPost post = await GetPost(blogPostId, new BlogPostFilterOptions { IncludeComments = true, IncludeUpvotes = true, IncludeAuthor = true });
            if (post == null) return new List<Comment>();
            if (lastIdSent == 0)
            {
                return post.Comments.Take(10).ToList();
            }
            return post.Comments
                .SkipWhile(t => t.Id != lastIdSent)
                .Skip(1)
                .Take(10)
                .ToList();
        }

        public async Task<List<CommentView>> CommentModelToView(List<Comment> comments)
        {
            List<CommentView> viewComments = new List<CommentView>();

            foreach (Comment comment in comments)
            {
                CommentView commentToAdd = new CommentView
                {
                    Id = comment.Id,
                    Text = comment.Text,
                    AuthorName = comment.Author.Name,
                    TypedAt = comment.CreatedDate
                };

                viewComments.Add(commentToAdd);
            }


            return viewComments;
        }

        public async Task CreateComment(Comment comment)
        {
            comment.Author = _context.Karter.SingleOrDefault(x => x.Id == comment.AuthorId);

            if (comment.Author == null)
            {
                return;
            }

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<BlogPost>> GetPostsForTrack(string trackTitle, int count = 0)
        {
            List<BlogPost> posts = _context.BlogPosts
                .Where(t => t.TaggedTrack != null && t.TaggedTrack.Title == trackTitle)
                .OrderByDescending(x => x.DateTimePosted)
                .Take(count)
                .Include(t => t.Upvotes)
                .ToList();

            return posts;
        }

        public async Task<BlogPost> GetPostById(int id)
        {
            BlogPost post = _context.BlogPosts.SingleOrDefault(t => t.Id == id);

            return post;
        }

        public async Task<bool> UpdatePost(BlogPostView post, int id, int karterId)
        {
            BlogPost retrievedPost = _context.BlogPosts.SingleOrDefault(t => t.Id == id);

            if (retrievedPost == null) return false;
            if (retrievedPost.KarterId != karterId) return false;
            retrievedPost.Title = post.Title;
            retrievedPost.Description = post.Description;

            Track track = _context.Track.SingleOrDefault(t => t.Title == post.TaggedTrackTitle);
            if (track == null) return false;

            retrievedPost.TaggedTrack = track;
            _context.SaveChanges();
            return true;
        }

    }
}
