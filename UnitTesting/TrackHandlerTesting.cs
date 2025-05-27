using GoKartUnite.Data;
using GoKartUnite.Handlers;
using GoKartUnite.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting
{
    public class TrackHandlerTesting
    {
        private readonly GoKartUniteContext _context;
        private readonly TrackHandler _trackHandler;

        private int TracksToCreateInitially = 3;

        public TrackHandlerTesting()
        {

            var options = new DbContextOptionsBuilder<GoKartUniteContext>()
                          .UseInMemoryDatabase(databaseName: "TrackAdminDb")
                          .Options;

            _context = new GoKartUniteContext(options);
            _trackHandler = new TrackHandler(_context);
        }

        private async Task ResetEnvironment()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            var Trackers = Enumerable.Range(1, TracksToCreateInitially)
                .Select(i => Helpers.GenerateValidTrack("Track", i))
                .ToList();
            await _context.Track.AddRangeAsync(Trackers);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task AddTrack_CreatingNewTrack_Valid()
        {
            await ResetEnvironment();

            var newTrack = Helpers.GenerateValidTrack("new", 0);
            await _trackHandler.AddTrack(newTrack);

            var listTracks = _context.Track.ToList();
            Assert.Equal(TracksToCreateInitially + 1, listTracks.Count);
        }

        [Fact]
        public async Task AddTrack_CreatingNewTrack_RepeatedTrackName()
        {
            await ResetEnvironment();

            var newTrack = Helpers.GenerateValidTrack("Track", 1);
            await _trackHandler.AddTrack(newTrack);

            var listTracks = _context.Track.ToList();
            Assert.Equal(TracksToCreateInitially, listTracks.Count);
        }

        [Fact]
        public async Task AddTrack_UpdatePreviousRecord()
        {
            await ResetEnvironment();
            var prevRecord = _context.Track.ToList().First();

            // Edit Values
            string editedDesc = "Testing Description";
            Locations editedLocation = Locations.SOUTHWEST;

            prevRecord.Description = editedDesc;
            prevRecord.Location = Locations.SOUTHWEST;

            await _trackHandler.AddTrack(prevRecord);

            Track retRecord = _context.Track.ToList().First();

            Assert.Equal(_context.Track.Count(), TracksToCreateInitially);
            Assert.Equal(editedDesc, retRecord.Description);
            Assert.Equal(editedLocation, retRecord.Location);
        }

        [Fact]
        public async Task DeleteTrack_ValidTrackToBeDeleted()
        {
            await ResetEnvironment();
            var result = await _trackHandler.DeleteTrack(1);

            Assert.True(result);
            Assert.Equal(_context.Track.Count(), TracksToCreateInitially - 1);
            foreach (var track in _context.Track.ToList())
            {
                Assert.True(track.Title.Contains("1") == false);
            }
        }

        [Fact]
        public async Task DeleteTrack_InvalidTrackToBeDeleted()
        {
            await ResetEnvironment();
            var result = _trackHandler.DeleteTrack(-1);
            Assert.False(await result);
            Assert.Equal(_context.Track.Count(), TracksToCreateInitially);

        }

        [Fact]
        public async Task GetTracksByTitle_ValidTitleToBeSearchedFor_ReturnsAllSinceAllStartWithSameName()
        {
            await ResetEnvironment();

            List<Track> tracksReturned = await _trackHandler.GetTracksByTitle("Track");
            Assert.Equal(tracksReturned.Count, TracksToCreateInitially);

            List<Track> tracksReturned1 = await _trackHandler.GetTracksByTitle("Track1");
            Assert.Equal(tracksReturned1.Count, 1);

            List<Track> tracksReturned2 = await _trackHandler.GetTracksByTitle("ack");
            Assert.Equal(tracksReturned2.Count, TracksToCreateInitially);
        }

        [Fact]
        public async Task GetTracksByTitle_ReturnsNonePoorSearch()
        {
            await ResetEnvironment();

            List<Track> tracksReturned = await _trackHandler.GetTracksByTitle("");
            Assert.Equal(tracksReturned.Count, 0);

            tracksReturned = await _trackHandler.GetTracksByTitle(null);
            Assert.Equal(tracksReturned.Count, 0);
        }


        [Fact]
        public async Task GetTracksByTitle_ValidSearch_SpecifiedLocation()
        {
            await ResetEnvironment();
            var Trackers = Enumerable.Range(1, TracksToCreateInitially)
                .Select(i => Helpers.GenerateValidTrack("TrackSouth", i, Locations.SOUTHWEST))
                .ToList();
            await _context.Track.AddRangeAsync(Trackers);
            await _context.SaveChangesAsync();

            List<Track> tracksReturned = await _trackHandler.GetTracksByTitle("Track", new List<Locations> { Locations.SOUTHWEST });
            Assert.Equal(tracksReturned.Count, TracksToCreateInitially);

            tracksReturned = await _trackHandler.GetTracksByTitle("ack", new List<Locations> { Locations.NORTHEAST });
            Assert.Equal(tracksReturned.Count, TracksToCreateInitially);

            tracksReturned = await _trackHandler.GetTracksByTitle("", new List<Locations> { Locations.NORTHEAST });
            Assert.Equal(tracksReturned.Count, 0);
        }

        [Fact]
        public async Task ModelToView_ListOfTracks_ConvertsCorrectly()
        {
            await ResetEnvironment();
            var tracks = await _context.Track.Include(x => x.BlogPosts).Include(x => x.Karters).ToListAsync();

            var trackViews = await _trackHandler.ModelToView(tracks);

            Assert.Equal(TracksToCreateInitially, trackViews.Count);

            for (int i = 0; i < TracksToCreateInitially; i++)
            {
                Assert.Equal($"Track{i + 1}", trackViews[i].Title);
                Assert.Equal(0, trackViews[i].karters);
                Assert.Equal(0, trackViews[i].blogPosts);
            }
        }

        [Fact]
        public async Task ModelToView_SingleTrack_ConvertsCorrectly()
        {
            await ResetEnvironment();
            var track = await _context.Track.FirstAsync();

            var trackView = await _trackHandler.ModelToView(track);

            Assert.Equal(track.Title, trackView.Title);
            Assert.Equal(track.Description, trackView.Description);
        }

        [Fact]
        public async Task ModelToView_ListOfTracks_TracksWithKartersAndBlogs_CorrectCounts()
        {
            await ResetEnvironment();
            var track = await _context.Track.Include(x => x.Karters).Include(x => x.BlogPosts).FirstAsync();

            var karter1 = new Karter { Name = "Karter 1", YearsExperience = 5 };
            var karter2 = new Karter { Name = "Karter 2", YearsExperience = 2 };

            var blogPost1 = new BlogPost { Title = "Blog 1", Description = "Blog Description" };
            var blogPost2 = new BlogPost { Title = "Blog 2", Description = "Another Blog" };

            track.Karters.Add(karter1);
            track.Karters.Add(karter2);
            track.BlogPosts.Add(blogPost1);
            track.BlogPosts.Add(blogPost2);

            await _context.SaveChangesAsync();

            var tracks = await _context.Track.ToListAsync();

            var trackViews = await _trackHandler.ModelToView(tracks);

            var trackView = trackViews.First();
            Assert.Equal(2, trackView.karters);
            Assert.Equal(2, trackView.blogPosts);
        }
    }
}
