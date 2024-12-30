using GoKartUnite.Handlers;
using GoKartUnite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GoKartUnite.Controllers
{
    public class NotificationController : Controller
    {
        private readonly KarterHandler _karters;
        private readonly NotificationHandler _notifs;
        public NotificationController(KarterHandler karters, NotificationHandler notifs)
        {
            _karters = karters;
            _notifs = notifs;
        }

        public async Task<int> getNotifCount()
        {
            string GoogleId = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            Karter k = await _karters.getUserByGoogleId(GoogleId);
            List<Notifications> notifications = await _notifs.GetUsersNotifications(k.Id);
            return notifications.Count;
        }
    }
}
