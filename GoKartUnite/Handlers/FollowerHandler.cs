using GoKartUnite.Data;
using GoKartUnite.Interfaces;
using GoKartUnite.Models;
using Microsoft.EntityFrameworkCore;
using static System.Reflection.Metadata.BlobBuilder;

namespace GoKartUnite.Handlers
{
    public class FollowerHandler : IFollowerHandler
    {
        private readonly GoKartUniteContext _context;
        public FollowerHandler(GoKartUniteContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateFollow(int karterId, int trackId)
        {
            try
            {
                FollowTrack follow = new FollowTrack(karterId, trackId);
                if (await DoesUserFollow(karterId, trackId))
                {
                    FollowTrack ftRemove = _context.FollowTracks.Find(karterId, trackId);
                    _context.FollowTracks.Remove(ftRemove);
                    await _context.SaveChangesAsync();
                    return true;
                }
                await _context.FollowTracks.AddAsync(follow);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DoesUserFollow(int karterId, int trackId)
        {
            return await _context.FollowTracks.AnyAsync(x => x.KarterId == karterId && x.TrackId == trackId);
        }

        public async Task<List<FollowTrack>> GetUsersFollowList(int karterId)
        {
            return await _context.FollowTracks.Include(x => x.track).Where(t => t.KarterId == karterId).ToListAsync();
        }

        public async Task<List<int>> AllUserIdsWhoFollowTrack(int track)
        {
            var karters = await _context.FollowTracks.Where(t => t.TrackId == track).ToListAsync();
            List<int> Ids = new List<int>();
            foreach (var k in karters)
            {
                Ids.Add(k.KarterId);
            }
            return Ids;
        }

        public async Task<List<string>> GetAllFollowedTracks(int userId)
        {
            return await _context.FollowTracks.Where(k => k.KarterId == userId).Select(t => t.track.Title).ToListAsync();
        }

    }
}
