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
        public required Locations Location { get; set; }

        // Relationships
        public ICollection<Karter>? Karters { get; set; }
    }

    public enum Locations
    {
        [Display(Name = "North")]
        NORTH,
        [Display(Name = "North-East")]
        NORTHEAST,
        [Display(Name = "East")]
        EAST,
        [Display(Name = "South-East")]
        SOUTHEAST,
        [Display(Name = "South")]
        SOUTH,
        [Display(Name = "South-West")]
        SOUTHWEST,
        [Display(Name = "West")]
        WEST,
        [Display(Name = "North-West")]
        NORTHWEST
    }

}
