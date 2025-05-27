using GoKartUnite.ViewModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoKartUnite.Models.Groups
{
    public class Membership
    {
        public virtual Karter User { get; set; }
        [ForeignKey("HostKarter")]
        public int KarterId { get; set; }
        public virtual Group Group { get; set; }
        public int GroupId { get; set; }
        public GroupMemberStatus MemberRole { get; set; }
    }
}
