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
    public class SomeControllerTest : IClassFixture<TestServer<Program>>, IAsyncLifetime
    {
        private readonly TestServer<Program> _factory;
        private HttpClient _client;
        private IServiceScope _scope;
        private GoKartUniteContext _dbContext;

        public SomeControllerTest(TestServer<Program> factory)
        {
            _factory = factory;
        }

        public async Task InitializeAsync()
        {
            _scope = _factory.Services.CreateScope();
            _dbContext = _scope.ServiceProvider.GetRequiredService<GoKartUniteContext>();
            await _dbContext.Database.EnsureCreatedAsync();
            Utilities.InitializeKarterDbForTests(_dbContext);

            _client = await HttpClientExtensions.CreateAuthedClient(_factory);
        }

        public async Task DisposeAsync()
        {
            await _dbContext.DisposeAsync();
            _scope.Dispose();
            _client.Dispose();
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/KarterHome")]
        [InlineData("/TrackHome")]
        [InlineData("/BlogHome")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var response = await _client.GetAsync(url);

            await HelpersTut.Utilities.ReinitializeKarterDbForTests(_dbContext);
            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }
}
