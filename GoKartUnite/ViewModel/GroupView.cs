namespace GoKartUnite.ViewModel
{
    public class GroupView
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? LeaderName { get; set; }

        public bool isMember { get; set; } = false;
        public bool isOwner { get; set; } = false;

        public int NumberMembers { get; set; } = 0;
        public DateTime DateCreated { get; set; }

        public List<KarterView> Karters { get; set; } = new List<KarterView>();
        public List<BlogPostView> posts { get; set; } = new List<BlogPostView>();

    }
}
