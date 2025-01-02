namespace GoKartUnite.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public DateTime CreatedDate { get; set; }

        public int AuthorId { get; set; }

        public int BlogPostId { get; set; }
        public Karter? Author { get; set; }
        public BlogPost? BlogPost { get; set; }
    }
}
