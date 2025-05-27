using GoKartUnite.Data;
using GoKartUnite.Handlers;
using GoKartUnite.Interfaces;
using GoKartUnite.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.ContentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting
{
    public class FollowerHandlerIntTesting
    {
        private readonly GoKartUniteContext _context;
        private readonly IFollowerHandler _follower;
        const string testingKartersName = "Dummy";
        const string trackTestingName = "TestTrack";
        public FollowerHandlerIntTesting()
        {

            var options = new DbContextOptionsBuilder<GoKartUniteContext>()
                          .UseInMemoryDatabase(databaseName: "FollowerHandlerIntTesting")
                          .Options;

            _context = new GoKartUniteContext(options);
            _follower = new FollowerHandler(_context);
        }

        public async Task ResetDb(int count = 5, bool withFollowRecords = true)
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            for (int i = 0; i < count; i++)
            {
                Karter k = Helpers.GenerateValidKarters(testingKartersName, i + 1);
                Track t = Helpers.GenerateValidTrack(trackTestingName);

                _context.Karter.Add(k);
                _context.Track.Add(t);
                if (withFollowRecords)
                {
                    FollowTrack followRecord = new FollowTrack(k.Id, t.Id);
                    _context.FollowTracks.Add(followRecord);
                }
            }
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task CreateFollow_ValidKarterIdTrackId_ReturnsTrue()
        {
            await ResetDb(10, false);
            List<FollowTrack> tracksInDb = await _context.FollowTracks.ToListAsync();
            Assert.True(tracksInDb.Count == 0);
            bool success = await _follower.CreateFollow(1, 1);
            Assert.True(success);
            List<FollowTrack> listInDbTesting = await _context.FollowTracks.ToListAsync();
            Assert.True(listInDbTesting.Count == 1);
            Assert.True(listInDbTesting.First().KarterId == 1 && listInDbTesting.First().TrackId == 1);
        }

        [Fact]
        public async Task CreateFollow_WhenFollowIsPresent_ReturnTrue_FollowRemoved()
        {
            await ResetDb(10, true);
            List<FollowTrack> tracksInDb = await _context.FollowTracks.ToListAsync();
            Assert.True(tracksInDb.Count == 10);
            bool success = await _follower.CreateFollow(1, 1);
            Assert.True(success);
            List<FollowTrack> listInDbTesting = await _context.FollowTracks.ToListAsync();
            Assert.True(listInDbTesting.Count == 9);
            Assert.True(listInDbTesting.Where(x => x.KarterId == 1 && x.TrackId == 1).FirstOrDefault() == null);
        }

        [Fact]
        public async Task DoesUserFollow_UserAlreadyFollowing_ReturnTrue()
        {
            await ResetDb(10, true);
            const int userToTestId = 3;
            List<FollowTrack> listOfFollows = await _context.FollowTracks.ToListAsync();
            Assert.NotEmpty(listOfFollows);
            Assert.Equal(listOfFollows.Count, 10);
            int i = 1;
            foreach (FollowTrack track in listOfFollows)
            {
                Assert.True(await _follower.DoesUserFollow(i, i));
                Assert.False(await _follower.DoesUserFollow(i, i + 1));
                Assert.False(await _follower.DoesUserFollow(i, i - 1));
                i++;
            }
        }


        [Fact]
        public async Task GetUsersFollowList_UserFollowsThreeTracks_ReturnsAllThree()
        {
            await ResetDb(15, false);
            int i = 3;
            int min = i;
            while (i < min + 3)
            {
                FollowTrack followRecord = new FollowTrack(2, i++);
                _context.FollowTracks.Add(followRecord);
            }
            await _context.SaveChangesAsync();

            List<FollowTrack> listOfWhoKarterFollows = await _follower.GetUsersFollowList(2);
            Assert.NotEmpty(listOfWhoKarterFollows);
            Assert.Equal(3, listOfWhoKarterFollows.Count);
            foreach (FollowTrack follow in listOfWhoKarterFollows)
            {
                Assert.InRange(follow.TrackId, min, min + 3);
            }
        }
        [Fact]
        public async Task GetUsersFollowList_UserFollowsNoTracks_ReturnsEmptyList()
        {
            await ResetDb(15, false);
            List<FollowTrack> listOfWhoKarterFollows = await _follower.GetUsersFollowList(2);
            Assert.Empty(listOfWhoKarterFollows);
        }

        [Fact]
        public async Task AllUserIdsWhoFollowTrack_ReturnsValid()
        {
            await ResetDb(15, false);
            int i = 3;
            int min = i;
            while (i < min + 5)
            {
                FollowTrack followRecord = new FollowTrack(i++, 2);
                _context.FollowTracks.Add(followRecord);
            }
            await _context.SaveChangesAsync();

            List<int> listReturned = await _follower.AllUserIdsWhoFollowTrack(2);
            Assert.NotEmpty(listReturned);
            Assert.Equal(5, listReturned.Count);
            foreach (int id in listReturned)
            {
                Assert.InRange(id, min, min + 5);
            }
        }

    }
}
