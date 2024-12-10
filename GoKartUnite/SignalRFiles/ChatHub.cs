using GoKartUnite.Handlers;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace GoKartUnite.SignalRFiles
{
    public class ChatHub : Hub
    {
        private readonly RelationshipHandler _relationships;
        private ConcurrentDictionary<string, string> connectedGroups;
        public ChatHub(RelationshipHandler relationship )
        {
            _relationships = relationship;
            connectedGroups = new ConcurrentDictionary<string, string>();
        }

        public async Task SendMessage(string user, string message)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"ChatHub instance created: {this.GetHashCode()}"); // Unique instance ID
                string usn = connectedGroups.GetValueOrDefault(Context.ConnectionId);
                await Clients.Group(usn).SendAsync("ReceiveMessage", await _relationships.getFriends(1031));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendMessage: {ex.Message}");
                throw; // Optionally rethrow or handle it gracefully
            }
        }

        public override Task OnConnectedAsync()
        {
            var username = Context.GetHttpContext().Request.Query["username"];
            System.Diagnostics.Debug.WriteLine($"ChatHub instance created: {this.GetHashCode()}"); // Unique instance ID
            connectedGroups.AddOrUpdate(
                Context.ConnectionId,
                username,
                (key, oldValue) => username
                );

            return base.OnConnectedAsync();
        }
    }
}