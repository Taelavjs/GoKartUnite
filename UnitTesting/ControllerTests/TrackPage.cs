using AngleSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting.ControllerTests
{
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

    }
}
