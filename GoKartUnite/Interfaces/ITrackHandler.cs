using GoKartUnite.Models;
using GoKartUnite.ViewModel;

namespace GoKartUnite.Interfaces
{
    public interface ITrackHandler
    {
        Task<Track?> GetTrack(int id, bool? getKarters);
        Task<bool> AddTrack(Track track);
        Task UpdateTrack(int id);
        Task<bool> DeleteTrack(int id);
        Task<List<Track>> GetAllTracks();
        Task<List<string>> GetAllTrackTitles();
        Task<List<Track>> GetTracksByTitle(string title, List<Locations>? location = null);
        Task<Track> GetSingleTrackByTitle(string title);
        Task<List<TrackView>> ModelToView(List<Track> tracks);
        Task<TrackView> ModelToView(Track track);
        Task<int> GetTrackIdByTitle(string g);
        Task<Track> GetTrackById(int id);
    }
}
