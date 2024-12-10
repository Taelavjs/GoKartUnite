using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GoKartUnite.Data;
using System.Net.WebSockets;
using GoKartUnite.SingletonServices;
namespace GoKartUnite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<GoKartUniteContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("GoKartUniteContext") ?? throw new InvalidOperationException("Connection string 'GoKartUniteContext' not found.")));

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSingleton<IHandleFriendsList, HandleFriendsList>();
            var app = builder.Build();
            app.UseWebSockets();
            var webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromMinutes(2)
            };

            app.UseWebSockets(webSocketOptions);
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            // WEBSOCKETS +_+_+_+_+_+_+_+_+_+_+_+_+_+_
            app.Map("/ws", wsApp =>
            {
                wsApp.Run(async context =>
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        var socketFinishedTcs = new TaskCompletionSource<object>();
                        var handleFriendsList = context.RequestServices.GetRequiredService<IHandleFriendsList>();
                        handleFriendsList.AddSocket(webSocket, socketFinishedTcs);
                        await socketFinishedTcs.Task;
                    }
                    else
                    {
                        context.Response.StatusCode = 400; // Bad Request
                        await context.Response.WriteAsync("Expected a WebSocket request.");
                    }
                });
            });
            // +_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }


    }
}
