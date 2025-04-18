using GoKartUnite.Models;

namespace GoKartUnite.ViewModel
{
    public class KarterAdminView
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? NameIdentifier { get; set; }

        public string Name { get; set; }
        public int YearsExperience { get; set; }

        public int? TrackId { get; set; }
        public Track? Track { get; set; }

        public virtual int UserRoles { get; set; }

        public virtual int Friendships { get; set; }
        public virtual int BlogPosts { get; set; }
        public int Notification { get; set; }
        public int Stats { get; set; }
    }
}
