using GoKartUnite.Models;

namespace GoKartUnite.ViewModel
{
    public class KarterIndex
    {
        public Karter karter { get; set; }
        public List<KarterView> karterFriends { get; set; } = new List<KarterView>();
        public List<KarterView> karterFriendRequests { get; set; } = new List<KarterView>();
        public List<KarterView> sentFriendRequests { get; set; } = new List<KarterView>();
    }
}
