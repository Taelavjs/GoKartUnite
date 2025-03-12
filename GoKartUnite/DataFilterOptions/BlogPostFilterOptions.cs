namespace GoKartUnite.DataFilterOptions
{
    public class BlogPostFilterOptions
    {
        public BlogPostFilterOptions() { }

        public bool IncludeComments { get; set; } = false;
        public bool IncludeUpvotes { get; set; } = false;
        public bool IncludeAuthor { get; set; } = false;
    }
}
