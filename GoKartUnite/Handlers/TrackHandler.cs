using GoKartUnite.Controllers;
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
            var query = _context.Track;

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
            var blogPosts = _context.BlogPosts.Where(bp => bp.TaggedTrackId == track.Id);
            foreach (var post in blogPosts)
            {
                post.TaggedTrackId = null;
                post.TaggedTrack = null;
            }
            _context.Track.Remove(track);
            _context.SaveChanges();
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

        public async Task<bool> SetTrackToBeVerified(string TrackName, string GooglePlacesId, string FormattedLocation, Coordinates GeoCoords, string Description)
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
                    Latitude = GeoCoords.Latitude,
                    Longitude = GeoCoords.Longitude,
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
        public async Task<List<Track>> GetUnverifiedTracksAdmin()
        {
            return await _context.Track.Where(x => x.IsVerifiedByAdmin == false).ToListAsync();
        }

        public async Task<bool> VerifyTrack(int trackId)
        {
            var trackToBeUpdated = await _context.Track.FirstOrDefaultAsync(t => t.Id == trackId);
            if (trackToBeUpdated == null)
            {
                return false;

            }

            trackToBeUpdated.IsVerifiedByAdmin = true;
            _context.Update(trackToBeUpdated);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task CalculateRecommendedTracksForUser(int userId)
        {
            await GetCloseFriendsScore(userId);
            await GetClosenessScore(userId);
        }
        public static double DistanceBetween(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371;
            var dLat = (lat2 - lat1) * Math.PI / 180.0;
            var dLon = (lon2 - lon1) * Math.PI / 180.0;

            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
        private async Task GetCloseFriendsScore(int userId)
        {
            double nearbyRadiusKm = 50;
            var friendInfos = await _context.Friendships
                .Where(f => f.KarterFirstId == userId || f.KarterSecondId == userId)
                .Select(f => new
                {
                    KarterId = f.KarterFirstId != userId ? f.KarterFirstId : f.KarterSecondId,
                    LocalTrackCoordinates = new
                    {
                        Longitude = f.KarterFirstId != userId
                ? (f.KarterFirst.Track != null ? f.KarterFirst.Track.Longitude : 0)
                : (f.KarterSecond.Track != null ? f.KarterSecond.Track.Longitude : 0),
                        Latitude = f.KarterFirstId != userId
                ? (f.KarterFirst.Track != null ? f.KarterFirst.Track.Latitude : 0)
                : (f.KarterSecond.Track != null ? f.KarterSecond.Track.Latitude : 0),
                    }
                })
                .ToListAsync();
            var tracks = await _context.Track.AsNoTracking().ToListAsync();

            var tracksWithFriendCounts = tracks
                .Select(t => new
                {
                    Track = t,
                    FriendsNearby = friendInfos
                        .Where(friend => DistanceBetween(
                            t.Latitude, t.Longitude,
                            friend.LocalTrackCoordinates.Latitude, friend.LocalTrackCoordinates.Longitude
                        ) <= nearbyRadiusKm)
                        .Count()
                })
                .ToList();
        }


        private async Task GetClosenessScore(int userId)
        {
            double minDistance = 10;
            double maxDistance = 95;
            double minScore = 0.01;
            var tracks = await _context.Track.AsNoTracking().ToListAsync();
            var userTrackObj = await _context.Karter
                .Where(t => t.Id == userId)
                .Select(t => new
                {
                    Longitude = (t.Track != null ? t.Track.Longitude : 0),
                    Latitude = (t.Track != null ? t.Track.Latitude : 0)
                })
                .SingleOrDefaultAsync();
            var closeNessScores = tracks
                .Select(t => new
                {
                    TrackId = t.Id,
                    ClosenessScore = Math.Max(minScore, Math.Min(10, 10 - ((DistanceBetween(t.Longitude, t.Latitude, userTrackObj.Longitude, userTrackObj.Latitude) - minDistance) / (maxDistance - minDistance)) * (10 - minScore) + minScore)),
                    DistanceDebug = DistanceBetween(t.Latitude, t.Longitude, userTrackObj.Latitude, userTrackObj.Longitude)
                });

        }



        /*RANKING TRACKS BY
         * - DISTANCE FROM KARTER
         * - IF THEY FOLLOW SAID TRACK
         * - HOW MANY FRIENDS ARE LOCAL TO SAID TRACK
         * - HOW MANY PEOPLE FOLLOW SAID TRACK
         * - NUMBER OF BLOG POSTS TAGGING SAID TRACK
         * - NUMBER OF TIMES USER HAS INTERACTED WITH SAID TRACK (BLOG, UPVOTES, FOLLOW, COMMENTS)
         * 
         * 
         * 
         * 
         * 
         */
    }
}
