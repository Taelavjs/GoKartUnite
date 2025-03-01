using GoKartUnite.DataFilterOptions;
using GoKartUnite.Models;

namespace GoKartUnite.Projection
{
    public class KarterProjection
    {
        public string Name { get; set; }
        public int YearsExperience { get; set; }
        public string TrackTitle { get; set; }
        public BlogNotifications Notification { get; set; }
        public ICollection<BlogPost> BlogPosts { get; set; }
        public ICollection<Friendships> Friendships { get; set; }
        public ICollection<UserRoles> UserRoles { get; set; }
    }
}
