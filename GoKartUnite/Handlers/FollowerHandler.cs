using GoKartUnite.Data;
using GoKartUnite.Models;
using Microsoft.EntityFrameworkCore;

namespace GoKartUnite.Handlers
{
    public class FollowerHandler
    {
        private readonly GoKartUniteContext _context;
        public FollowerHandler(GoKartUniteContext context)
        {
            _context = context;
        }

        public async Task CreateFollow(int karterId, int trackId)
        {
            FollowTrack follow = new FollowTrack(karterId, trackId);
            if (await doesUserFollow(karterId, trackId))
            {
                FollowTrack ftRemove = _context.FollowTracks.Find(karterId, trackId);
                _context.FollowTracks.Remove(ftRemove);
                await _context.SaveChangesAsync();
                return;
            }
            await _context.FollowTracks.AddAsync(follow);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> doesUserFollow(int karterId, int trackId)
        {
            return await _context.FollowTracks.AnyAsync(x => x.KarterId == karterId && x.TrackId == trackId);
        }

    }
}
