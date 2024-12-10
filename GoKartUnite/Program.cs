using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GoKartUnite.Data;
using System.Net.WebSockets;
using GoKartUnite.SingletonServices;
using GoKartUnite.SignalRFiles;


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
            app.MapHub<ChatHub>("/chatHub");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            app.Run();
        }


    }
}
