using GoKartUnite.Data;
using Microsoft.EntityFrameworkCore;

namespace GoKartUnite.Handlers
{
    public class RelationshipHandler
    {
        private readonly GoKartUniteContext _context;
        public RelationshipHandler(GoKartUniteContext context)
        {
            _context = context;
        }

        public async Task<int> getFriends(int id)
        {
            int numFriends = await _context.Friendships.CountAsync(k => k.KarterFirstId == id || k.KarterSecondId == id);
            return numFriends;
        }
    }
}
