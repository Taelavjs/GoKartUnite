using GoKartUnite.Models;
using GoKartUnite.ViewModel;

namespace GoKartUnite.Interfaces
{
    public interface IKarterHandler
    {
        Task<Karter> GetUser(int id);
        Task<Karter> GetUser(string name);
        Task DeleteUser(Karter karter);
        Task DeleteUser(int id);
        Task<bool> SendFriendRequest(Karter sentBy, Karter requestedTo);
        Task<List<Karter>> GetAllUsers(bool fetchTracks, string? track = null, int pageNo = 0, int usersPerPage = 3, SortKartersBy sort = SortKartersBy.Alphabetically);
        Task<int> GetNumberOfUserPages(string track, int usersPerPage = 3);
        Task<List<Karter>> GetAllUsersByTrackId(int id);
        Task CreateUser(Karter karter, string email);
        Task<Karter> GetUserByGoogleId(string GoogleId, bool withTrack = false);
        Task<List<KarterView>> KarterModelToView(List<Karter> karters);
        Task<KarterView> KarterModelToView(Karter karter);
        Task UpdateUser(string nameIdentifier, KarterView kv);
    }
}
