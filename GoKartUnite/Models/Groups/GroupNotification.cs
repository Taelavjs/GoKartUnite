namespace GoKartUnite.Models.Groups
{
    public class GroupNotification
    {
        public int Id { get; set; }
        public int StatId { get; set; }
        public required virtual KarterTrackStats Stat { get; set; }

        public DateTime Created { get; set; }
    }
}
