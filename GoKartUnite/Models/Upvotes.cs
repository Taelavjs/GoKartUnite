using System.ComponentModel.DataAnnotations.Schema;

namespace GoKartUnite.Models
{
    public class Upvotes
    {
        public int Id { get; set; }
        public int VoterId { get; set; }
        public int PostId { get; set; }
        [ForeignKey("VoterId")]
        public virtual Karter Karter { get; set; }
        [ForeignKey("PostId")]
        public virtual BlogPost Post { get; set; }
    }
}
