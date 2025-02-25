using GoKartUnite.Models;

namespace GoKartUnite.Interfaces
{
    public interface INotificationHandler
    {
        Task CreateBlogNotification(int userId, int postId);
        Task<List<BlogNotifications>> GetUserBlogNotifications(int userId, bool isViewed = false);
        Task SetAllBlogNotifsViewed(int userId);
        Task<List<BlogPost>> GetAllUsersUnseenPosts(int userId);
    }
}
