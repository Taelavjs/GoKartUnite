using GoKartUnite.Data;
using GoKartUnite.Handlers;
using GoKartUnite.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting
{
    public class AdminHandlerUnitTest
    {
        private readonly GoKartUniteContext _context;
        private readonly RoleHandler _roleHandler;

        public AdminHandlerUnitTest()
        {

            var options = new DbContextOptionsBuilder<GoKartUniteContext>()
                          .UseInMemoryDatabase(databaseName: "AdminDbName")
                          .Options;

            _context = new GoKartUniteContext(options);
            _roleHandler = new RoleHandler(_context);
        }

        public async Task ResetSetupScenario()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            var karters = Enumerable.Range(1, 3)
                .Select(i => Helpers.GenerateValidKarters("Test", i))
                .ToList();

            var role1 = new Role
            {
                Name = "Role1"
            };

            var role2 = new Role
            {
                Name = "Role2"
            };

            _context.Karter.AddRange(karters);

            await _context.Karter.AddRangeAsync(karters);
            await _context.Role.AddRangeAsync(role1, role2);
            await _context.SaveChangesAsync();
        }

        private async Task AddAdminUsersToTracks()
        {
            _context.Track.Add(new Track { Title = "Buckmore", Location = Locations.NORTHEAST });
            _context.SaveChanges();

            _context.TrackAdmin.Add(new TrackAdmins { KarterId = 1, ManagedTrack = _context.Track.Single(x => x.Title == "Buckmore"), TrackId = 1 });
            _context.SaveChanges();
        }
        private async Task CreateUnverifiedTracks(int count = 5)
        {
            var dummyTracks = Enumerable.Range(1, count).Select(i => new Track
            {
                Title = $"TestTrack_{i}",
                Location = (Locations)(i % Enum.GetValues(typeof(Locations)).Length),
                IsVerifiedByAdmin = false
            }).ToList();

            await _context.Track.AddRangeAsync(dummyTracks);
            await _context.SaveChangesAsync();
        }


        [Fact]
        public async Task AddRoleToUser_ValidRoleAndUser()
        {
            await ResetSetupScenario();

            int userId = 1;
            string roleName = "Role1";

            await _roleHandler.AddRoleToUser(userId, roleName);

            UserRoles userRole = _context.UserRoles.SingleOrDefault(x => x.KarterId == userId);
            Assert.NotNull(userRole);

            var listRoles = _context.Role.ToList();
            Assert.True(userRole.RoleId == 1);
        }

        [Fact]
        public async Task AddRoleToUser_InvalidUserId()
        {
            await ResetSetupScenario();
            int userId = -1;
            string roleName = "Role1";

            await _roleHandler.AddRoleToUser(userId, roleName);

            UserRoles userRole = _context.UserRoles.SingleOrDefault(x => x.KarterId == userId);
            Assert.Null(userRole);
        }

        [Fact]
        public async Task AddRoleToUser_InvalidRoleName()
        {
            await ResetSetupScenario();
            int userId = 1;
            string roleName = "Roleo1";

            await _roleHandler.AddRoleToUser(userId, roleName);

            UserRoles userRole = _context.UserRoles.SingleOrDefault(x => x.KarterId == userId);
            Assert.Null(userRole);
        }

        [Fact]
        public async Task RemoveRoleFromUser_ValidInputs()
        {
            await ResetSetupScenario();
            _context.UserRoles.Add(new UserRoles { KarterId = 1, RoleId = 2 });
            _context.UserRoles.Add(new UserRoles { KarterId = 1, RoleId = 1 });
            _context.SaveChanges();


            await _roleHandler.RemoveUserRole(1, "Role1");

            var allUserRoles = _context.UserRoles.Include(x => x.Role).Where(x => x.KarterId == 1).ToList();
            Assert.True(allUserRoles.Count() == 1);
            Assert.True(allUserRoles[0].Role.Name == "Role2", "Incorrect Role Deleted");
        }

        [Fact]
        public async Task RemoveRoleFromUser_InValidKarterId()
        {
            await ResetSetupScenario();
            _context.UserRoles.Add(new UserRoles { KarterId = 1, RoleId = 2 });
            _context.UserRoles.Add(new UserRoles { KarterId = 1, RoleId = 1 });
            _context.SaveChanges();


            await _roleHandler.RemoveUserRole(-1, "Role1");

            var allUserRoles = _context.UserRoles.Where(x => x.KarterId == 1).ToList();
            Assert.True(allUserRoles.Count() == 2, "Incorrect Role has been deleted");
        }

        [Fact]
        public async Task RemoveRoleFromUser_InValidRoleName()
        {
            await ResetSetupScenario();
            _context.UserRoles.Add(new UserRoles { KarterId = 1, RoleId = 2 });
            _context.UserRoles.Add(new UserRoles { KarterId = 1, RoleId = 1 });
            _context.SaveChanges();


            await _roleHandler.RemoveUserRole(1, "Role");

            var allUserRoles = _context.UserRoles.Where(x => x.KarterId == 1).ToList();
            Assert.True(allUserRoles.Count() == 2, "Incorrect Role has been deleted");
        }

        [Fact]
        public async Task IsAdminAtTrack_ValidInputsWithAdminBeingTrue()
        {
            await ResetSetupScenario();
            await AddAdminUsersToTracks();

            bool result = await _roleHandler.IsAdminAtTrack("Buckmore", 1);

            Assert.True(result);
        }

        [Fact]
        public async Task IsAdminAtTrack_ValidInputsButFalse()
        {
            await ResetSetupScenario();
            await AddAdminUsersToTracks();

            bool result = await _roleHandler.IsAdminAtTrack("Buckmore", 2);

            Assert.False(result);
        }

        [Fact]
        public async Task IsAdminAtTrack_InvalidInputs()
        {
            await ResetSetupScenario();
            await AddAdminUsersToTracks();

            bool result = await _roleHandler.IsAdminAtTrack("gb", -2);

            Assert.False(result);
        }


        //                                                                  METHOD NOT CREATED YET
        //[Fact]
        //public async Task VerifyTrack_ById_ValidIdGiven()
        //{
        //    await ResetSetupScenario();
        //    await CreateUnverifiedTracks();
        //    int id = 1;
        //    bool success = await _trackHandler.VerifyTrack(id);
        //
        //    List<Track> tracksInDb = await _context.Track.ToListAsync();
        //
        //    Assert.True(success);
        //    Assert.True(tracksInDb.Where(x => x.Id == id).First().IsVerifiedByAdmin == true);
        //    var otherTrack = tracksInDb.FirstOrDefault(x => x.Id != id);
        //    Assert.NotNull(otherTrack);
        //    Assert.True(otherTrack.IsVerifiedByAdmin, "Expected other tracks to be not verified.");
        //}
        //
        //                                                                  METHOD NOT CREATED YET

    }
}
