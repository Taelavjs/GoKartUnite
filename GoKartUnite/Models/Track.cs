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
        public bool IsVerifiedByAdmin { get; set; } = false;
        public string FormattedGoogleLocation { get; set; } = string.Empty;
        public string GooglePlacesId { get; set; } = string.Empty;
        // Relationships
        public virtual ICollection<Karter> Karters { get; set; } = new List<Karter>();
        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
        public virtual ICollection<KarterTrackStats> KarterStats { get; set; } = new List<KarterTrackStats>();
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
