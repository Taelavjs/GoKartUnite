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
            builder.Services.AddTransient<RelationshipHandler>();
            builder.Services.AddTransient<KarterHandler>();
            builder.Services.AddTransient<TrackHandler>();
            builder.Services.AddTransient<BlogHandler>();
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
                    });
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSingleton<IHandleFriendsList, HandleFriendsList>();
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<GoKartUniteContext>();
                dummyDbData(context);
            }
            app.Run();
        }

        private static void dummyDbData(GoKartUniteContext context)
        {
            if (context.BlogPosts.Count() > 100) return;
            for (int i = 0; i < 100; i++)
            {
                BlogPost post = new BlogPost
                {
                    AuthorId = 4032,
                    Title = "Test",
                    Descripttion = "Test"
                };

                context.BlogPosts.Add(post);
                context.SaveChanges();
            }


        }


    }
}
