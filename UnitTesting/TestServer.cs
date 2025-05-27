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
    }
}
