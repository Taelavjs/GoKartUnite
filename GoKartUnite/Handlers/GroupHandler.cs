using GoKartUnite.Data;
using GoKartUnite.Interfaces;

namespace GoKartUnite.Handlers
{
    public class GroupHandler : IGroupHandler
    {
        private readonly GoKartUniteContext _context;

        public GroupHandler(GoKartUniteContext context)
        {
            _context = context;
        }


    }
}
