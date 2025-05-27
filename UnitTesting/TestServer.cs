using GoKartUnite.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GoKartUnite;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Linq;
using Microsoft.AspNetCore.Hosting.Server;
using GoKartUnite.Models;
using static System.Formats.Asn1.AsnWriter;

namespace UnitTesting
{
    public class TestServer : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // 🔥 Remove existing authentication configuration
                var authDescriptors = services
                    .Where(d => d.ServiceType == typeof(IConfigureOptions<AuthenticationOptions>))
                    .ToList();

                foreach (var descriptor in authDescriptors)
                {
                    services.Remove(descriptor);
                }

                // ✅ Add test authentication scheme
                services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>("Test", options => { });

                // 🔥 Remove existing DbContext
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<GoKartUniteContext>));
                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                // ✅ Add in-memory test DbContext
                services.AddDbContext<GoKartUniteContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // ✅ Build provider and seed DB
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<GoKartUniteContext>();
                db.Database.EnsureCreated();

            });
        }
        public async Task SeedUserProfileAsync()
        {
            using var scope = Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GoKartUniteContext>();
            dbContext.Comments.RemoveRange(dbContext.Comments);
            dbContext.BlogPosts.RemoveRange(dbContext.BlogPosts);
            dbContext.Karter.RemoveRange(dbContext.Karter);
            dbContext.BlogNotifications.RemoveRange(dbContext.BlogNotifications);
            dbContext.BlogPosts.RemoveRange(dbContext.BlogPosts);
            dbContext.FollowTracks.RemoveRange(dbContext.FollowTracks);
            dbContext.Role.RemoveRange(dbContext.Role);
            dbContext.Friendships.RemoveRange(dbContext.Friendships);
            dbContext.Track.RemoveRange(dbContext.Track);
            dbContext.TrackAdmin.RemoveRange(dbContext.TrackAdmin);
            dbContext.UserRoles.RemoveRange(dbContext.UserRoles);
            dbContext.SaveChanges();

            dbContext.Karter.Add(ConstValues.SelfKarter);

            await dbContext.SaveChangesAsync();
        }

        public async Task SeedTrackVerified()
        {
            using var scope = Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GoKartUniteContext>();

            dbContext.Track.Add(ConstValues.VerifiedTrack);
            await dbContext.SaveChangesAsync();
        }

        public async Task SeedTrackNonVerified()
        {
            using var scope = Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GoKartUniteContext>();

            dbContext.Track.Add(ConstValues.NonVerifiedTrack);
            await dbContext.SaveChangesAsync();
        }
    }
}
