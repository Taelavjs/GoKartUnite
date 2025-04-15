using GoKartUnite.Models;

namespace GoKartUnite.ViewModel
{
    public class KarterIndex
    {
        public Karter karter { get; set; }
        public List<string> trackTitles { get; set; } = new List<string>();
        public List<KarterView> karterFriends { get; set; } = new List<KarterView>();
        public List<KarterView> karterFriendRequests { get; set; } = new List<KarterView>();
        public List<KarterView> sentFriendRequests { get; set; } = new List<KarterView>();
        public List<string> tracks { get; set; } = new List<string>();
        public bool isUser = false;

    }
}
