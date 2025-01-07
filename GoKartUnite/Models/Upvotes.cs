namespace GoKartUnite.Models
{
    public class Upvotes
    {
        public int Id { get; set; }
        public int VoterId { get; set; }
        public int PostId { get; set; }

        public virtual Karter? Karter { get; set; }
        public virtual BlogPost? Post { get; set; }
    }
}
