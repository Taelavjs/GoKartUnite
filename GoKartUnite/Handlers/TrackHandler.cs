using GoKartUnite.Data;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using System.Collections.Generic;
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

        public async Task<List<string>> GetAllTrackTitles()
        {
            return await _context.Track.Select(track => track.Title).ToListAsync();
        }

        public async Task<List<Track>> getTracksByTitle(string title, List<Locations>? location = null)
        {
            if (title == "" || title == null) return new List<Track>();
            if (location == null)
            {
                List<Track> tracks2 = _context.Track.Where(t => t.Title.ToLower().Contains(title.ToLower())).ToList();
                if (tracks2.Count == 0)
                {
                    return new List<Track>();
                }


                return tracks2;
            }

            List<Track> tracks = _context.Track.Where(t => t.Title.ToLower().Contains(title.ToLower()) && location.Contains(t.Location)).ToList();
            if (tracks.Count == 0)
            {
                return new List<Track>();
            }


            return tracks;
        }


        public async Task<Track> getSingleTrackByTitle(string title)
        {
            return await _context.Track.SingleOrDefaultAsync(t => t.Title == title);
        }


        public async Task<List<TrackView>> modelToView(List<Track> tracks)
        {
            List<TrackView> trackRet = new List<TrackView>();
            foreach (Track track in tracks)
            {
                int kartersCount = _context.Track.Entry(track).Collection(p => p.Karters).Query().Count();
                int blogCount = _context.Track.Entry(track).Collection(p => p.BlogPosts).Query().Count();

                TrackView trackView = new TrackView();
                trackView.Title = track.Title;
                trackView.Description = track.Description;
                trackView.karters = kartersCount;
                trackView.blogPosts = blogCount;
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

        public async Task<int> getTrackIdByTitle(string g)
        {
            Track t = await _context.Track.Where(t => t.Title == g).SingleAsync();
            return t.Id;
        }

        public async Task<Track> getTrackById(int id)
        {
            Track t = await _context.Track.Where(t => t.Id == id).SingleAsync();
            return t;
        }

    }
}