using GoKartUnite.DataFilterOptions;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;

namespace GoKartUnite.Interfaces
{
    public interface IBlogHandler
    {
        Task<List<BlogPost>> GetAllPosts(BlogFilterOptions filterOptions);

        Task<int> AddPost(BlogPostView post, Karter author, Track taggedT);

        Task<int> AddPost(BlogPostView post, Karter author);

        Task<List<BlogPostView>> GetModelToView(List<BlogPost> posts);

        Task<BlogPost> GetPost(int Id, bool inclUpvotes = false, bool inclComments = false);

        Task UpvotePost(int Id, Upvotes upvoteToAdd);

        Task<int> GetTotalPageCount(int pageSize = 10);

        Task<List<Comment>> GetAllCommentsForPost(int blogPostId, int lastIdSent);

        Task<List<CommentView>> CommentModelToView(List<Comment> comments);

        Task CreateComment(Comment comment);

        Task<List<BlogPost>> GetPostsForTrack(string trackTitle, int count = 0);
    }
}
