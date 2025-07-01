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
using GoKartUnite.ViewModel;
using Respawn;
using GoKartUnite.Models;

namespace UnitTesting.ControllerTests
{
    public class IndexPageTests : IClassFixture<TestServer<Program>>, IAsyncLifetime

    {
        private readonly TestServer<Program> _factory;
        private HttpClient _client;
        private IServiceScope _scope;
        private GoKartUniteContext _dbContext;
        private static Checkpoint _checkpoint;
        private string _connectionString = "Server=(localdb)\\mssqllocaldb;Database=GoKartUniteTestDb;Trusted_Connection=True;MultipleActiveResultSets=true";

        public IndexPageTests(
            TestServer<Program> factory)
        {
            _factory = factory;

            // Initialize Respawn checkpoint only once
            if (_checkpoint == null)
            {
                _checkpoint = new Checkpoint
                {
                    TablesToIgnore = new[] { new Respawn.Graph.Table("__EFMigrationsHistory") },
                    DbAdapter = DbAdapter.SqlServer,
                };
            }

        }
        public async Task InitializeAsync()
        {
            _scope = _factory.Services.CreateScope();
            _dbContext = _scope.ServiceProvider.GetRequiredService<GoKartUniteContext>();

            await _dbContext.Database.EnsureCreatedAsync();
            await _checkpoint.Reset(_connectionString);

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

        [Fact]
        public async Task GroupCreation()
        {
            // Arrange: Prepare the form data as key-value pairs
            var formData = new Dictionary<string, string>
            {
                { "PostageGroup.Name", "Example Group" },
                { "PostageGroup.Description", "Example Description" },
                { "PostageGroup.LeaderName", "Example Leader" }
            };

            var content = new FormUrlEncodedContent(formData);

            // Act: Send POST request
            var response = await _client.PostAsync("/Group/CreateGroup", content);
            var response2 = await _client.GetAsync("/Group");

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/Group", response.Headers.Location.ToString());

            var allGroups = await _dbContext.Groups.ToListAsync();
            _dbContext.Groups.RemoveRange(allGroups);
            await _dbContext.SaveChangesAsync();
        }
    }
}
