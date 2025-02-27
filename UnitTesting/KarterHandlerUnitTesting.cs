using GoKartUnite.Data;
using GoKartUnite.Handlers;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting
{
    public class KarterHandlerUnitTesting
    {
        private readonly GoKartUniteContext _context;
        private readonly KarterHandler _karterHandler;

        private int KartersCreatedToTest = 3;
        private string testingKartersName = "Test";

        public KarterHandlerUnitTesting()
        {

            var options = new DbContextOptionsBuilder<GoKartUniteContext>()
                          .UseInMemoryDatabase(databaseName: "KarterHandlerDb")
                          .Options;

            _context = new GoKartUniteContext(options);
            _karterHandler = new KarterHandler(_context);
        }

        public async Task ResetDatabase()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            for (int i = 0; i < KartersCreatedToTest; i++)
            {
                Karter k = Helpers.GenerateValidKarters(testingKartersName, i + 1);
                _context.Karter.Add(k);

            }

            await _context.SaveChangesAsync();
        }


        [Fact]
        public async Task GetUser_ByID_ValidIdRequested()
        {
            await ResetDatabase();

            Karter result = await _karterHandler.GetUser(1);

            Assert.NotNull(result);
            Assert.Contains("1", result.Name);
            result = await _karterHandler.GetUser(2);

            Assert.NotNull(result);
            Assert.Contains("2", result.Name);
        }

        [Fact]
        public async Task GetUser_ByID_InvalidIdRequested()
        {
            await ResetDatabase();

            Karter result = await _karterHandler.GetUser(KartersCreatedToTest * -1);
            Assert.Null(result);
            result = await _karterHandler.GetUser(0);
            Assert.Null(result);
            result = await _karterHandler.GetUser(999999);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUser_ByName_ValidNameRequested()
        {
            await ResetDatabase();

            Karter result = await _karterHandler.GetUser(testingKartersName + "1");
            Assert.NotNull(result);
            Assert.Equal(result.Name, testingKartersName + "1");

            result = await _karterHandler.GetUser(testingKartersName + "2");
            Assert.NotNull(result);
            Assert.Equal(result.Name, testingKartersName + "2");
        }

        [Fact]
        public async Task GetUser_ByName_InvalidNameRequested()
        {
            await ResetDatabase();

            Karter result = await _karterHandler.GetUser(testingKartersName + "Nobody");
            Assert.Null(result);

            result = await _karterHandler.GetUser(testingKartersName + "");
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteUser_ValidUserDeletionRequests()
        {
            await ResetDatabase();

            Karter k = _context.Karter.ToList().First();

            await _karterHandler.DeleteUser(k);

            foreach (var l in _context.Karter.ToList())
            {
                Assert.True(l.Name != k.Name);
            }
        }

        [Fact]
        public async Task DeleteUser_InvalidUserDeletionRequests()
        {
            await ResetDatabase();

            Karter k = Helpers.GenerateValidKarters("nobody");

            await _karterHandler.DeleteUser(k);

            Assert.Equal(_context.Karter.Count(), KartersCreatedToTest);
        }

        [Fact]
        public async Task SendFriendRequestByName()
        {
            await ResetDatabase();
            Karter kSender = _context.Karter.ToList().First();
            Karter kReceiver = _context.Karter.ToList().Last();

            bool res = await _karterHandler.SendFriendRequest(kSender, kReceiver);

            Assert.True(res);

            Friendships fs = _context.Friendships.Single(x => x.KarterFirstId == kSender.Id || x.KarterSecondId == kReceiver.Id);
            Assert.Equal(fs.requestedByInt, kSender.Id);
            Assert.False(fs.accepted);
            Assert.True(fs.KarterFirstId < fs.KarterSecondId);
        }

        [Fact]
        public async Task SendFriendRequestByName_RecordAlreadyExists()
        {
            await ResetDatabase();
            Karter kSender = _context.Karter.ToList().First();
            Karter kReceiver = _context.Karter.ToList().Last();

            bool res = await _karterHandler.SendFriendRequest(kSender, kReceiver);
            res = await _karterHandler.SendFriendRequest(kSender, kReceiver);

            Assert.False(res);

            Friendships fs = _context.Friendships.Single(x => x.KarterFirstId == kSender.Id || x.KarterSecondId == kReceiver.Id);
            Assert.Equal(fs.requestedByInt, kSender.Id);
            Assert.False(fs.accepted);
            Assert.True(fs.KarterFirstId < fs.KarterSecondId);
        }

        [Fact]
        public async Task SendFriendRequestByName_InvalidKartersPassedIn()
        {
            await ResetDatabase();
            Karter kSender = _context.Karter.ToList().First();

            bool res = await _karterHandler.SendFriendRequest(kSender, null);

            Assert.False(res);

            Friendships fs = _context.Friendships.SingleOrDefault(x => x.KarterFirstId == kSender.Id);
            Assert.Null(fs);
        }

        [Fact]
        public async Task KarterModelToView_SingleKarter()
        {
            Karter k = Helpers.GenerateValidKarters("TestDummy", 1);
            KarterView kv = await _karterHandler.KarterModelToView(k);

            Assert.NotNull(kv);
            Assert.Equal(k.Name, kv.Name);
            Assert.Equal(k.YearsExperience, kv.YearsExperience);
        }

        [Fact]
        public async Task KarterModelToView_ListOfKarters()
        {
            List<Karter> karters = new List<Karter>
            {
                Helpers.GenerateValidKarters("TestDummy1", 1),
                Helpers.GenerateValidKarters("TestDummy2", 2),
                Helpers.GenerateValidKarters("TestDummy3", 3)
            };

            List<KarterView> kvs = await _karterHandler.KarterModelToView(karters);

            Assert.NotNull(kvs);
            Assert.Equal(karters.Count, kvs.Count);

            for (int i = 0; i < karters.Count; i++)
            {
                Assert.Equal(karters[i].Name, kvs[i].Name);
                Assert.Equal(karters[i].YearsExperience, kvs[i].YearsExperience);
                Assert.Equal(karters[i].Track, kvs[i].LocalTrack);
            }
        }

    }
}
