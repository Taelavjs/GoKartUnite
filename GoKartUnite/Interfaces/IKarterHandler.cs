using GoKartUnite.DataFilterOptions;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;
using System.Security.Claims;

namespace GoKartUnite.Interfaces
{
    public interface IKarterHandler
    {
        Task<Karter> GetUser(int id, KarterGetAllUsersFilter? options = null);
        Task<Karter> GetUser(string name, KarterGetAllUsersFilter? options = null);
        Task DeleteUser(Karter karter);
        Task DeleteUser(int id);
        Task<bool> SendFriendRequest(Karter sentBy, Karter requestedTo);
        Task<List<Karter>> GetAllUsers(KarterGetAllUsersFilter? options = null);
        Task<int> GetNumberOfUserPages(string track, int usersPerPage = 3);
        Task<List<Karter>> GetAllUsersByTrackId(int id, KarterGetAllUsersFilter? options = null);
        Task CreateUser(Karter karter, string email);
        Task<Karter> GetUserByGoogleId(string GoogleId, bool withTrack = false);
        Task<List<KarterView>> KarterModelToView(List<Karter> karters);
        Task<KarterView> KarterModelToView(Karter karter);
        Task UpdateUser(string nameIdentifier, KarterView kv);
        Task<string> GetCurrentUserNameIdentifier(ClaimsPrincipal User);

    }
}
