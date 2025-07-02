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
using UnitTesting.ControllerTests.Bases;

namespace UnitTesting.ControllerTests
{
    public class IndexPageTests : BaseControllerClass, IClassFixture<TestServer<Program>>

    {
        public IndexPageTests(TestServer<Program> server) : base(server) { }
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
