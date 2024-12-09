using Azure.Core;
using GoKartUnite.Data;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Text;

namespace GoKartUnite.Controllers
{
    public class WebsocketController : ControllerBase
    {

        private readonly GoKartUniteContext _context;
        public WebsocketController(GoKartUniteContext context)
        {
            _context = context;
        }

        [Route("/ws")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await Echo(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private async Task Echo(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!receiveResult.CloseStatus.HasValue)
            {


                int numFriends = await GetNumberOfFriendsAsync();
                var buffer2 = Encoding.UTF8.GetBytes($"{numFriends}");

                await webSocket.SendAsync(
                    new ArraySegment<byte>(buffer2),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);
        }

        private async Task<int> GetNumberOfFriendsAsync()
        {
            int numFriends = await _context.Friendships
                .CountAsync(k => k.KarterFirstId == 1031 || k.KarterSecondId == 1031);

            return numFriends;
        }
    }
}
