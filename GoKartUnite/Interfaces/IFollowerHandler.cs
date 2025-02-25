using GoKartUnite.Models;

namespace GoKartUnite.Interfaces
{
    public interface IFollowerHandler
    {
        Task CreateFollow(int karterId, int trackId);
        Task<bool> DoesUserFollow(int karterId, int trackId);
        Task<List<FollowTrack>> GetUsersFollowList(int karterId);
        Task<List<int>> AllUserIdsWhoFollowTrack(int track);
        Task<List<string>> GetAllFollowedTracks(int userId);
    }
}
