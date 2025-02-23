using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoKartUnite;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using GoKartUnite.Data;
using Microsoft.EntityFrameworkCore;
using GoKartUnite.Models;
using FluentAssertions;


namespace GoKartUnite.IntegrationTests
{
    public class UniteWebFactory
    {
        public async Task testExample()
        {
            var factory = new WebApplicationFactory<GoKartUnite.Program>()
                .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll(typeof(GoKartUniteContext));

                    services.AddDbContext<GoKartUniteContext>(options =>
                    {
                        options.UseInMemoryDatabase("test");
                    });
                });
            });

            using (var scope = factory.Services.CreateScope())
            {
                var scopService = scope.ServiceProvider;
                var dbContext = scopService.GetRequiredService<GoKartUniteContext>();

                dbContext.Karter.Add(new Models.Karter()
                {
                    Name = "name1",
                    YearsExperience = 10
                });

                dbContext.SaveChanges();

                var client = factory.CreateClient();
                var response = await client.GetAsync("/karterHome/Index");
                var result = await response.Content.ReadAsStringAsync();

                response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            };
        }
    }
}
//public int Id { get; set; }
//public string? Email { get; set; }
//public string? NameIdentifier { get; set; }



//public string Name { get; set; }
//public int YearsExperience { get; set; }

//// Relationships
//public int? TrackId { get; set; }
//public Track? Track { get; set; }

//public virtual ICollection<UserRoles>? UserRoles { get; set; }

//public virtual ICollection<Friendships>? Friendships { get; set; }
//public virtual ICollection<BlogPost>? BlogPosts { get; set; }
//public ICollection<BlogNotifications>? Notification { get; set; }