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

        public async Task CreateNotification(NotificationType type, int userId)
        {
            Notifications notif = new Notifications();
            notif.type = type;
            notif.userId = userId;

            await _context.Notifications.AddAsync(notif);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notifications>> GetUsersNotifications(int userId, bool isViewed = false)
        {
            if (isViewed)
            {
                List<Notifications> notifsAll = _context.Notifications.Where(n => n.userId == userId).ToList();
                return notifsAll;

            }


            List<Notifications> notifs = await _context.Notifications.Where(n => n.userId == userId && n.isViewed == false).ToListAsync();
            return notifs;

        }
    }
}
