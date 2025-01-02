namespace GoKartUnite.ViewModel
{
    public class CommentView
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string AuthorName { get; set; }
        public DateTime TypedAt { get; set; }
        public int? blogId { get; set; }

    }
}
