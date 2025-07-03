namespace GoKartUnite.Models
{
    public class FriendStatusNotifications
    {
        public int Id { get; set; }
        public FriendUpdatedStatus status { get; set; }

        public string NotificationMessage = string.Empty;
        public int FriendId { get; set; }
        public Karter Friend { get; set; }
        public string FriendName { get; set; }
        public int UserId { get; set; }
        public Karter karter { get; set; }
        public FriendStatusNotifications(int friendId, string friendName, int userId, FriendUpdatedStatus status)
        {
            UserId = userId;
            FriendId = friendId;
            this.status = status;
            FriendName = friendName;

            switch (status)
            {
                case FriendUpdatedStatus.FriendToUser:
                    NotificationMessage = $"{friendName} has removed you as a friend";
                    break;
                case FriendUpdatedStatus.UserToRequested:
                    NotificationMessage = $"{friendName} has sent a friend request!";
                    break;
                case FriendUpdatedStatus.RequestedToDeclined:
                    NotificationMessage = $"{friendName} has declined your friend request";
                    break;
                case FriendUpdatedStatus.RequestedToWithdrawn:
                    NotificationMessage = $"{friendName} has rejected your friend request";
                    break;
            }
        }
    }

    public enum FriendUpdatedStatus
    {
        FriendToUser,
        RequestedToWithdrawn,
        RequestedToDeclined,
        UserToRequested
    }
}
