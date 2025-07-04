using GoKartUnite;
using GoKartUnite.Models;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTesting.ControllerTests.Bases;

namespace UnitTesting.ControllerTests
{
    public class FriendStatusNotificationReturnedTests : BaseControllerClass, IClassFixture<TestServer<Program>>
    {
        public FriendStatusNotificationReturnedTests(TestServer<Program> factory) : base(factory)
        {
        }

        public async Task<List<FriendStatusNotifications>> DummyNotifications(int count, int toId, int fromId, string friendName)
        {
            var random = new Random();
            var notifications = new List<FriendStatusNotifications>();
            DateTime now = DateTime.UtcNow;
            for (int i = 0; i < count; i++)
            {
                var statuses = Enum.GetValues(typeof(FriendUpdatedStatus));
                var randomStatus = (FriendUpdatedStatus)statuses.GetValue(random.Next(statuses.Length));

                var notification = new FriendStatusNotifications(fromId, friendName, toId, randomStatus)
                {
                    DateCreated = now.AddDays(-i),
                    IsViewed = true
                };

                notifications.Add(notification);
            }

            return await Task.FromResult(notifications);
        }

        [Fact]
        public async Task Test_NotificationOrderReturnedToUser()
        {
            int notifDummyCount = 6;
            int SkipCount = 3;
            int TakeCount = 5;
            // Arrange
            _dbContext.FriendStatusNotifications.AddRange(await DummyNotifications(notifDummyCount, userId, otherUserId, "John"));
            _dbContext.FriendStatusNotifications.AddRange(await DummyNotifications(15, otherUserId, userId, "Not John"));
            _dbContext.SaveChanges();

            // Act
            var response = await _client.GetAsync($"/KarterHome/GetUsersFriendNotifs?Skip={SkipCount}&Take={TakeCount}");
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();
            var jsonDoc = System.Text.Json.JsonDocument.Parse(responseString);
            var root = jsonDoc.RootElement;
            root.TryGetProperty("notifications", out var notificationsArray);

            Assert.True(notificationsArray.GetArrayLength() <= TakeCount);
            DateTime previousDate = DateTime.MaxValue;
            DateTime minExpectedDate = DateTime.UtcNow.AddDays(-SkipCount);
            foreach (var notif in notificationsArray.EnumerateArray())
            {
                DateTime dateCreated = notif.GetProperty("dateCreated").GetDateTime();
                int userFrom = notif.GetProperty("friendId").GetInt32();
                Assert.True(dateCreated <= previousDate);
                Assert.True(dateCreated <= minExpectedDate);
                Assert.Equal(otherUserId, userFrom);
                previousDate = dateCreated;
            }

        }

    }
}
