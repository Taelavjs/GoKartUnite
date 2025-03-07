using System.ComponentModel.DataAnnotations.Schema;

namespace GoKartUnite.Models
{
    public class Upvotes
    {
        public int Id { get; set; }
        [ForeignKey("Karter")]
        public int VoterId { get; set; }
        [ForeignKey("Post")]
        public int PostId { get; set; }

        public virtual Karter? Karter { get; set; }
        public virtual BlogPost? Post { get; set; }
    }
}
