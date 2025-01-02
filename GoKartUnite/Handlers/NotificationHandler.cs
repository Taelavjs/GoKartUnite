using GoKartUnite.Data;
using GoKartUnite.Models;
using Microsoft.EntityFrameworkCore;

namespace GoKartUnite.Handlers
{
    public class NotificationHandler
    {
        private readonly GoKartUniteContext _context;
        public NotificationHandler(GoKartUniteContext context)
        {
            _context = context;
        }

        public async Task CreateNotification(int userId)
        {
            BlogNotifications notif = new BlogNotifications();
            notif.userId = userId;

            await _context.BlogNotifications.AddAsync(notif);
            await _context.SaveChangesAsync();
        }

        public async Task<List<BlogNotifications>> GetUsersNotifications(int userId, bool isViewed = false)
        {
            if (isViewed)
            {
                List<BlogNotifications> notifsAll = _context.BlogNotifications.Where(n => n.userId == userId).ToList();
                return notifsAll;

            }


            List<BlogNotifications> notifs = await _context.BlogNotifications.Where(n => n.userId == userId && n.isViewed == false).ToListAsync();
            return notifs;

        }

        public async Task<List<BlogNotifications>> getBlogNotifs(int userId)
        {
            return await _context.BlogNotifications.Where(n => n.userId == userId && n.isViewed == false).ToListAsync();
        }

        public async Task setAllBlogNotifsViewed(int userId)
        {
            List<BlogNotifications> allBlogNotifs = await getBlogNotifs(userId);

            foreach (BlogNotifications notif in allBlogNotifs)
            {
                notif.isViewed = true;
            }

            await _context.SaveChangesAsync();
        }

    }
}
