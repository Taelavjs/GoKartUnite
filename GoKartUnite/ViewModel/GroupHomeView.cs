using GoKartUnite.Projection;

namespace GoKartUnite.ViewModel
{
    public class GroupHomeView
    {
        public List<BlogPostView> Posts { get; set; } = new List<BlogPostView>();
        public GroupView Group { get; set; }
        public List<GroupMember> Members { get; set; } = new List<GroupMember>();
    }
}
