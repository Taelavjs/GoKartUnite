namespace GoKartUnite.Models
{
    public class BlogNotifications
    {
        public int Id { get; set; }
        public Karter? Author { get; set; }
        public int userId { get; set; }
        public bool isViewed { get; set; } = false;
        public DateTime? createdAt { get; set; } = DateTime.UtcNow;
        public int BlogID { get; set; }

        public BlogPost? LinkedPost { get; set; }
    }
}
