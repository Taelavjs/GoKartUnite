using AngleSharp;
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
        // ******************************************
        // TESTING FOR REDIRECT TO CREATE PROFILE 
        // ******************************************

        [Fact]
        public async void GroupPage_AccessingPagesWithoutProfile_RedirectToCreateAccount()
        {
            var client = _server.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", "Test");

            var result = await client.GetAsync("/group");
            var content = await result.Content.ReadAsStringAsync();

            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(content));
            var h1 = document.QuerySelector("h1");

            Assert.NotNull(h1);
            Assert.Equal("Create", h1.TextContent.Trim());
        }

        [Fact]
        public async void ProfilePage_AccessingPagesWithoutProfile_RedirectToCreateAccount()
        {
            var client = _server.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", "Test");

            var result = await client.GetAsync("/karterHome");
            var content = await result.Content.ReadAsStringAsync();

            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(content));
            var h1 = document.QuerySelector("h1");

            Assert.NotNull(h1);
            Assert.Equal("Create", h1.TextContent.Trim());
        }

        [Fact]
        public async void BlogPage_AccessingPagesWithoutProfile_RedirectToCreateAccount()
        {
            var client = _server.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", "Test");

            var result = await client.GetAsync("/blogHome");
            var content = await result.Content.ReadAsStringAsync();

            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(content));
            var h1 = document.QuerySelector("h1");

            Assert.NotNull(h1);
            Assert.Equal("Create", h1.TextContent.Trim());
        }

        [Fact]
        public async void HomePage_AccessingPagesWithoutProfile_RedirectToCreateAccount()
        {
            var client = _server.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", "Test");

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



        [Fact]
        public void test2()
        {

        }
    }
}
