using GoKartUnite.Models;
using System.ComponentModel.DataAnnotations;

namespace GoKartUnite.ViewModel
{


    public class KarterView
    {
        public int Id { get; set; }
        [Display(Name = "Karter")]
        [MinLength(3)]
        [StringLength(15)]
        public string Name { get; set; }
        [Range(0, 20)]
        public int YearsExperience { get; set; }
        [Display(Name = "Local Track Id")]
        public Track? LocalTrack { get; set; }
        public int TrackId { get; set; }

        public FriendshipStatus FriendStatus { get; set; } = FriendshipStatus.User;
    }



}
