using System.ComponentModel.DataAnnotations;

namespace GoKartUnite.Models
{
    public class Track
    {
        public int Id { get; set; }
        [MinLength(3)]
        [Display(Name = "Track")]
        public required string Title { get; set; }
        public string Description { get; set; } = "";

        // Relationships
        public ICollection<Karter>? Karters { get; set; }
    }
}
