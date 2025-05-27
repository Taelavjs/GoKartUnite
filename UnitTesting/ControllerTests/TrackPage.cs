using AngleSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace UnitTesting.ControllerTests
{
    [Collection("Database2")]
    public class TrackPage : IClassFixture<TestServer>
    {
        private readonly TestServer _server;
        public TrackPage(TestServer server)
        {
            _server = server;
        }

        // SETUP
        internal HttpClient ClientCreationForTesting()
        {
            HttpClient client = _server.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", "Test");
            return client;
        }

        // ***************************************************
        // SEARCH TRRACKS
        // ***************************************************

        [Fact]
        public async Task SearchTracks_VerifiedTrackMatch_VerifiedTrackPresent()
        {
            await _server.SeedUserProfileAsync();
            HttpClient client = ClientCreationForTesting();
            await _server.SeedTrackVerified();

            var result = await client.GetAsync(
                $"/TrackHome/SearchTracks?trackSearched={ConstValues.VerifiedTrack.Title}&" +
                "Location=NORTH&Location=NORTHEAST&Location=EAST&Location=SOUTHEAST&" +
                "Location=SOUTH&Location=SOUTHWEST&Location=WEST&Location=NORTHWEST"
            ); var content = await result.Content.ReadAsStringAsync();

            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(content));
            var h2 = document.QuerySelector("h2");

            Assert.NotNull(h2);
            Assert.Equal("Can't find your track? Submit it below for verification",
                        h2.TextContent.Trim());
            Assert.Contains(ConstValues.VerifiedTrack.Title, content.Trim());
        }

        [Fact]
        public async Task SearchTracks_NoVerifiedTracksMatch_VerifiedTrackPresent()
        {
            await _server.SeedUserProfileAsync();
            HttpClient client = ClientCreationForTesting();
            await _server.SeedTrackVerified();

            var result = await client.GetAsync(
                $"/TrackHome/SearchTracks?trackSearched=nonexistenttrack&" +
                "Location=NORTH&Location=NORTHEAST&Location=EAST&Location=SOUTHEAST&" +
                "Location=SOUTH&Location=SOUTHWEST&Location=WEST&Location=NORTHWEST"
            ); var content = await result.Content.ReadAsStringAsync();

            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(content));
            var h2 = document.QuerySelector("h2");

            Assert.NotNull(h2);
            Assert.Equal("No Tracks Found...",
                        h2.TextContent.Trim());
        }

        [Fact]
        public async Task SearchTracks_VerifiedTrackMatch_NoVerifiedTrackPresent()
        {
            await _server.SeedUserProfileAsync();
            HttpClient client = ClientCreationForTesting();
            await _server.SeedTrackNonVerified();

            var result = await client.GetAsync(
                $"/TrackHome/SearchTracks?trackSearched={ConstValues.NonVerifiedTrack.Title}&" +
                "Location=NORTH&Location=NORTHEAST&Location=EAST&Location=SOUTHEAST&" +
                "Location=SOUTH&Location=SOUTHWEST&Location=WEST&Location=NORTHWEST"
            ); var content = await result.Content.ReadAsStringAsync();

            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(content));
            var h2 = document.QuerySelector("h2");

            Assert.NotNull(h2);
            Assert.Equal("No Tracks Found...",
                        h2.TextContent.Trim());
        }
        // ***************************************************
        // END : SEARCH TRRACKS
        // ***************************************************

        // ***************************************************
        // DELETE
        // ***************************************************

        [Fact]
        public async Task Delete_NonAdmin_FailToDelete()
        {
            await _server.SeedUserProfileAsync();
            HttpClient client = ClientCreationForTesting();
            client = Helpers.ChangeUserAuthRole(client, "User");
            await _server.SeedTrackNonVerified();
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(new { }),
                    Encoding.UTF8,
                    "application/json"
            );

            var result = await client.PostAsync("/TrackHome/Delete?id=1", jsonContent);
            var content = result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public async Task Delete_Admin_SuccessfulToDelete()
        {
            await _server.SeedUserProfileWithAdminAsync();
            HttpClient client = ClientCreationForTesting();
            client = Helpers.ChangeUserAuthRole(client, "Admin");

            await _server.SeedTrackNonVerified();
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(new { }),
                    Encoding.UTF8,
                    "application/json"
            );

            var result = await client.PostAsync("/TrackHome/Delete?id=1", jsonContent);
            var content = result.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            // Redirects to Blog Page

        }

    }
}
