using GoKartUnite.Data;
using GoKartUnite.Models;
using Microsoft.EntityFrameworkCore;
using System.Web.Mvc;
using System.Security.Claims;
using GoKartUnite.ViewModel;
using Microsoft.IdentityModel.Tokens;
using GoKartUnite.Interfaces;

namespace GoKartUnite.Handlers
{
    public class KarterHandler : IKarterHandler
    {
        private readonly GoKartUniteContext _context;
        public KarterHandler(GoKartUniteContext context)
        {
            _context = context;
        }

        public async Task<Karter> GetUser(int id)
        {
            var karterInDb = await _context.Karter.SingleOrDefaultAsync(x => x.Id == id);
            return karterInDb;
        }

        public async Task<Karter> GetUser(string name)
        {
            var karterInDb = await _context.Karter.SingleOrDefaultAsync(k => k.Name.ToLower() == name.ToLower());
            return karterInDb;
        }

        public async Task DeleteUser(Karter karter)
        {
            if (await GetUser(karter.Id) != karter) return;

            var friendshipsToRemove = await _context.Friendships
                .Where(f => f.KarterFirstId == karter.Id || f.KarterSecondId == karter.Id).ToListAsync();
            _context.Friendships.RemoveRange(friendshipsToRemove);



            _context.Karter.Remove(karter);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUser(int id)
        {
            await DeleteUser(await GetUser(id));
        }

        //public async Task<bool> SendFriendRequestByName(string friendsName, string GoogleId)
        //{
        //    var karter2 = await GetUserByGoogleId(GoogleId);
        //    var karter = await GetUser(friendsName);
        //    if (karter == null || karter2 == null)
        //    {
        //        return false;
        //    }

        //    if (karter.Id > karter2.Id)
        //    {
        //        (karter, karter2) = (karter2, karter);
        //    }

        //    var ifExists = await _context.Friendships.FindAsync(karter.Id, karter2.Id); // uupdate when all handlers created
        //    if (ifExists != null || karter.Id == karter2.Id)
        //    {
        //        //var modelErrText = karter.Id == karter2.Id ? "Cannot send a friend request to yourself" : "You are already friends <3";
        //        //ModelState.AddModelError(string.Empty, modelErrText);
        //        return false;
        //    }

        //    var friendship = new Friendships(karter.Id, karter2.Id);
        //    _context.Attach(karter);
        //    _context.Attach(karter2);

        //    await _context.Friendships.AddAsync(friendship);
        //    await _context.SaveChangesAsync();

        //    return true;
        //}

        public async Task<bool> SendFriendRequest(Karter sentBy, Karter requestedTo)
        {
            int sentById = sentBy.Id;
            if (sentBy == null || requestedTo == null)
            {
                return false;
            }

            if (sentBy.Id > requestedTo.Id)
            {
                (sentBy, requestedTo) = (requestedTo, sentBy);
            }

            var ifExists = await _context.Friendships.FindAsync(sentBy.Id, requestedTo.Id); // uupdate when all handlers created
            if (ifExists != null || sentBy.Id == requestedTo.Id)
            {
                //var modelErrText = karter.Id == karter2.Id ? "Cannot send a friend request to yourself" : "You are already friends <3";
                //ModelState.AddModelError(string.Empty, modelErrText);
                return false;
            }

            var friendship = new Friendships(sentBy.Id, requestedTo.Id);
            _context.Attach(sentBy);
            _context.Attach(requestedTo);
            friendship.requestedByInt = sentById;

            await _context.Friendships.AddAsync(friendship);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<Karter>> GetAllUsers(bool fetchTracks, string? track = null, int pageNo = 0, int usersPerPage = 3, SortKartersBy sort = SortKartersBy.Alphabetically)
        {
            var query = _context.Karter.AsQueryable();
            if (fetchTracks)
            {
                query = query.Include(k => k.Track);
            }

            if (!string.IsNullOrEmpty(track))
            {
                query = query.Where(k => k.Track != null && k.Track.Title == track);
            }

            switch (sort)
            {
                case SortKartersBy.Alphabetically:
                    query = query.OrderBy(k => k.Name);
                    break;
                case SortKartersBy.ReverseAlphabetically:
                    query = query.OrderByDescending(k => k.Name);
                    break;
                case SortKartersBy.YearsExperience:
                    query = query.OrderBy(k => k.YearsExperience);
                    break;
                case SortKartersBy.ReverseYearsExperience:
                    query = query.OrderByDescending(k => k.YearsExperience);
                    break;
            }

            query = query.Skip(pageNo * usersPerPage).Take(usersPerPage);
            return await query.ToListAsync();

        }

        public async Task<int> GetNumberOfUserPages(string track, int usersPerPage = 3)
        {
            return _context.Karter.Include(k => k.Track).Count() / usersPerPage;
        }

        public async Task<List<Karter>> GetAllUsersByTrackId(int id)
        {
            var karters = await _context.Karter
                .Where(k => k.TrackId == id)
                .ToListAsync();

            return karters;
        }

        public async Task CreateUser(Karter karter, string email)
        {
            karter.Track = await _context.Track.SingleOrDefaultAsync(t => t.Id == karter.TrackId);
            karter.Email = email;
            var prevKarterRecord = await GetUser(karter.Id);

            if (prevKarterRecord == null)
            {
                if (await _context.Karter.FirstOrDefaultAsync(k => k.NameIdentifier == karter.NameIdentifier) != null)
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

        public async Task<Karter> GetUserByGoogleId(string GoogleId, bool withTrack = false)
        {

            if (withTrack)
            {
                var karterInDbD = await _context.Karter.Include(k => k.Track).FirstOrDefaultAsync(k => k.NameIdentifier == GoogleId);
                return karterInDbD;
            }
            var karterInDb = await _context.Karter.FirstOrDefaultAsync(k => k.NameIdentifier == GoogleId);
            return karterInDb;
        }

        public async Task<List<KarterView>> KarterModelToView(List<Karter> karters)
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

        public async Task<KarterView> KarterModelToView(Karter karter)
        {

            KarterView kv = new KarterView();
            kv.YearsExperience = karter.YearsExperience;
            kv.Name = karter.Name;
            kv.LocalTrack = karter.Track;
            return kv;
        }

        public async Task UpdateUser(string nameIdentifier, KarterView kv)
        {
            Karter k = await GetUserByGoogleId(nameIdentifier);
            k.Name = kv.Name;
            k.TrackId = kv.TrackId;
            k.YearsExperience = kv.YearsExperience;

            _context.Karter.Update(k);
            _context.SaveChanges();
        }

    }
}
