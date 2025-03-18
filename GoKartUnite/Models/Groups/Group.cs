using System.ComponentModel.DataAnnotations.Schema;

namespace GoKartUnite.Models.Groups
{
    public class Group
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public int HostId { get; set; }
        public virtual Karter HostKarter { get; set; }
        public virtual List<Membership> MemberKarters { get; set; } = new List<Membership>();
        public virtual List<GroupMessage> GroupMessages { get; set; } = new List<GroupMessage>();
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
