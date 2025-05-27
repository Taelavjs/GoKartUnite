using System.ComponentModel.DataAnnotations.Schema;

namespace GoKartUnite.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;


        public int AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public virtual Karter Author { get; set; }
        public int BlogPostId { get; set; }
        [ForeignKey("BlogPostId")]
        public virtual BlogPost BlogPost { get; set; }
    }
}
