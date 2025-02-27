using GoKartUnite.Models;

namespace GoKartUnite.DataFilterOptions
{
    public class KarterGetAllUsersFilter
    {
        public bool IncludeTrack { get; set; } = false;
        public bool IncludeNotifications { get; set; } = false;
        public bool IncludeUserRoles { get; set; } = false;
        public bool IncludeFriendships { get; set; } = false;
        public bool IncludeBlogPosts { get; set; } = false;
        public bool IncludeNotification { get; set; } = false;
        public string TrackToFetchFor { get; set; } = "";
        public int pageNo { get; set; } = 0;
        public int pageSize { get; set; } = 5;
        public SortKartersBy sort { get; set; } = Models.SortKartersBy.Alphabetically;

    }
}
