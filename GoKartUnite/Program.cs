using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GoKartUnite.Data;
using System.Net.WebSockets;
using GoKartUnite.SingletonServices;
using GoKartUnite.Handlers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using GoKartUnite.Models;
using X.PagedList;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;
using System.Diagnostics;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using GoKartUnite.Interfaces;


namespace GoKartUnite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<GoKartUniteContext>(options =>
                            options.UseSqlServer(builder.Configuration.GetConnectionString("GoKartUniteContext") ?? throw new InvalidOperationException("Connection string 'GoKartUniteContext' not found.")));
            builder.Services.AddSignalR();
            builder.Services.AddScoped<IRelationshipHandler, RelationshipHandler>();
            builder.Services.AddScoped<IKarterHandler, KarterHandler>();
            builder.Services.AddScoped<ITrackHandler, TrackHandler>();
            builder.Services.AddScoped<IBlogHandler, BlogHandler>();
            builder.Services.AddScoped<IFollowerHandler, FollowerHandler>();
            builder.Services.AddScoped<INotificationHandler, NotificationHandler>();
            builder.Services.AddScoped<IRoleHandler, RoleHandler>();
            builder.Services.AddScoped<IKarterStat, KarterStat>();
            builder.Services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            builder.Services.AddRateLimiter(_ => _
                .AddFixedWindowLimiter(policyName: "fixed", options =>
                {
                    options.PermitLimit = 4;
                    options.Window = TimeSpan.FromSeconds(12);
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 2;
                }));

            builder.Services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 8,
                            Window = TimeSpan.FromSeconds(2),
                            SegmentsPerWindow = 4,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 2
                        }));

                options.RejectionStatusCode = 429;
            });

            builder.Services.AddRateLimiter(_ => _
            .AddSlidingWindowLimiter(policyName: "slidingPolicy", options =>
            {
                options.PermitLimit = 4;
                options.Window = TimeSpan.FromSeconds(4);
                options.SegmentsPerWindow = 4;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = 2;
            }));

            var services = builder.Services;
            var configuration = builder.Configuration;
            var secretFilePath = Path.Combine(Directory.GetCurrentDirectory(), "secrets.txt");
            var secretsContent = File.ReadAllText(secretFilePath);
            var secretPairs = secretsContent.Split(',');
            foreach (var secret in secretPairs)
            {
                var keyValue = secret.Split('=');
                if (keyValue.Length == 2)
                {
                    builder.Configuration[keyValue[0].Trim()] = keyValue[1].Trim();
                }
            }
            builder.Logging.AddConsole();

            builder.Services
                    .AddAuthentication(options =>
                    {
                        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                    })
                    .AddCookie() // Cookie-based authentication for maintaining login session
                    .AddGoogle(googleOptions =>
                    {
                        googleOptions.ClientId = configuration["ClientId"];
                        googleOptions.ClientSecret = configuration["ClientSecret"];
                        googleOptions.Events.OnCreatingTicket = ctx =>
                        {
                            var dbContext = ctx.HttpContext.RequestServices.GetRequiredService<GoKartUniteContext>();
                            var email = ctx.Principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                            var NameIdentifier = ctx.Principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                            var karter = dbContext.Karter.Include(k => k.UserRoles).FirstOrDefault(k => k.Email == email);
                            var claims = new List<Claim>();
                            if (karter != null)
                            {
                                if (karter.UserRoles == null)
                                {
                                    ctx.Principal.AddIdentity(new ClaimsIdentity(claims));
                                    return Task.CompletedTask;
                                }
                                foreach (UserRoles role in karter.UserRoles)
                                {
                                    Role singleRole = dbContext.Role.FirstOrDefault(r => r.Id == role.RoleId);
                                    claims.Add(new Claim(ClaimTypes.Role, singleRole.Name));
                                }
                            }

                            ctx.Principal.AddIdentity(new ClaimsIdentity(claims));
                            return Task.CompletedTask;

                        };
                    });
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            var app = builder.Build();

            var webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromMinutes(2)
            };

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            // WEBSOCKETS +_+_+_+_+_+_+_+_+_+_+_+_+_+_

            // +_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+
            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseRateLimiter();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<GoKartUniteContext>();
                //dummyDbData(context);
                //dummyDbComments(context);
            }
            app.Run();
        }

        private static void dummyDbData(GoKartUniteContext context)
        {
            if (context.BlogPosts.Count() > 10000) return;
            for (int i = 0; i < 15; i++)
            {
                BlogPost post = new BlogPost
                {
                    AuthorId = 4041,
                    Title = "NewestPost",
                    Description = "This is for NewBuckmore",
                    TaggedTrackId = 1014
                };

                context.BlogPosts.Add(post);
                BlogPost post2 = new BlogPost
                {
                    AuthorId = 4033,
                    Title = "NewestPost",
                    Description = "This is for Alt",
                    TaggedTrackId = 1016
                };

                context.BlogPosts.Add(post2);
                context.SaveChanges();
            }

        }

        private static void dummyDbComments(GoKartUniteContext context)
        {
            if (context.Comments.Count() > 400) return;

            for (int i = 0; i < 100; i++)
            {
                Comment comment = new Comment
                {
                    Text = "This is testData",
                    AuthorId = 4033,
                    BlogPostId = 3026
                };

                context.Comments.Add(comment);
                context.SaveChanges();

            }

        }
    }
}
