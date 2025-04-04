using GoKartUnite.DataFilterOptions;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;

namespace GoKartUnite.Interfaces
{
    public interface IBlogHandler
    {
        Task<List<BlogPost>> GetAllPosts(BlogFilterOptions filterOptions);

        Task<int> AddPost(BlogPostView post);

        Task<List<BlogPostView>> GetModelToView(List<BlogPost> posts);
        Task<BlogPostView> GetModelToView(BlogPost posts);

        Task<BlogPost> GetPost(int Id, BlogPostFilterOptions? options = null);

        Task<bool> UpvotePost(int Id, Upvotes upvoteToAdd);
        Task DeleteUpvote(Upvotes upvoteToDelete);


        Task<int> GetTotalPageCount(int pageSize = 10);

        Task<List<Comment>> GetAllCommentsForPost(int blogPostId, int lastIdSent);

        Task<List<CommentView>> CommentModelToView(List<Comment> comments);

        Task CreateComment(Comment comment);

        Task<BlogPost> GetPostById(int id);

        Task<List<BlogPost>> GetPostsForTrack(string trackTitle, int count = 0);

        Task<bool> UpdatePost(BlogPostView post, int id, int karterId);
    }
}
