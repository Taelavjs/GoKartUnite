namespace GoKartUnite.ViewModel
{
    public class ListedGroupView
    {
        public int? Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public string? LeaderName { get; set; }

        public bool isMember { get; set; } = false;
        public bool isOwner { get; set; } = false;
        public int NumberMembers { get; set; } = 0;
        public DateTime DateCreated { get; set; }

        public List<KarterView> Karters { get; set; } = new List<KarterView>();
        public List<BlogPostView> posts { get; set; } = new List<BlogPostView>();
    }

    public enum GroupMemberStatus
    {
        OWNER,
        MEMBER,
        NONE
    }
}