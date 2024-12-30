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
                .OrderBy(i => i.DateTimePosted)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task addPost(BlogPostView post, Karter author, Track taggedT)
        {
            BlogPost dbPost = new BlogPost();

            dbPost.Author = author;
            dbPost.Title = post.Title;
            dbPost.AuthorId = author.Id;
            dbPost.Descripttion = post.Descripttion;
            dbPost.TaggedTrack = taggedT;


            await _context.BlogPosts.AddAsync(dbPost);

            await _context.SaveChangesAsync();

        }

        public async Task addPost(BlogPostView post, Karter author)
        {
            BlogPost dbPost = new BlogPost();

            dbPost.Author = author;
            dbPost.Title = post.Title;
            dbPost.AuthorId = author.Id;
            dbPost.Descripttion = post.Descripttion;
            await _context.BlogPosts.AddAsync(dbPost);

            await _context.SaveChangesAsync();

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
                retPosts.Add(post);
            }

            return retPosts;
        }

        public async Task<BlogPost> getPost(int Id, bool inclUpvotes = false)
        {
            if (inclUpvotes) return await _context.BlogPosts.Include(k => k.Upvotes).SingleOrDefaultAsync(k => k.Id == Id);
            return await _context.BlogPosts.SingleOrDefaultAsync(k => k.Id == Id);
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
    }
}
