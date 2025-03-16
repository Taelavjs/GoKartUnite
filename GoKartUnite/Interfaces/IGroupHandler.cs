using GoKartUnite.Models;
using GoKartUnite.ViewModel;

namespace GoKartUnite.Interfaces
{
    public interface IGroupHandler
    {
        Task CreateNewGroup(ListedGroupView group, Karter k);
        Task<List<ListedGroupView>> GetAllGroups(Karter k);
        Task JoinGroup(int groupId, Karter karter);
        Task LeaveGroup(int groupId, Karter karter);


    }
}
