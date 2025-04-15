using GoKartUnite.Models;

namespace GoKartUnite.ViewModel
{
    public class KarterProfilePreview
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string YearsExperience { get; set; }
        public string LocalTrack { get; set; }
        public FriendshipStatus isFriend { get; set; }
    }
}
