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
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace UnitTesting
{
    public class TestServer<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var dbName = $"GoKartUniteTestDb_{Guid.NewGuid()}";

            var _connectionString = $"Server=(localdb)\\mssqllocaldb;Database={dbName};Trusted_Connection=True;MultipleActiveResultSets=true";

            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext 
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<GoKartUniteContext>));
                if (dbContextDescriptor != null)
                    services.Remove(dbContextDescriptor);

                // Remove existing DbConnection 
                var dbConnectionDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbConnection));
                if (dbConnectionDescriptor != null)
                    services.Remove(dbConnectionDescriptor);

                services.AddDbContext<GoKartUniteContext>(options =>
                {
                    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=GoKartUniteTestDb;Trusted_Connection=True;MultipleActiveResultSets=true");
                });
            });

            builder.UseEnvironment("Testing");
        }
        //public async Task SeedUserProfileAsync()
        //{
        //    using var scope = Services.CreateScope();
        //    var dbContext = scope.ServiceProvider.GetRequiredService<GoKartUniteContext>();
        //    dbContext.Comments.RemoveRange(dbContext.Comments);
        //    dbContext.BlogPosts.RemoveRange(dbContext.BlogPosts);
        //    dbContext.Karter.RemoveRange(dbContext.Karter);
        //    dbContext.BlogNotifications.RemoveRange(dbContext.BlogNotifications);
        //    dbContext.BlogPosts.RemoveRange(dbContext.BlogPosts);
        //    dbContext.FollowTracks.RemoveRange(dbContext.FollowTracks);
        //    dbContext.Role.RemoveRange(dbContext.Role);
        //    dbContext.Friendships.RemoveRange(dbContext.Friendships);
        //    dbContext.Track.RemoveRange(dbContext.Track);
        //    dbContext.TrackAdmin.RemoveRange(dbContext.TrackAdmin);
        //    dbContext.UserRoles.RemoveRange(dbContext.UserRoles);
        //    dbContext.SaveChanges();

        //    var karter = ConstValues.SelfKarter;
        //    dbContext.Karter.Add(karter);
        //    await dbContext.SaveChangesAsync();
        //}

        //public async Task ClearDatabase()
        //{
        //    using var scope = Services.CreateScope();
        //    var dbContext = scope.ServiceProvider.GetRequiredService<GoKartUniteContext>();
        //    dbContext.Comments.RemoveRange(dbContext.Comments);
        //    dbContext.BlogPosts.RemoveRange(dbContext.BlogPosts);
        //    dbContext.Karter.RemoveRange(dbContext.Karter);
        //    dbContext.BlogNotifications.RemoveRange(dbContext.BlogNotifications);
        //    dbContext.BlogPosts.RemoveRange(dbContext.BlogPosts);
        //    dbContext.FollowTracks.RemoveRange(dbContext.FollowTracks);
        //    dbContext.Role.RemoveRange(dbContext.Role);
        //    dbContext.Friendships.RemoveRange(dbContext.Friendships);
        //    dbContext.Track.RemoveRange(dbContext.Track);
        //    dbContext.TrackAdmin.RemoveRange(dbContext.TrackAdmin);
        //    dbContext.UserRoles.RemoveRange(dbContext.UserRoles);
        //    dbContext.SaveChanges();
        //}

        //public async Task SeedUserProfileWithAdminAsync()
        //{
        //    using var scope = Services.CreateScope();
        //    var dbContext = scope.ServiceProvider.GetRequiredService<GoKartUniteContext>();
        //    dbContext.Comments.RemoveRange(dbContext.Comments);
        //    dbContext.BlogPosts.RemoveRange(dbContext.BlogPosts);
        //    dbContext.Karter.RemoveRange(dbContext.Karter);
        //    dbContext.BlogNotifications.RemoveRange(dbContext.BlogNotifications);
        //    dbContext.BlogPosts.RemoveRange(dbContext.BlogPosts);
        //    dbContext.FollowTracks.RemoveRange(dbContext.FollowTracks);
        //    dbContext.Role.RemoveRange(dbContext.Role);
        //    dbContext.Friendships.RemoveRange(dbContext.Friendships);
        //    dbContext.Track.RemoveRange(dbContext.Track);
        //    dbContext.TrackAdmin.RemoveRange(dbContext.TrackAdmin);
        //    dbContext.UserRoles.RemoveRange(dbContext.UserRoles);
        //    dbContext.SaveChanges();

        //    var karter = ConstValues.SelfKarter;
        //    dbContext.Karter.Add(karter);

        //    var role = new Role { Id = 1, Name = "Admin" };
        //    dbContext.Role.Add(role);

        //    await dbContext.SaveChangesAsync();

        //    dbContext.UserRoles.Add(new UserRoles
        //    {
        //        KarterId = karter.Id,
        //        RoleId = role.Id
        //    });

        //    await dbContext.SaveChangesAsync();
        //}


        //public async Task SeedTrackVerified()
        //{
        //    using var scope = Services.CreateScope();
        //    var dbContext = scope.ServiceProvider.GetRequiredService<GoKartUniteContext>();

        //    dbContext.Track.Add(ConstValues.VerifiedTrack);
        //    await dbContext.SaveChangesAsync();

        //}

        //public async Task SeedTrackNonVerified()
        //{
        //    using var scope = Services.CreateScope();
        //    var dbContext = scope.ServiceProvider.GetRequiredService<GoKartUniteContext>();

        //    dbContext.Track.Add(ConstValues.NonVerifiedTrack);
        //    await dbContext.SaveChangesAsync();
        //}
    }
}
