namespace GoKartUnite.Models
{
    public class Upvotes
    {
        public int Id { get; set; }
        public int VoterId { get; set; }
        public int PostId { get; set; }

        public Karter? Karter { get; set; }
        public BlogPost? Post { get; set; }
    }
}
