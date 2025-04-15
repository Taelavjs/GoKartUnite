using GoKartUnite.Data;
using GoKartUnite.Interfaces;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;

namespace GoKartUnite.Handlers
{
    public class RelationshipHandler : IRelationshipHandler
    {
        private readonly GoKartUniteContext _context;
        public RelationshipHandler(GoKartUniteContext context)
        {
            _context = context;
        }

        public async Task<int> GetFriendsCount(int id)
        {
            int numFriends = await _context.Friendships.CountAsync(k => k.KarterFirstId == id || k.KarterSecondId == id);
            return numFriends;
        }

        public async Task<List<Karter>> GetAllFriends(int id)
        {
            List<Friendships> friends = _context.Friendships
                .Include(k => k.KarterFirst)
                .Include(k => k.KarterSecond)
                .Where(k => (k.KarterFirstId == id || k.KarterSecondId == id) && k.accepted)
                .ToList();
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

        public async Task<List<Karter>> GetAllFriendRequests(int id)
        {
            List<Friendships> friends = _context.Friendships
                .Include(k => k.KarterFirst.Track)
                .Include(k => k.KarterSecond.Track)
                .Where(k => (k.KarterFirstId == id || k.KarterSecondId == id) && id != k.requestedByInt && !k.accepted)
                .ToList();
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

        public async Task<List<Karter>> GetAllSentRequests(int id)
        {

            List<Friendships> friends = _context.Friendships
                .Include(k => k.KarterFirst.Track)
                .Include(k => k.KarterSecond.Track)
                .Where(k => (k.KarterFirstId == id || k.KarterSecondId == id) && id == k.requestedByInt && !k.accepted)
                .ToList();
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

        public async Task<bool> AcceptFriendRequest(int acceptorsId, int senderId)
        {
            Friendships? friendReq = await _context.Friendships
                .Where(x => x.requestedByInt == senderId)
                .Where(x => x.accepted == false)
                .SingleOrDefaultAsync(k =>
                    (k.KarterFirstId == acceptorsId && k.KarterSecondId == senderId) ||
                    (k.KarterFirstId == senderId && k.KarterSecondId == acceptorsId));

            if (friendReq == null)
            {
                return false;
            }

            friendReq.accepted = true;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task RemoveFriendShip(int sentbyId, int friendId)
        {
            Friendships? friendReq = await _context.Friendships
            .SingleOrDefaultAsync(k =>
            (k.KarterFirstId == sentbyId && k.KarterSecondId == friendId) ||
            (k.KarterFirstId == friendId && k.KarterSecondId == sentbyId));

            if (friendReq == null)
            {
                return;
            }

            _context.Friendships.Remove(friendReq);

            _context.SaveChanges();

        }

        public async Task<Friendships> GetFriendshipByIds(int id1, int id2)
        {
            Friendships? friendReq = await _context.Friendships
                .SingleOrDefaultAsync(k =>
                (k.KarterFirstId == id1 && k.KarterSecondId == id2) ||
                (k.KarterFirstId == id2 && k.KarterSecondId == id1));

            return friendReq;

        }

        public async Task<List<KarterView>> AddStatusToKarters(List<KarterView> karters, int userId)
        {
            foreach (var karter in karters)
            {
                Friendships f = await GetFriendshipByIds(karter.Id, userId);
                if (f == null)
                {
                    if (karter.Id == userId) karter.FriendStatus = FriendshipStatus.UserSelf;

                    karter.FriendStatus = FriendshipStatus.User;
                    continue;
                }

                if (f.accepted)
                {
                    karter.FriendStatus = FriendshipStatus.Friend;
                }
                else
                {
                    if (f.requestedByInt == userId)
                    {
                        karter.FriendStatus = FriendshipStatus.Requested;
                    }
                    else
                    {
                        karter.FriendStatus = FriendshipStatus.Received;
                    }
                }
            }

            return karters;
        }

        public async Task<FriendshipStatus> GetFriendshipStatus(int id1, int id2)
        {
            Friendships f = await GetFriendshipByIds(id2, id1);
            if (f == null)
            {
                if (id2 == id1) return FriendshipStatus.UserSelf;
                return FriendshipStatus.User;
            }

            if (f.accepted)
            {
                return FriendshipStatus.Friend;
            }
            else
            {
                if (f.requestedByInt == id1)
                {
                    return FriendshipStatus.Requested;
                }
                else
                {
                    return FriendshipStatus.Received;
                }
            }
        }
    }
}
