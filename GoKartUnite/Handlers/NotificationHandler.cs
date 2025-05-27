using GoKartUnite.Data;
using GoKartUnite.Interfaces;
using GoKartUnite.Models;
using Microsoft.EntityFrameworkCore;

namespace GoKartUnite.Handlers
{
    public class NotificationHandler : INotificationHandler
    {
        private readonly GoKartUniteContext _context;
        public NotificationHandler(GoKartUniteContext context)
        {
            _context = context;
        }

        public async Task CreateBlogNotification(int userId, int postId)
        {
            BlogNotifications notif = new BlogNotifications();
            notif.userId = userId;
            notif.BlogID = postId;

            await _context.BlogNotifications.AddAsync(notif);
            await _context.SaveChangesAsync();
        }

        public async Task<List<BlogNotifications>> GetUserBlogNotifications(int userId, bool isViewed = false)
        {
            if (isViewed)
            {
                List<BlogNotifications> notifsAll = _context.BlogNotifications.Where(n => n.userId == userId).ToList();
                return notifsAll;

            }


            List<BlogNotifications> notifs = await _context.BlogNotifications.Where(n => n.userId == userId && n.isViewed == false).ToListAsync();
            return notifs;

        }

        public async Task SetAllBlogNotifsViewed(int userId)
        {
            List<BlogNotifications> allBlogNotifs = await GetUserBlogNotifications(userId);

            foreach (BlogNotifications notif in allBlogNotifs)
            {
                notif.isViewed = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<BlogPost>> GetAllUsersUnseenPosts(int userId)
        {
            return await _context.BlogNotifications
                .Include(t => t.LinkedPost)
                .Include(t => t.Author)
                .Where(t => t.userId == userId && t.isViewed == false)
                .Select(t => t.LinkedPost)
                .OrderByDescending(k => k.DateTimePosted)
                .ToListAsync() ?? new List<BlogPost>();
        }

    }
}
