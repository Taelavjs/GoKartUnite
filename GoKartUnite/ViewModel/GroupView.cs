namespace GoKartUnite.ViewModel
{
    public class GroupView
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public string CreatorName { get; set; } = String.Empty;
        public int MemberCount { get; set; }
        public List<GroupMessageView> Comments { get; set; } = new List<GroupMessageView>();
    }
}
