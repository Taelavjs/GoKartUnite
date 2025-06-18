using AngleSharp;
using GoKartUnite;
using GoKartUnite.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnitTesting.HelpersTut;

namespace UnitTesting.ControllerTests
{
    public class SomeControllerTest : IClassFixture<TestServer<Program>>
    {
        private readonly TestServer<Program> _factory;
        private readonly HttpClient _client;

        public SomeControllerTest(TestServer<Program> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/KarterHome")]
        [InlineData("/TrackHome")]
        [InlineData("/BlogHome")]
        [InlineData("/GroupsHome")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<GoKartUniteContext>();
                await db.Database.EnsureDeletedAsync(); // Clean slate
                await db.Database.MigrateAsync(); // Apply migrations
                Utilities.InitializeKarterDbForTests(db); // Seed test data
            }
            var client = await HelpersTut.HttpClientExtensions.CreateAuthedClient(_factory);

            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }
}
