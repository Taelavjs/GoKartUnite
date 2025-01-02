using System.ComponentModel.DataAnnotations.Schema;

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
        [ForeignKey("BlogID")]
        public BlogPost? LinkedPost { get; set; }
    }
}
