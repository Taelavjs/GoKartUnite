using GoKartUnite.Data;
using GoKartUnite.Models;
using Microsoft.EntityFrameworkCore;

namespace GoKartUnite.Handlers
{
    public class KarterHandler
    {
        private readonly GoKartUniteContext _context;
        public KarterHandler(GoKartUniteContext context)
        {
            _context = context;
        }

        public async Task<Karter> getUser(int id)
        {
            var karterInDb = await _context.Karter.SingleOrDefaultAsync(x => x.Id == id);
            return karterInDb;
        }

        public async Task<Karter> getUser(string name)
        {
            var karterInDb = await _context.Karter.SingleOrDefaultAsync(k => k.Name.ToLower() == "taela");
            return karterInDb;
        }
        public async Task deleteUser(Karter karter)
        {
            var friendshipsToRemove = await _context.Friendships
                .Where(f => f.KarterFirstId == karter.Id || f.KarterSecondId == karter.Id).ToListAsync();
            _context.Friendships.RemoveRange(friendshipsToRemove);
            _context.Karter.Remove(karter);
            await _context.SaveChangesAsync();
        }

        public async Task deleteUser(int id)
        {
            await deleteUser(await getUser(id));
        }

        public async Task<bool> sendFriendRequest(string friendsName)
        {
            var karter2 = await getUser("taela");
            var karter = await getUser(friendsName);
            if (karter == null || karter2 == null)
            {
                return false;
            }

            if (karter.Id > karter2.Id)
            {
                (karter, karter2) = (karter2, karter);
            }

            var ifExists = await _context.Friendships.FindAsync(karter.Id, karter2.Id); // uupdate when all handlers created
            if (ifExists != null || karter.Id == karter2.Id)
            {
                //var modelErrText = karter.Id == karter2.Id ? "Cannot send a friend request to yourself" : "You are already friends <3";
                //ModelState.AddModelError(string.Empty, modelErrText);
                return false;
            }

            var friendship = new Friendships(karter.Id, karter2.Id);
            _context.Attach(karter);
            _context.Attach(karter2);

            await _context.Friendships.AddAsync(friendship);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<Karter>> getAllUsers(bool fetchTracks)
        {
            return await _context.Karter.Include(k => k.Track).ToListAsync();
        }
    }
}
