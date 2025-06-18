using GoKartUnite;
using System.Net;
using AngleSharp.Html.Dom;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

using Xunit;
using UnitTesting.HelpersTut;
using Microsoft.Extensions.DependencyInjection;
using GoKartUnite.Data;
using Microsoft.EntityFrameworkCore;
using static System.Formats.Asn1.AsnWriter;

namespace UnitTesting.ControllerTests
{
    public class IndexPageTests : IClassFixture<TestServer<Program>>, IAsyncLifetime

    {
        private readonly TestServer<Program> _factory;
        private HttpClient _client;
        private IServiceScope _scope;
        private GoKartUniteContext _dbContext;

        public IndexPageTests(
            TestServer<Program> factory)
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

        [Fact]
        public async Task Post_DeleteAllMessagesHandler_ReturnsRedirectToRoot()
        {
            // Act
            var response = await _client.GetAsync("/");
            response.EnsureSuccessStatusCode();

            var content = await HtmlHelpers.GetDocumentAsyncaa(response);

            // Assert
            Assert.Equal("Go Kart Unite", content.QuerySelector(".display-4")?.TextContent);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
