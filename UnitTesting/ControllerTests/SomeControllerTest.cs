using AngleSharp;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting.ControllerTests
{
    public class SomeControllerTest : IClassFixture<TestServer>
    {
        private readonly TestServer _server;
        public SomeControllerTest(TestServer server)
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

        // ******************************************


        // ******************************************
        // TESTING FOR REDIRECT TO CREATE PROFILE 
        // ******************************************

        [Fact]
        public async Task TrackPageSearchFeature_AccessingPagesWithoutProfile_Successful()
        {
            await _server.SeedUserProfileAsync();

            HttpClient client = ClientCreationForTesting();

            var result = await client.GetAsync("/TrackHome/SearchTracks? trackSearched = Buckmore & Location = NORTH & Location = NORTHEAST & Location = EAST & Location = SOUTHEAST & Location = SOUTH & Location = SOUTHWEST & Location = WEST & Location = NORTHWEST\r\n");
            var content = await result.Content.ReadAsStringAsync();

            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(content));
            var h1 = document.QuerySelector("h1");

            Assert.NotNull(h1);
            Assert.Equal("Create", h1.TextContent.Trim());
        }

        [Fact]
        public async Task TrackPage_AccessingPagesWithoutProfile_RedirectToCreateAccount()
        {
            HttpClient client = ClientCreationForTesting();

            var result = await client.GetAsync("/trackHome");
            var content = await result.Content.ReadAsStringAsync();

            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(content));
            var h1 = document.QuerySelector("h1");

            Assert.NotNull(h1);
            Assert.Equal("Create", h1.TextContent.Trim());
        }

        [Fact]
        public async Task GroupPage_AccessingPagesWithoutProfile_RedirectToCreateAccount()
        {
            HttpClient client = ClientCreationForTesting();

            var result = await client.GetAsync("/group");
            var content = await result.Content.ReadAsStringAsync();

            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(content));
            var h1 = document.QuerySelector("h1");

            Assert.NotNull(h1);
            Assert.Equal("Create", h1.TextContent.Trim());
        }

        [Fact]
        public async Task ProfilePage_AccessingPagesWithoutProfile_RedirectToCreateAccount()
        {
            HttpClient client = ClientCreationForTesting();

            var result = await client.GetAsync("/karterHome");
            var content = await result.Content.ReadAsStringAsync();

            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(content));
            var h1 = document.QuerySelector("h1");

            Assert.NotNull(h1);
            Assert.Equal("Create", h1.TextContent.Trim());
        }

        [Fact]
        public async Task BlogPage_AccessingPagesWithoutProfile_RedirectToCreateAccount()
        {
            HttpClient client = ClientCreationForTesting();

            var result = await client.GetAsync("/blogHome");
            var content = await result.Content.ReadAsStringAsync();

            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(content));
            var h1 = document.QuerySelector("h1");

            Assert.NotNull(h1);
            Assert.Equal("Create", h1.TextContent.Trim());
        }

        [Fact]
        public async Task HomePage_AccessingPagesWithoutProfile_RedirectToCreateAccount()
        {
            HttpClient client = ClientCreationForTesting();

            var result = await client.GetAsync("/");
            var content = await result.Content.ReadAsStringAsync();

            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(content));
            var h1 = document.QuerySelector("h1");

            Assert.NotNull(h1);
            Assert.Equal("Create", h1.TextContent.Trim());
        }
        // ******************************************
        // END : TESTING FOR REDIRECT TO CREATE PROFILE 
        // ******************************************

        // ******************************************
        // Created Profile For User, Testing all page access
        // ******************************************
        [Fact]
        public async Task ProfilePage_AccessingPages_Successful()
        {
            await _server.SeedUserProfileAsync();

            HttpClient client = ClientCreationForTesting();

            var result = await client.GetAsync("/karterHome");
            var content = await result.Content.ReadAsStringAsync();

            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(content));
            var h1 = document.QuerySelector(".profile-name");

            Assert.NotNull(h1);
            Assert.Equal(ConstValues.SelfKarter.Name, h1.TextContent.Trim());
            Assert.Contains(ConstValues.SelfKarter.Email, content);
        }

        [Fact]
        public async Task BlogPage_AccessingPages_Successful()
        {
            await _server.SeedUserProfileAsync();

            HttpClient client = ClientCreationForTesting();

            var result = await client.GetAsync("/blogHome");
            var content = await result.Content.ReadAsStringAsync();

            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(content));
            var h1 = document.QuerySelector("h1");

            Assert.NotNull(h1);
            Assert.Equal("Blog", h1.TextContent.Trim());
        }

        [Fact]
        public async Task TrackPage_AccessingPages_Successful()
        {
            await _server.SeedUserProfileAsync();

            HttpClient client = ClientCreationForTesting();

            var result = await client.GetAsync("/trackHome");
            var content = await result.Content.ReadAsStringAsync();

            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(content));
            var h1 = document.QuerySelector("h1");

            Assert.NotNull(h1);
            Assert.Equal("Track Home", h1.TextContent.Trim());
        }
        [Fact]
        public async Task TrackPageSearchFeature_AccessingPages_Successful()
        {
            await _server.SeedUserProfileAsync();

            HttpClient client = ClientCreationForTesting();

            var result = await client.GetAsync("/TrackHome/SearchTracks? trackSearched = Buckmore & Location = NORTH & Location = NORTHEAST & Location = EAST & Location = SOUTHEAST & Location = SOUTH & Location = SOUTHWEST & Location = WEST & Location = NORTHWEST\r\n");
            var content = await result.Content.ReadAsStringAsync();

            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(content));
            var h2 = document.QuerySelector("h2");

            Assert.NotNull(h2);
            Assert.Equal("No Tracks Found...", h2.TextContent.Trim());
        }
        [Fact]
        public async Task HomePage_AccessingPages_Successful()
        {
            await _server.SeedUserProfileAsync();

            HttpClient client = ClientCreationForTesting();

            var result = await client.GetAsync("/");
            var content = await result.Content.ReadAsStringAsync();

            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(content));
            var h1 = document.QuerySelector("h1");

            Assert.NotNull(h1);
            Assert.Equal("Go Kart Unite", h1.TextContent.Trim());
        }

        [Fact]
        public async Task GroupPage_AccessingPages_Successful()
        {
            await _server.SeedUserProfileAsync();

            HttpClient client = ClientCreationForTesting();

            var result = await client.GetAsync("/Group");
            var content = await result.Content.ReadAsStringAsync();

            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(content));
            var SearchForGroupInput = document.QuerySelector("#SearchGroupsInput");

            Assert.NotNull(SearchForGroupInput);
            Assert.Equal("Search Groups", SearchForGroupInput.GetAttribute("placeholder"));
        }

        // ******************************************
        // END : Created Profile For User, Testing all page access
        // ******************************************

    }
}
