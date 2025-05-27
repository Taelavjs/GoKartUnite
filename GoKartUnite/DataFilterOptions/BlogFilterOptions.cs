namespace GoKartUnite.DataFilterOptions
{
    public class BlogFilterOptions
    {
        public int PageSize { get; set; } = 10;
        public int PageNo { get; set; } = 1;
        public bool SortByAscending { get; set; } = false;
        public bool SortByPopular { get; set; } = false;
        public bool GetAllTime { get; set; } = false;
        public int? UserIdFilter { get; set; }
        public string? TrackNameFilter { get; set; }
        public bool IncludeTrack { get; set; } = false;
        public bool IncludeUpvotes { get; set; } = false;
        public DateTime? AfterDateFilter { get; set; }
        public DateTime? PreDateFilter { get; set; }
        public bool IncludeAuthor { get; set; } = false;
    }
}
