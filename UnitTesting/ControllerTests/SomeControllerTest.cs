using AngleSharp;
using GoKartUnite;
using GoKartUnite.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnitTesting.HelpersTut;
using Respawn;
using UnitTesting.ControllerTests.Bases;
namespace UnitTesting.ControllerTests
{
    public class SomeControllerTest : BaseControllerClass, IClassFixture<TestServer<Program>>
    {
        public SomeControllerTest(TestServer<Program> factory) : base(factory) { }

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

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }
}
