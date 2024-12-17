using GoKartUnite.Data;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Web.Mvc;

namespace GoKartUnite.Handlers
{
    public class BlogHandler
    {
        private readonly GoKartUniteContext _context;
        public BlogHandler(GoKartUniteContext context)
        {
            _context = context;
        }

        public async Task<List<BlogPost>> getAllPosts()
        {
            List<BlogPost> posts = await _context.BlogPosts.ToListAsync();

            return posts;
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
            foreach(BlogPost bp in posts)
            {
                BlogPostView post = new BlogPostView();

                post.Title = bp.Title;
                post.Descripttion = bp.Descripttion;
                Karter Author = await _context.Karter.SingleOrDefaultAsync(k => k.Id == bp.AuthorId);
                post.Author = Author.Name;
                retPosts.Add(post);
            }

            return retPosts;
        }

    }
}
