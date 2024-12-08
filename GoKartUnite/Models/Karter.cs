using System.ComponentModel.DataAnnotations;

namespace GoKartUnite.Models
{
    public class Karter
    {
        public int Id { get; set; }
        [Display(Name = "Karter")]
        [MinLength(3)]
        [StringLength(15)]
        public required string Name { get; set; }
        [Range(0, 20)]
        public required int YearsExperience { get; set; }

        // Relationships
        [Display(Name = "Local Track Id")]
        public int? TrackId { get; set; }
        public Track? Track { get; set; }
        
    }
}
