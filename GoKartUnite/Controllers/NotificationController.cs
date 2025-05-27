using GoKartUnite.Handlers;
using GoKartUnite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using GoKartUnite.CustomAttributes;
using GoKartUnite.Interfaces;

namespace GoKartUnite.Controllers
{
    [Authorize]
    [AccountConfirmed]
    public class NotificationController : Controller
    {
        private readonly IKarterHandler _karter;
        private readonly INotificationHandler _notifs;
        public NotificationController(IKarterHandler karters, INotificationHandler notifs)
        {
            _karter = karters;
            _notifs = notifs;
        }


        [HttpGet]
        public async Task<int> getNotifCount()
        {
            string GoogleId = await _karter.GetCurrentUserNameIdentifier(User);

            Karter k = await _karter.GetUserByGoogleId(GoogleId);
            List<BlogNotifications> notifications = await _notifs.GetUserBlogNotifications(k.Id);
            return notifications.Count;
        }
    }
}
