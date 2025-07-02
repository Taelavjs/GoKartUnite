using GoKartUnite;
using GoKartUnite.Data;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnitTesting.HelpersTut;

namespace UnitTesting.ControllerTests.Bases
{
    public abstract class BaseControllerClass : IAsyncLifetime
    {
        protected readonly TestServer<Program> _factory;
        protected HttpClient _client;
        protected IServiceScope _scope;
        protected GoKartUniteContext _dbContext;
        protected static Checkpoint _checkpoint;
        protected string _connectionString = "Server=(localdb)\\mssqllocaldb;Database=GoKartUniteTestDb;Trusted_Connection=True;MultipleActiveResultSets=true";

        protected int userId = 0;
        protected int otherUserId = 0;

        public BaseControllerClass(
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

            var ids = Utilities.InitializeKarterDbForTests(_dbContext);
            (userId, otherUserId) = (ids[0], ids[1]);
            _client = await HttpClientExtensions.CreateAuthedClient(_factory);
        }

        public async Task DisposeAsync()
        {
            await _dbContext.DisposeAsync();
            _scope.Dispose();
            _client.Dispose();
        }

    }
}
