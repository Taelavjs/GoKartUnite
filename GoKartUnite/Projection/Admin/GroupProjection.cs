namespace GoKartUnite.Projection.Admin
{
    public class GroupProjection
    {
        public int Id { get; set; }
        public int MemberCount { get; set; }
        public AdminGroupMember Creator { get; set; }
        public List<AdminGroupMember> Members { get; set; }
        public List<AdminGroupMessage> Messages { get; set; }
    }

    public class AdminGroupMember
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AdminGroupMessage
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string MessageContent { get; set; }
        public DateTime DateSent { get; set; }
    }


}
