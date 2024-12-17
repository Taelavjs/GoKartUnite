using GoKartUnite.Handlers;
using GoKartUnite.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Claims;

namespace GoKartUnite.SignalRFiles
{
    public class ChatHub : Hub
    {
        private readonly RelationshipHandler _relationships;
        private readonly KarterHandler _karters;
        private static ConcurrentDictionary<string, string> connectedGroups = new ConcurrentDictionary<string, string>();
        public ChatHub(RelationshipHandler relationship, KarterHandler karter )
        {
            _relationships = relationship;
            _karters = karter;
        }

        public async Task SendMessage(string user, string message)
        {
            try
            {
                
                System.Diagnostics.Debug.WriteLine($"ChatHub instance created: {this.GetHashCode()}"); // Unique instance ID
                string usn = connectedGroups.GetValueOrDefault(Context.ConnectionId);
                string usr = Context.GetHttpContext().User.Claims.FirstOrDefault(k => k.Type == ClaimTypes.NameIdentifier).Value;
                Karter src = await _karters.getUserByGoogleId(usr);
                int friendsNo = await _relationships.getFriends(src.Id);
                await Clients.Group(usn).SendAsync("ReceiveMessage", friendsNo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendMessage: {ex.Message}");
                throw; // Optionally rethrow or handle it gracefully
            }
        }

        public override async Task OnConnectedAsync()
        {
            var username = Context.GetHttpContext().Request.Query["username"];
            await Context.GetHttpContext().AuthenticateAsync();
            System.Diagnostics.Debug.WriteLine($"ChatHub instance created: {this.GetHashCode()}"); // Unique instance ID
            connectedGroups.AddOrUpdate(
                Context.ConnectionId,
                username,
                (key, oldValue) => username
                );

            await Groups.AddToGroupAsync(Context.ConnectionId, username);
            await base.OnConnectedAsync();
            SendMessage(username, "");
            return;
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Trace.WriteLine(Context.ConnectionId + " - disconnected");

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.GetHttpContext().Request.Query["username"]);
            string res;
            connectedGroups.Remove(Context.ConnectionId, out res);

            await base.OnDisconnectedAsync(exception);
        }
    }
}