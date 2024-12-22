using GoKartUnite.Data;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Web.Mvc;

namespace GoKartUnite.Handlers
{
    public class TrackHandler
    {
        private readonly GoKartUniteContext _context;
        public TrackHandler(GoKartUniteContext context)
        {
            _context = context;
        }

        public async Task<Track?> getTrack(int id, bool? getKarters)
        {
            if (getKarters == true)
            {
                return await _context.Track
                .Include(t => t.Karters)
                .FirstOrDefaultAsync(t => t.Id == id);
            }
            return await _context.Track.SingleOrDefaultAsync(t => t.Id == id);
        }

        public async Task<bool> addTrack(Track track)
        {
            var prevTrackRecord = await getTrack(track.Id, false);

            if (prevTrackRecord == null)
            {
                _context.Track.Add(track);
            }
            else
            {
                prevTrackRecord.Title = track.Title;
                prevTrackRecord.Description = track.Description;
                prevTrackRecord.Location = track.Location;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task updateTrack(int id)
        {
            var track = await getTrack(id, false);
            _context.Track.Remove(track);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> deleteTrack(int id)
        {
            var track = await getTrack(id, true);

            if (track == null)
            {
                return false;
            }

            foreach (var karter in track.Karters)
            {
                karter.Track = null;
                karter.TrackId = null;
            }
            _context.Track.Remove(track);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Track>> getAllTracks()
        {
            return await _context.Track.ToListAsync();
        }

        public async Task<List<Track>> getTrackByTitle(string title, List<Locations> location)
        {
            if (title == "" || title == null) return new List<Track>();
            List<Track> tracks = _context.Track.Where(t => t.Title.ToLower().Contains(title.ToLower()) && location.Contains(t.Location)).ToList();
            if (tracks.Count == 0)
            {
                return new List<Track>();
            }


            return tracks;
        }


        public async Task<List<TrackView>> modelToView(List<Track> tracks)
        {
            List<TrackView> trackRet = new List<TrackView>();
            foreach (Track track in tracks)
            {
                TrackView trackView = new TrackView();
                trackView.Title = track.Title;
                trackView.Description = track.Description;
                trackRet.Add(trackView);
            }


            return trackRet;
        }

        public async Task<TrackView> modelToView(Track track)
        {
            TrackView trackView = new TrackView();
            trackView.Title = track.Title;
            trackView.Description = track.Description;


            return trackView;
        }
    }
}