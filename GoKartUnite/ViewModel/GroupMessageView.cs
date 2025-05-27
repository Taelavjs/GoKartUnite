namespace GoKartUnite.ViewModel
{
    public class GroupMessageView
    {
        public int Id { get; set; }
        public string AuthorName { get; set; } = String.Empty;
        public string MessageContent { get; set; } = String.Empty;
        public DateTime TimeSent { get; set; }
    }
}
