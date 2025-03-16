using GoKartUnite.Models;
using GoKartUnite.ViewModel;

namespace GoKartUnite.Interfaces
{
    public interface IGroupHandler
    {
        Task CreateNewGroup(GroupView group, Karter k);
        Task<List<GroupView>> GetAllGroups(Karter k);
        Task JoinGroup(int groupId, Karter karter);
        Task LeaveGroup(int groupId, Karter karter);


    }
}
