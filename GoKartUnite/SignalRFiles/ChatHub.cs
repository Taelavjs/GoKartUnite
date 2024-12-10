using Microsoft.AspNetCore.SignalR;

namespace GoKartUnite.SignalRFiles
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            try
            {
                await Clients.All.SendAsync("ReceiveMessage", user, message);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in SendMessage: {ex.Message}");
                throw; // Optionally rethrow or handle it gracefully
            }
        }
    }
}