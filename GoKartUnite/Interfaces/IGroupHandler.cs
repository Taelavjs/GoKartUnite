using GoKartUnite.Models;
using GoKartUnite.Models.Groups;
using GoKartUnite.ViewModel;

namespace GoKartUnite.Interfaces
{
    public interface IGroupHandler
    {
        Task CreateNewGroup(ListedGroupView group, Karter k);
        Task<List<ListedGroupView>> GetAllGroups(Karter k);
        Task JoinGroup(int groupId, Karter karter);
        Task LeaveGroup(int groupId, Karter karter);
        Task<List<GroupMessageView>> MessagesToDTO(List<GroupMessage> comments);
        Task<List<GroupView>> ToDTOList(List<Group> group);
        Task<GroupView> ToDTO(Group group);
        Task<Group> GetGroupById(int groupId);

    }
}
