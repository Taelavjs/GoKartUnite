using System.ComponentModel.DataAnnotations.Schema;

namespace GoKartUnite.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        [ForeignKey("HostKarter")]
        public int HostId { get; set; }
        public virtual Karter HostKarter { get; set; }
        public virtual List<Karter> MemberKarters { get; set; } = new List<Karter>();
        public virtual List<BlogPost> GroupPosts { get; set; } = new List<BlogPost>();
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
