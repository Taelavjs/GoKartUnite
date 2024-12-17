using System.Reflection;

namespace GoKartUnite.Models
{
    public class BlogPost
    {

        public int Id { get; set; }

        public int AuthorId { get; set; }
        public Karter? Author { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Descripttion { get; set; } = string.Empty;
        private DateTime DateTimePosted { get; set; } = DateTime.UtcNow;


    }
}
