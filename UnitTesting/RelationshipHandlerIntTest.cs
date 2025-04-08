using GoKartUnite.Data;
using GoKartUnite.Handlers;
using GoKartUnite.Interfaces;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting
{
    public class RelationshipHandlerIntTest
    {
        private readonly GoKartUniteContext _context;
        private readonly IRelationshipHandler _friend;
        public RelationshipHandlerIntTest()
        {

            var options = new DbContextOptionsBuilder<GoKartUniteContext>()
                          .UseInMemoryDatabase(databaseName: "RelationshipHandlerIntTestsDb")
                          .Options;

            _context = new GoKartUniteContext(options);
            _friend = new RelationshipHandler(_context);
        }

        private async Task ResetDb(int count = 8, bool inclFriendships = true)
        {
            if (count % 2 != 0) count += 1;
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            int i = 1;
            while (i < count + 1)
            {
                Karter k = Helpers.GenerateValidKarters("Dummy", i);
                await _context.AddAsync(k);
                i++;
            }
            await _context.SaveChangesAsync();
            i = 1;
            while (i < count + 1)
            {
                Friendships fs = new Friendships(i, i + 1);
                await _context.Friendships.AddAsync(fs);
                i += 2;
            }
            await _context.SaveChangesAsync();
        }

        /*
         * SENT BY ARE ODD IDS
         * SENT TO ARE EVEN IDS
         * 
         */
        [Fact]
        public async Task GetFriendsCount_ValidKarterId_OneFriendRequestSend_NoneAccepted()
        {
            await ResetDb();
            const int karterIdToTest = 2;
            bool isKarterIdInAFriendRequest = await _context.Friendships
                .AnyAsync(x =>
                (x.KarterFirstId == karterIdToTest || x.KarterSecondId == karterIdToTest)
                && x.accepted == false
                );

            Assert.True(isKarterIdInAFriendRequest);
            List<Karter> returnedResult = await _friend.GetAllFriends(karterIdToTest);
            Assert.Empty(returnedResult);
        }

        [Fact]
        public async Task GetFriendsCount_ValidKarterId_AcceptedFriendRequest()
        {
            await ResetDb();
            const int karterIdToTest = 2;
            bool isKarterIdInAFriendRequest = await _context.Friendships
                .AnyAsync(x =>
                (x.KarterFirstId == karterIdToTest || x.KarterSecondId == karterIdToTest)
                && x.accepted == false
                );

            List<Friendships> AllKarterRequests = await _context.Friendships.Where(x => x.KarterFirstId == karterIdToTest || x.KarterSecondId == karterIdToTest).ToListAsync();
            Assert.Equal(1, AllKarterRequests.Count);
            AllKarterRequests.First().accepted = true;
            await _context.SaveChangesAsync();

            Assert.True(isKarterIdInAFriendRequest);
            List<Karter> returnedResult = await _friend.GetAllFriends(karterIdToTest);
            Assert.NotEmpty(returnedResult);
            Assert.Equal(1, returnedResult.Count);
            Assert.NotEqual(karterIdToTest, returnedResult[0].Id);
        }

        [Fact]
        public async Task GetFriendsCount_OneFriendRequestAccepted()
        {
            await ResetDb();
            const int karterIdToTest = 2;
            bool isKarterIdInAFriendRequest = await _context.Friendships
                .AnyAsync(x =>
                (x.KarterFirstId == karterIdToTest || x.KarterSecondId == karterIdToTest)
                && x.accepted == false
                );

            List<Friendships> AllKarterRequests = await _context.Friendships.Where(x => x.KarterFirstId == karterIdToTest || x.KarterSecondId == karterIdToTest).ToListAsync();
            Assert.Equal(1, AllKarterRequests.Count);
            AllKarterRequests.First().accepted = true;
            await _context.SaveChangesAsync();

            Assert.True(isKarterIdInAFriendRequest);
            int returnedResult = await _friend.GetFriendsCount(karterIdToTest);
            Assert.Equal(1, returnedResult);
        }


        [Fact]
        public async Task GetAllSentRequests_ValidKarterId_ReturnsCorrectSentRequests()
        {
            const int count = 16;
            await ResetDb(count);
            const int karterIdToTest = 1;
            const int numFriendRequestsSent = 5 + 1;

            int i = 1;
            while (i <= numFriendRequestsSent)
            {
                Friendships fs = new Friendships(karterIdToTest, 10 + i);
                await _context.Friendships.AddAsync(fs);
                i++;
            }
            await _context.SaveChangesAsync();

            var handler = new RelationshipHandler(_context);

            // Act
            var sentRequests = await handler.GetAllSentRequests(karterIdToTest);

            Assert.NotNull(sentRequests);
            Assert.Equal(numFriendRequestsSent + 1, sentRequests.Count);
            Assert.Contains(sentRequests, friend => friend.Id == 10 || friend.Id == 2);
            Assert.Contains(sentRequests, friend => friend.Id == 15 || friend.Id == 2);
        }

        [Fact]
        public async Task GetAllSentRequests_SomeRequestsAccepted_ReturnsOnlyUnacceptedRequests()
        {
            const int count = 16;
            await ResetDb(count);
            const int karterIdToTest = 1;
            const int numFriendRequestsSent = 6;

            int i = 1;
            while (i <= numFriendRequestsSent)
            {
                Friendships fs = new Friendships(karterIdToTest, 10 + i);
                await _context.Friendships.AddAsync(fs);
                i++;
            }
            await _context.SaveChangesAsync();

            var allRequests = await _context.Friendships
                .Where(x => (x.KarterFirstId == karterIdToTest || x.KarterSecondId == karterIdToTest))
                .ToListAsync();

            var acceptedRequests = allRequests.Take(3).ToList();
            foreach (var request in acceptedRequests)
            {
                request.accepted = true;
            }
            await _context.SaveChangesAsync();

            var returnedFromMethod = await _friend.GetAllSentRequests(karterIdToTest);
            foreach (var request in returnedFromMethod)
            {
                Assert.DoesNotContain(request.Id, acceptedRequests.Select(r => r.KarterSecondId));
            }
        }

        [Fact]
        public async Task GetAllSentRequests_KarterHasSentNoRequests_ReturnsEmptyList()
        {
            const int count = 16;
            await ResetDb(count);
            const int karterIdToTest = 2;
            const int numFriendRequestsSent = 0;

            var handler = new RelationshipHandler(_context);

            var sentRequests = await handler.GetAllSentRequests(karterIdToTest);

            Assert.NotNull(sentRequests);
            Assert.Empty(sentRequests);
        }

        [Fact]
        public async Task GetAllSentRequests_AllRequestsAccepted_ReturnsEmptyList()
        {
            const int count = 16;
            await ResetDb(count);
            const int karterIdToTest = 1;
            const int numFriendRequestsSent = 5;

            int i = 1;
            while (i <= numFriendRequestsSent)
            {
                Friendships fs = new Friendships(karterIdToTest, 10 + i);
                await _context.Friendships.AddAsync(fs);
                i++;
            }
            await _context.SaveChangesAsync();

            var allRequests = await _context.Friendships
                .Where(x => (x.KarterFirstId == karterIdToTest || x.KarterSecondId == karterIdToTest))
                .ToListAsync();

            var acceptedRequests = allRequests;
            foreach (var request in acceptedRequests)
            {
                request.accepted = true;
            }
            await _context.SaveChangesAsync();

            var handler = new RelationshipHandler(_context);

            var sentRequests = await handler.GetAllSentRequests(karterIdToTest);

            Assert.NotNull(sentRequests);
            Assert.Empty(sentRequests);
        }

        [Fact]
        public async Task GetAllSentRequests_NoRequestsAccepted_ReturnsAllSentRequests()
        {
            const int count = 16;
            await ResetDb(count);
            const int karterIdToTest = 1; // Karter with ID 1
            const int numFriendRequestsSent = 4; // Karter 1 sends 4 requests

            int i = 1;
            while (i <= numFriendRequestsSent)
            {
                Friendships fs = new Friendships(karterIdToTest, 10 + i);
                await _context.Friendships.AddAsync(fs);
                i++;
            }
            await _context.SaveChangesAsync();

            var handler = new RelationshipHandler(_context);

            // Act
            var sentRequests = await handler.GetAllSentRequests(karterIdToTest);

            Assert.NotNull(sentRequests);
            Assert.Equal(numFriendRequestsSent + 1, sentRequests.Count); // All requests should be returned, none accepted
        }
        [Fact]
        public async Task GetAllSentRequests_KarterDoesNotExist_ReturnsEmptyList()
        {
            const int count = 16;
            await ResetDb(count);
            const int nonExistentKarterId = 999;

            var handler = new RelationshipHandler(_context);

            var sentRequests = await handler.GetAllSentRequests(nonExistentKarterId);

            Assert.NotNull(sentRequests);
            Assert.Empty(sentRequests);
        }

        [Fact]
        public async Task AcceptFriendRequest_ValidRequest()
        {
            await ResetDb(10);
            const int karterId = 2;
            bool res = await _friend.AcceptFriendRequest(karterId, karterId - 1);
            List<Friendships> friendships = await _context.Friendships.Where(x => x.accepted == false).ToListAsync();
            Assert.True(res);
            Assert.Equal(4, friendships.Count);
            foreach (Friendships friendship in friendships)
            {
                Assert.False(friendship.accepted);
            }
        }

        [Fact]
        public async Task AcceptFriendRequest_InvalidRequest_SenderTriesToAccept()
        {
            await ResetDb(10);
            const int karterId = 1;
            bool res = await _friend.AcceptFriendRequest(karterId, karterId - 1);
            List<Friendships> friendships = await _context.Friendships.Where(x => x.accepted == false).ToListAsync();
            Assert.False(res);
            Assert.Equal(5, friendships.Count);
            foreach (Friendships friendship in friendships)
            {
                Assert.False(friendship.accepted);
            }
        }

        [Fact]
        public async Task AcceptFriendRequest_InvalidRequest_KarterIdInvalid()
        {
            await ResetDb(10);
            const int karterId = -1;
            bool res = await _friend.AcceptFriendRequest(karterId, karterId - 1);
            List<Friendships> friendships = await _context.Friendships.Where(x => x.accepted == false).ToListAsync();
            Assert.False(res);
            Assert.Equal(5, friendships.Count);
            foreach (Friendships friendship in friendships)
            {
                Assert.False(friendship.accepted);
            }
        }

        [Fact]
        public async Task AddStatusToKarters_NoFriendship_StatusIsUser()
        {
            await ResetDb(5);
            const int userId = 4;
            const int karterId = 1;

            List<KarterView> karters = new List<KarterView>
            {
                new KarterView { Id = karterId }
            };

            List<KarterView> result = await _friend.AddStatusToKarters(karters, userId);

            Assert.Single(result);
            Assert.Equal(FriendshipStatus.User, result[0].FriendStatus);
        }

        [Fact]
        public async Task AddStatusToKarters_AcceptedFriendship_StatusIsFriend()
        {
            await ResetDb();
            const int userId = 5;
            const int karterId = 2;

            // Add a friendship between userId and karterId
            Friendships fs = new Friendships(userId, karterId) { accepted = true };
            await _context.Friendships.AddAsync(fs);
            await _context.SaveChangesAsync();

            List<KarterView> karters = new List<KarterView>
            {
                new KarterView { Id = karterId }
            };

            List<KarterView> result = await _friend.AddStatusToKarters(karters, userId);

            Assert.Single(result);
            Assert.Equal(FriendshipStatus.Friend, result[0].FriendStatus);
        }

        [Fact]
        public async Task AddStatusToKarters_ReceivedFriendship_StatusIsReceived()
        {
            await ResetDb();
            const int userId = 5;
            const int karterId = 2;

            Friendships fs = new Friendships(karterId, userId) { accepted = false, requestedByInt = karterId };
            await _context.Friendships.AddAsync(fs);
            await _context.SaveChangesAsync();

            List<KarterView> karters = new List<KarterView>
            {
                new KarterView { Id = karterId }
            };

            List<KarterView> result = await _friend.AddStatusToKarters(karters, userId);

            Assert.Single(result);
            Assert.Equal(FriendshipStatus.Received, result[0].FriendStatus);
        }

        [Fact]
        public async Task AddStatusToKarters_RequestedFriendship_StatusIsRequested()
        {
            await ResetDb();
            const int userId = 2;
            const int karterId = 6;

            Friendships fs = new Friendships(userId, karterId) { accepted = false, requestedByInt = userId };
            await _context.Friendships.AddAsync(fs);
            await _context.SaveChangesAsync();

            List<KarterView> karters = new List<KarterView>
            {
                new KarterView { Id = karterId }
            };

            List<KarterView> result = await _friend.AddStatusToKarters(karters, userId);

            Assert.Single(result);
            Assert.Equal(FriendshipStatus.Requested, result[0].FriendStatus);
        }

        [Fact]
        public async Task AddStatusToKarters_MultipleKarters_CorrectStatuses()
        {
            await ResetDb();
            const int userId = 8;

            List<KarterView> karters = new List<KarterView>
            {
                new KarterView { Id = 2 },
                new KarterView { Id = 4 },
                new KarterView { Id = 6 }
            };

            Friendships fs1 = new Friendships(userId, 4) { accepted = true };
            await _context.Friendships.AddAsync(fs1);

            Friendships fs2 = new Friendships(6, userId) { accepted = false, requestedByInt = 4 };
            await _context.Friendships.AddAsync(fs2);

            await _context.SaveChangesAsync();

            List<KarterView> result = await _friend.AddStatusToKarters(karters, userId);

            Assert.Equal(FriendshipStatus.User, result[0].FriendStatus);
            Assert.Equal(FriendshipStatus.Friend, result[1].FriendStatus);
            Assert.Equal(FriendshipStatus.Received, result[2].FriendStatus);
        }



    }
}
