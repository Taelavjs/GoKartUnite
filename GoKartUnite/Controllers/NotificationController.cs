using GoKartUnite.Handlers;
using GoKartUnite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using GoKartUnite.CustomAttributes;
using GoKartUnite.Interfaces;

namespace GoKartUnite.Controllers
{
    public class NotificationController : Controller
    {
        private readonly IKarterHandler _karters;
        private readonly INotificationHandler _notifs;
        public NotificationController(IKarterHandler karters, INotificationHandler notifs)
        {
            _karters = karters;
            _notifs = notifs;
        }


        [HttpGet]
        [Authorize]
        [AccountConfirmed]
        public async Task<int> getNotifCount()
        {
            string GoogleId = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            Karter k = await _karters.GetUserByGoogleId(GoogleId);
            List<BlogNotifications> notifications = await _notifs.GetUserBlogNotifications(k.Id);
            return notifications.Count;
        }
    }
}
