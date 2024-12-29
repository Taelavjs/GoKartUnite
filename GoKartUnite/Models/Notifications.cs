namespace GoKartUnite.Models
{
    public class Notifications
    {
        public int Id { get; set; }
        public NotificationType type { get; set; }
        public Karter? Author { get; set; }
        public int userId { get; set; }
        public bool isViewed { get; set; } = false;
        public DateTime? createdAt { get; set; } = DateTime.UtcNow;

    }


    public enum NotificationType : int
    {
        Any = 0,
        Blog = 1,
        Comment = 2,
        Upvote = 3,
        Track = 4
    }
}
