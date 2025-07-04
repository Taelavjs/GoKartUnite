using GoKartUnite;
using GoKartUnite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTesting.ControllerTests.Bases;

namespace UnitTesting.ControllerTests
{
    public class FriendshipStatusUpdatingTests : BaseControllerClass, IClassFixture<TestServer<Program>>
    {
        public FriendshipStatusUpdatingTests(TestServer<Program> server) : base(server) { }

        // Friend Request : Add ******************************************

        [Fact]
        public async Task UserAddsFriends_ShowsPendingRequestSent()
        {
            // Arrange
            var formData = new Dictionary<string, string>
            {
                { "friendId", otherUserId.ToString() },
                { "friendAction", "Add" }
            };
            var content = new FormUrlEncodedContent(formData);

            // Act
            var response = await _client.PostAsync("/KarterHome/HandleFriendRequest", content);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            var jsonDoc = System.Text.Json.JsonDocument.Parse(responseString);
            var root = jsonDoc.RootElement;
            var newFriendStatus = root.GetProperty("newFriendStatus").GetString();

            // Assert
            Assert.Equal("Cancel", newFriendStatus);  // Because Add returns NewFriendStatus = "Cancel"
            Assert.True(await IsFriendshipStatusNotificationCreated(userId, otherUserId, FriendUpdatedStatus.UserToRequested), "Notification Error");
        }

        [Fact]
        public async Task UserAddsFriends_AlreadyFriends_Error()
        {
            // Arrange
            _dbContext.Friendships.Add(new GoKartUnite.Models.Friendships
            {
                accepted = true,
                KarterFirstId = userId,
                KarterSecondId = otherUserId,
            });
            _dbContext.SaveChanges();
            var formData = new Dictionary<string, string>
            {
                { "friendId", otherUserId.ToString() },
                { "friendAction", "Add" }
            };
            var content = new FormUrlEncodedContent(formData);

            // Act
            var response = await _client.PostAsync("/KarterHome/HandleFriendRequest", content);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            Assert.False(await IsFriendshipStatusNotificationCreated(userId, otherUserId, FriendUpdatedStatus.UserToRequested), "Notification Error");
        }

        // ******************************************************************

        // Friend Request : Accept ******************************************

        [Fact]
        public async Task UserAcceptsFriends_ShowsNewFriendAdded()
        {
            // Arrange

            _dbContext.Friendships.Add(new GoKartUnite.Models.Friendships
            {
                accepted = false,
                KarterFirstId = userId,
                KarterSecondId = otherUserId,
                requestedByInt = otherUserId
            });
            _dbContext.SaveChanges();
            var formData = new Dictionary<string, string>
            {
                { "friendId", otherUserId.ToString() },
                { "friendAction", "Accept" }
            };
            var content = new FormUrlEncodedContent(formData);

            // Act
            var response = await _client.PostAsync("/KarterHome/HandleFriendRequest", content);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            var jsonDoc = System.Text.Json.JsonDocument.Parse(responseString);
            var root = jsonDoc.RootElement;
            var newFriendStatus = root.GetProperty("newFriendStatus").GetString();

            // Assert
            Assert.Equal("Remove", newFriendStatus);
            Assert.True(await IsFriendshipStatusNotificationCreated(userId, otherUserId, FriendUpdatedStatus.RequestedToAccepted), "Notification Error");
        }

        [Fact]
        public async Task UserAcceptsFriends_AlreadyFriends_Error()
        {
            // Arrange
            _dbContext.Friendships.Add(new GoKartUnite.Models.Friendships
            {
                accepted = true,
                KarterFirstId = userId,
                KarterSecondId = otherUserId,
                requestedByInt = otherUserId
            });
            _dbContext.SaveChanges();
            var formData = new Dictionary<string, string>
            {
                { "friendId", otherUserId.ToString() },
                { "friendAction", "Accept" }
            };
            var content = new FormUrlEncodedContent(formData);

            // Act
            var response = await _client.PostAsync("/KarterHome/HandleFriendRequest", content);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            Assert.False(await IsFriendshipStatusNotificationCreated(userId, otherUserId, FriendUpdatedStatus.RequestedToAccepted), "Notification Error");
        }

        // ******************************************************************

        // Friend Request : Withdraws Request ******************************************

        [Fact]
        public async Task UserWithdrawsRequest_ShowsNoPendingRequests()
        {
            // Arrange

            _dbContext.Friendships.Add(new GoKartUnite.Models.Friendships
            {
                accepted = false,
                KarterFirstId = userId,
                KarterSecondId = otherUserId,
                requestedByInt = userId
            });
            _dbContext.SaveChanges();
            var formData = new Dictionary<string, string>
            {
                { "friendId", otherUserId.ToString() },
                { "friendAction", "Cancel" }
            };
            var content = new FormUrlEncodedContent(formData);

            // Act
            var response = await _client.PostAsync("/KarterHome/HandleFriendRequest", content);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            var jsonDoc = System.Text.Json.JsonDocument.Parse(responseString);
            var root = jsonDoc.RootElement;
            var newFriendStatus = root.GetProperty("newFriendStatus").GetString();

            // Assert
            Assert.Equal("Add", newFriendStatus);
            Assert.True(await IsFriendshipStatusNotificationCreated(userId, otherUserId, FriendUpdatedStatus.RequestedToWithdrawn), "Notification Error");
        }

        [Fact]
        public async Task UserWithdraw_AlreadyFriends_RemovesAsFriendSuccessfully()
        {
            var exList = _dbContext.Friendships.ToList();
            if (exList.Count > 0)
            {
                Assert.Equal(false, true);
            }
            // Arrange
            _dbContext.Friendships.Add(new GoKartUnite.Models.Friendships
            {
                accepted = true,
                KarterFirstId = userId,
                KarterSecondId = otherUserId,
                requestedByInt = userId
            });
            _dbContext.SaveChanges();
            var formData = new Dictionary<string, string>
            {
                { "friendId", otherUserId.ToString() },
                { "friendAction", "Cancel" }
            };
            var content = new FormUrlEncodedContent(formData);
            var NotificationsForOtherKarter = _dbContext.FriendStatusNotifications.Where(x => x.UserId == otherUserId).ToList();
            var ddd = _dbContext.FriendStatusNotifications.ToList();

            // Act
            var response = await _client.PostAsync("/KarterHome/HandleFriendRequest", content);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.True(await IsFriendshipStatusNotificationCreated(userId, otherUserId, FriendUpdatedStatus.RequestedToWithdrawn), "Notification Error");


        }

        // ******************************************************************


        // Friend Request : Remove Friend ******************************************

        [Fact]
        public async Task UserRemoveFriend_ShowsNoFriends()
        {
            // Arrange

            _dbContext.Friendships.Add(new GoKartUnite.Models.Friendships
            {
                accepted = true,
                KarterFirstId = userId,
                KarterSecondId = otherUserId,
                requestedByInt = userId
            });
            _dbContext.SaveChanges();
            var formData = new Dictionary<string, string>
            {
                { "friendId", otherUserId.ToString() },
                { "friendAction", "Remove" }
            };
            var content = new FormUrlEncodedContent(formData);

            // Act
            var response = await _client.PostAsync("/KarterHome/HandleFriendRequest", content);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            var jsonDoc = System.Text.Json.JsonDocument.Parse(responseString);
            var root = jsonDoc.RootElement;
            var newFriendStatus = root.GetProperty("newFriendStatus").GetString();

            // Assert
            Assert.Equal("Add", newFriendStatus);
            Assert.True(await IsFriendshipStatusNotificationCreated(userId, otherUserId, FriendUpdatedStatus.FriendToUser), "Notification Error");
        }

        // ******************************************************************


        private async Task<bool> IsFriendshipStatusNotificationCreated(int fromId, int toId, FriendUpdatedStatus statusInNotif)
        {
            List<FriendStatusNotifications> notifs = await _dbContext.FriendStatusNotifications.ToListAsync();

            if (notifs.Count != 1)
            {
                return false;
            }

            if (notifs[0].status == statusInNotif &&
                notifs[0].UserId == toId &&
                notifs[0].FriendId == fromId)
            {
                return true;
            }
            return false;

        }

    }
}
