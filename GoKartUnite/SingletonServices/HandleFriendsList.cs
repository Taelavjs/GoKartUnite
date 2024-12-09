using GoKartUnite.Data;
using System.Net.WebSockets;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GoKartUnite.SingletonServices
{
    public class HandleFriendsList
    {
        private readonly List<ClientHandler> _clients = new();

        private readonly GoKartUniteContext _context;
        public HandleFriendsList(GoKartUniteContext context)
        {
            _context = context;
        }

        public void AddSocket(WebSocket webSocket, TaskCompletionSource<object> socketFinishedTcs)
        {
            var clientHandler = new ClientHandler(webSocket, socketFinishedTcs);

            lock (_clients)
            {
                _clients.Add(clientHandler);
            }
            _ = clientHandler.ProcessClientAsync(() =>
            {
                lock (_clients)
                {
                    _clients.Remove(clientHandler);
                }
            });
        }

        public class ClientHandler
        {
            private readonly WebSocket _webSocket;
            private readonly TaskCompletionSource<object> _socketFinishedTcs;
            private readonly TaskCompletionSource<string> _incomingMessageTcs = new();
            private readonly TaskCompletionSource<string> _notificationTcs = new();

            public ClientHandler(WebSocket webSocket, TaskCompletionSource<object> socketFinishedTcs)
            {
                _webSocket = webSocket;
                _socketFinishedTcs = socketFinishedTcs;
            }


            public async Task ProcessClientAsync(Action onComplete) {
                var receievedTask = ReceiveMessagesAsync();
                var sentTask = SendMessagesAsync();

                await Task.WhenAny(receievedTask);

                onComplete();
            }




            private async Task ReceiveMessagesAsync()
            {
                var buffer = new byte[1024 * 4];
                try
                {
                    while(_webSocket.State == WebSocketState.Open)
                    {
                        var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                            break;
                        }

                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            System.Diagnostics.Debug.WriteLine($"Received: {message}");
                            _incomingMessageTcs.TrySetResult(message);
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in ReceiveMessagesAsync: {ex.Message}");
                }


            }

            private async Task SendMessagesAsync()
            {
                var messageBuffer = Encoding.UTF8.GetBytes("Hello");
                try
                {
                    while (_webSocket.State == WebSocketState.Open)
                    {
                        await _webSocket.SendAsync(
                            new ArraySegment<byte>(messageBuffer),
                            WebSocketMessageType.Text,
                            true, 
                            CancellationToken.None
                        );

                        await Task.Delay(1000);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in ReceiveMessagesAsync: {ex.Message}");
                }
            }
        }
    }
}
