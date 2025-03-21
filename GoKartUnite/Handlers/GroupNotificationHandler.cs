using GoKartUnite.Data;
using Microsoft.Owin.Security;

namespace GoKartUnite.Handlers
{
    public class GroupNotificationHandler
    {
        private readonly GoKartUniteContext _context;

        public GroupNotificationHandler(GoKartUniteContext context)
        {
            _context = context;
        }


        public async Task<bool> NewStatCreatedNotifcation()
        {


            return true;
        }
    }
}
