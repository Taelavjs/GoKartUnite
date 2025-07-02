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

        [Fact]
        public async Task UserAddsFriends_ShowsPendingRequestSent()
        {
            // Arrange

            var response3 = await _client.GetAsync("/karterHome/Details");

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
    }
}
