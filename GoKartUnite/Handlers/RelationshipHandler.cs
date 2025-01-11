using GoKartUnite.Data;
using GoKartUnite.Models;
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

        public async Task<int> getFriendsCount(int id)
        {
            int numFriends = await _context.Friendships.CountAsync(k => k.KarterFirstId == id || k.KarterSecondId == id);
            return numFriends;
        }

        public async Task<List<Karter>> getAllFriends(int id)
        {
            List<Friendships> friends = _context.Friendships.Include(k => k.KarterFirst).Include(k => k.KarterSecond).Where(k => k.KarterFirstId == id || k.KarterSecondId == id).ToList();
            List<Karter> result = new List<Karter>();

            foreach (var friend in friends)
            {
                if (friend.KarterFirstId == id)
                {
                    result.Add(friend.KarterSecond);
                    continue;
                }
                result.Add(friend.KarterFirst);

            }
            return result;
        }
    }
}
