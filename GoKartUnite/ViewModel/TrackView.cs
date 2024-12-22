using Microsoft.CodeAnalysis;

namespace GoKartUnite.ViewModel
{
    public class TrackView
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Location location { get; set; }
        public int karters = 0;
        public int blogPosts = 0;
    }
}
