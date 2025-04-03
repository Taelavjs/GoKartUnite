using GoKartUnite.Data;
using GoKartUnite.Interfaces;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using System.Collections.Generic;
using System.Web.Mvc;

namespace GoKartUnite.Handlers
{
    public class TrackHandler : ITrackHandler
    {
        private readonly GoKartUniteContext _context;

        public TrackHandler(GoKartUniteContext context)
        {
            _context = context;
        }

        private IQueryable<Track> GetVerifiedTracks()
        {
            return _context.Track.Where(t => t.IsVerifiedByAdmin);
        }

        public async Task<Track?> GetTrack(int id, bool? getKarters)
        {
            var query = GetVerifiedTracks();

            if (getKarters == true)
            {
                return await query
                    .Include(t => t.Karters)
                    .FirstOrDefaultAsync(t => t.Id == id);
            }

            return await query.SingleOrDefaultAsync(t => t.Id == id);
        }

        public async Task<bool> AddTrack(Track track)
        {
            var prevTrackRecord = await GetTrack(track.Id, false);

            if (prevTrackRecord == null)
            {
                if (await _context.Track.AnyAsync(x => x.Title.ToLower() == track.Title.ToLower()))
                {
                    return false;
                }
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

        public async Task<bool> DeleteTrack(int id)
        {
            var track = await GetTrack(id, true);

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

        public async Task<List<Track>> GetAllTracks()
        {
            return await GetVerifiedTracks().ToListAsync();
        }

        public async Task<List<string>> GetAllTrackTitles()
        {
            return await GetVerifiedTracks().Select(track => track.Title).ToListAsync();
        }

        public async Task<List<Track>> GetTracksByTitle(string title, List<Locations>? location = null)
        {
            if (string.IsNullOrEmpty(title)) return new List<Track>();

            var query = GetVerifiedTracks().Where(t => t.Title.ToLower().Contains(title.ToLower()));

            if (location != null)
            {
                query = query.Where(t => location.Contains(t.Location));
            }

            return await query.ToListAsync();
        }

        public async Task<Track> GetSingleTrackByTitle(string title)
        {
            return await GetVerifiedTracks().SingleOrDefaultAsync(t => t.Title == title);
        }

        public async Task<List<TrackView>> ModelToView(List<Track> tracks)
        {
            List<TrackView> trackRet = new List<TrackView>();
            foreach (Track track in tracks)
            {
                int kartersCount = _context.Track.Entry(track).Collection(p => p.Karters).Query().Count();
                int blogCount = _context.Track.Entry(track).Collection(p => p.BlogPosts).Query().Count();

                TrackView trackView = new TrackView
                {
                    Title = track.Title,
                    Description = track.Description,
                    karters = kartersCount,
                    blogPosts = blogCount
                };
                trackRet.Add(trackView);
            }

            return trackRet;
        }

        public async Task<TrackView> ModelToView(Track track)
        {
            return new TrackView
            {
                Title = track.Title,
                Description = track.Description
            };
        }

        public async Task<int> GetTrackIdByTitle(string g)
        {
            Track t = await GetVerifiedTracks().Where(t => t.Title == g).SingleAsync();
            return t.Id;
        }

        public async Task<Track> GetTrackById(int id)
        {
            return await GetVerifiedTracks().Where(t => t.Id == id).SingleAsync();
        }

        public async Task<bool> SetTrackToBeVerified(string TrackName, string GooglePlacesId, string FormattedLocation, string Description)
        {
            if (await _context.Track.AnyAsync(x => x.GooglePlacesId == GooglePlacesId))
            {
                return false;
            }

            if (await _context.Track.AnyAsync(x => x.Title == TrackName && x.FormattedGoogleLocation == FormattedLocation))
            {
                return false;
            }

            try
            {
                Track trackToAddNonVerified = new()
                {
                    Title = TrackName,
                    GooglePlacesId = GooglePlacesId,
                    FormattedGoogleLocation = FormattedLocation,
                    Location = Locations.NORTH,
                    Description = Description,
                    IsVerifiedByAdmin = false,
                };
                await _context.Track.AddAsync(trackToAddNonVerified);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
