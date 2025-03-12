using GoKartUnite.Models;

namespace GoKartUnite.ViewModel
{
    public class BlogPage
    {
        public List<BlogPostView> posts = new List<BlogPostView>();
        public List<BlogPostView> notifiedPosts = new List<BlogPostView>();
        public BlogPostView editedPost = new BlogPostView();

        public string FilteredTrack { get; set; }
        public string SortedBy { get; set; }

    }
}
