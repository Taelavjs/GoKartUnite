using GoKartUnite.Handlers;
using GoKartUnite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using GoKartUnite.CustomAttributes;

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


        [HttpGet]
        [Authorize]
        [AccountConfirmed]
        public async Task<int> getNotifCount()
        {
            string GoogleId = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            Karter k = await _karters.getUserByGoogleId(GoogleId);
            List<BlogNotifications> notifications = await _notifs.GetUserBlogNotifications(k.Id);
            return notifications.Count;
        }
    }
}
