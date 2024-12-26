using GoKartUnite.Data;
using GoKartUnite.Models;
using Microsoft.EntityFrameworkCore;
using System.Web.Mvc;
using System.Security.Claims;
using GoKartUnite.ViewModel;
using Microsoft.IdentityModel.Tokens;

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
            var karterInDb = await _context.Karter.SingleOrDefaultAsync(k => k.Name.ToLower() == name);
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

        public async Task<bool> sendFriendRequest(string friendsName, string GoogleId)
        {
            var karter2 = await getUserByGoogleId(GoogleId);
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

        public async Task<List<Karter>> getAllUsers(bool fetchTracks, string? track = null)
        {
            if (fetchTracks)
            {
                return await _context.Karter.Include(k => k.Track).ToListAsync();

            }

            if (!track.IsNullOrEmpty())
            {
                return await _context.Karter.Include(k => k.Track).Where(k => k.Track != null && k.Track.Title == track).ToListAsync();

            }

            return await _context.Karter.Include(k => k.Track).ToListAsync();
        }

        public async Task<List<Karter>> getAllUsersByTrackId(int id)
        {
            var karters = await _context.Karter
                .Where(k => k.TrackId == id)
                .ToListAsync();

            return karters;
        }

        public async Task createUser(Karter karter, string email)
        {
            karter.Track = await _context.Track.SingleOrDefaultAsync(t => t.Id == karter.TrackId);
            karter.Email = email;
            var prevKarterRecord = await getUser(karter.Id);

            if (prevKarterRecord == null)
            {
                if (await _context.Karter.FirstOrDefaultAsync(k => k.GoogleId == karter.GoogleId) != null)
                {
                    return;
                }
                _context.Karter.Add(karter);
            }
            else
            {
                prevKarterRecord.Name = karter.Name;
                prevKarterRecord.Track = karter.Track;
                prevKarterRecord.TrackId = karter.TrackId;
                prevKarterRecord.YearsExperience = karter.YearsExperience;
            }

            await _context.SaveChangesAsync();

        }

        public async Task<Karter> getUserByGoogleId(string GoogleId, bool withTrack = false)
        {

            if (withTrack)
            {
                var karterInDbD = await _context.Karter.Include(k => k.Track).FirstOrDefaultAsync(k => k.GoogleId == GoogleId);
                return karterInDbD;
            }
            var karterInDb = await _context.Karter.FirstOrDefaultAsync(k => k.GoogleId == GoogleId);
            return karterInDb;
        }

        public async Task<List<KarterView>> karterModelToView(List<Karter> karters)
        {
            List<KarterView> kvs = new List<KarterView>();
            foreach (var karter in karters)
            {

                KarterView kv = new KarterView();
                kv.Id = karter.Id;
                kv.YearsExperience = karter.YearsExperience;
                kv.Name = karter.Name;
                kv.LocalTrack = karter.Track;


                kvs.Add(kv);
            }

            return kvs;
        }

        public async Task<KarterView> karterModelToView(Karter karter)
        {

            KarterView kv = new KarterView();
            kv.YearsExperience = karter.YearsExperience;
            kv.Name = karter.Name;
            kv.LocalTrack = karter.Track;
            return kv;
        }
    }
}
