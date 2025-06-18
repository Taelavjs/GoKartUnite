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

namespace UnitTesting.ControllerTests
{
    public class IndexPageTests : IClassFixture<TestServer<Program>>

    {
        private readonly HttpClient _client;
        private readonly TestServer<Program>
            _factory;

        public IndexPageTests(
            TestServer<Program> factory)
        {
            _factory = factory;

        }


        [Fact]
        public async Task Post_DeleteAllMessagesHandler_ReturnsRedirectToRoot()
        {
            // Arrange
            // Arrange

            var client = await HelpersTut.HttpClientExtensions.CreateAuthedClient(_factory);

            var defaultPage = await client.GetAsync("/");
            defaultPage.EnsureSuccessStatusCode();
            var content = await HtmlHelpers.GetDocumentAsyncaa(defaultPage);

            //Act
            Assert.Equal("Go Kart Unite", content.QuerySelector(".display-4")?.TextContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
        }
    }
}
