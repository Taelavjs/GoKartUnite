using GoKartUnite.Data;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Drawing.Printing;
using System.Web.Mvc;
using X.PagedList;

namespace GoKartUnite.Handlers
{
    public class BlogHandler
    {
        private readonly GoKartUniteContext _context;
        public BlogHandler(GoKartUniteContext context)
        {
            _context = context;
        }

        public async Task<List<BlogPost>> GetAllPosts(int page = 1, int pageSize = 10, bool getTaggedTrack = false, string? trackFilter = null)
        {
            IQueryable<BlogPost> query = _context.BlogPosts.Include(k => k.Upvotes).AsQueryable();

            if (getTaggedTrack)
            {
                query = query.Include(k => k.TaggedTrack);

                if (!string.IsNullOrEmpty(trackFilter))
                {
                    query = query.Where(k => k.TaggedTrack != null && k.TaggedTrack.Title == trackFilter);
                }
            }

            return await query
                .OrderByDescending(i => i.DateTimePosted)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<BlogPost>> GetAllPostsFromUser(int Id, int pageNo = 0)
        {
            IQueryable<BlogPost> postsQuery = _context.BlogPosts
                .Where(k => k.AuthorId == Id)
                .OrderByDescending(i => i.DateTimePosted)
                .Skip(pageNo * 10)
                .Take(10)
                .Include(k => k.Upvotes)
                .Include(k => k.TaggedTrack);


            return postsQuery.ToList();
        }

        public async Task<int> addPost(BlogPostView post, Karter author, Track taggedT)
        {
            BlogPost dbPost = new BlogPost();

            dbPost.Author = author;
            dbPost.Title = post.Title;
            dbPost.AuthorId = author.Id;
            dbPost.Descripttion = post.Descripttion;
            dbPost.TaggedTrack = taggedT;


            await _context.BlogPosts.AddAsync(dbPost);

            await _context.SaveChangesAsync();
            return dbPost.Id;
        }

        public async Task<int> addPost(BlogPostView post, Karter author)
        {
            BlogPost dbPost = new BlogPost();

            dbPost.Author = author;
            dbPost.Title = post.Title;
            dbPost.AuthorId = author.Id;
            dbPost.Descripttion = post.Descripttion;
            await _context.BlogPosts.AddAsync(dbPost);

            await _context.SaveChangesAsync();
            return dbPost.Id;
        }
        public async Task<List<BlogPostView>> getModelToView(List<BlogPost> posts)
        {
            List<BlogPostView> retPosts = new List<BlogPostView>();
            foreach (BlogPost bp in posts)
            {
                BlogPostView post = new BlogPostView();
                post.Id = bp.Id;
                post.Title = bp.Title;
                post.Descripttion = bp.Descripttion;
                Karter Author = await _context.Karter.SingleOrDefaultAsync(k => k.Id == bp.AuthorId);
                post.Author = Author.Name;
                post.Upvotes = bp.Upvotes.Count;
                post.TaggedTrack = bp.TaggedTrack?.Title;
                post.authorId = bp.AuthorId;
                retPosts.Add(post);
            }

            return retPosts;
        }

        public async Task<BlogPost> getPost(int Id, bool inclUpvotes = false, bool inclComments = false)
        {
            IQueryable<BlogPost> query = _context.BlogPosts.AsQueryable();
            if (inclUpvotes)
            {
                query = query.Include(k => k.Upvotes);
            }
            if (inclComments)
            {
                query = query.Include(k => k.Comments);
            }
            return await query.SingleOrDefaultAsync(k => k.Id == Id);
        }

        public async Task upvotePost(int Id, Upvotes upvoteToAdd)
        {
            BlogPost post = await getPost(Id);
            post.Upvotes.Add(upvoteToAdd);

            await _context.SaveChangesAsync();
        }

        public async Task<int> getTotalPageCount(int pageSize = 10)
        {
            int totalCount = await _context.BlogPosts.CountAsync();
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            return totalPages;
        }


        public async Task<List<BlogPost>> AllBlogsAtTrackAfterDate(string trackTitle, DateTime date)
        {
            return await _context.BlogPosts.Include(X => X.TaggedTrack)
                            .Where(x => x.TaggedTrack != null && x.DateTimePosted > date && x.TaggedTrack.Title == trackTitle)
                            .OrderByDescending(x => x.DateTimePosted)
                            .ToListAsync();

        }

        public async Task<List<Comment>> GetAllCommentsForPost(int blogPostId, int lastIdSent)
        {
            BlogPost post = await getPost(blogPostId, false, true);

            if (lastIdSent == 0)
            {
                return post.Comments.Take(10).ToList();
            }
            return post.Comments.SkipWhile(t => t.Id != lastIdSent).Take(10).ToList();
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
                    AuthorName = "Taeka",
                    TypedAt = comment.CreatedDate
                };

                viewComments.Add(commentToAdd);
            }


            return viewComments;
        }


        public async Task CreateComment(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<BlogPost>> getPostsForTrack(string trackTitle, int count = 0)
        {
            List<BlogPost> posts = _context.BlogPosts
                .Where(t => t.TaggedTrack != null && t.TaggedTrack.Title == trackTitle)
                .OrderByDescending(x => x.DateTimePosted)
                .Take(count)
                .Include(t => t.Upvotes)
                .ToList();

            return posts;
        }
    }
}
