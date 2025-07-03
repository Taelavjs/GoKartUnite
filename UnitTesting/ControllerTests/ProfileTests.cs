using GoKartUnite;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTesting.ControllerTests.Bases;

namespace UnitTesting.ControllerTests
{
    public class ProfileTests : BaseControllerClass, IClassFixture<TestServer<Program>>
    {
        public ProfileTests(TestServer<Program> server) : base(server) { }

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
        }

        // ******************************************************************
    }
}
