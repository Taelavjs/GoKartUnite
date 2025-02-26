using System.Data;

namespace GoKartUnite.Models
{
    public class UserRoles
    {
        public int Id { get; set; }
        public int KarterId { get; set; }
        public virtual Karter Karter { get; set; }

        public int RoleId { get; set; }
        public virtual Role Role { get; set; }

    }
}
