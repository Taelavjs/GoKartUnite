using System.Net.WebSockets;
using static GoKartUnite.SingletonServices.HandleFriendsList;

namespace GoKartUnite.SingletonServices
{
    public interface IHandleFriendsList
    {
        public void AddSocket(
            WebSocket webSocket, 
            TaskCompletionSource<object> socketFinishedTcs
        );


    }
}
