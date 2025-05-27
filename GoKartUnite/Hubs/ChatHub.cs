using GoKartUnite.Handlers;
using GoKartUnite.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Web.Mvc;
namespace GoKartUnite.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private static IGroupHandler _group;
        private static IKarterHandler _karter;
        public ChatHub(IGroupHandler group, IKarterHandler karter)
        {
            _group = group;
            _karter = karter;
        }
        private async Task<bool> IsUserInGroup(string groupId)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var kid = await _karter.GetUserByGoogleId(userId);
            var allUserGroupIds = await _group.GetAllUserGroupIds(kid.Id);
            if (!allUserGroupIds.Any(x => x.Equals(int.Parse(groupId))))
            {
                await Clients.Caller.SendAsync("ReceiveSystemMessage",
                    $"You are not authorized to join group {groupId}.");
                Context.Abort();

                return false;
            }
            return true;
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("ReceiveSystemMessage",
                                        $"{Context.UserIdentifier} joined.");
            await base.OnConnectedAsync();
        }

        public async Task AddToGroup(string groupId)
        {
            if (!await IsUserInGroup(groupId)) return;
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);

            await Clients.Group(groupId).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupId}.");
        }

        public async Task RemoveFromGroup(string groupId)
        {
            if (!await IsUserInGroup(groupId)) return;
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);

            await Clients.Group(groupId).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupId}.");
        }

        public async Task SendMessageToAllInGroup(string groupId, string messageContent)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var kid = await _karter.GetUserByGoogleId(userId);
            bool res = await _group.CreateUserMessageInGroup(int.Parse(groupId), messageContent, kid);

            if (res)
            {
                await Clients.Group(groupId).SendAsync("MessageIncoming", kid.Name, messageContent);
            }


        }
    }
}
