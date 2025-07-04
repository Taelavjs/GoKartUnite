using GoKartUnite.Models;

namespace GoKartUnite.Projection
{
    public class FriendStatusNotification
    {
        public int FromId { get; set; }
        public string NotificationMessage { get; set; } = string.Empty;
        public FriendUpdatedStatus Status { get; set; }
        public bool isViewed { get; set; } = false;
        public DateTime DateCreated { get; set; }

    }
}
