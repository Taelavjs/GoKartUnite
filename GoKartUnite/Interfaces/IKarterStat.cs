using GoKartUnite.Models;
using GoKartUnite.ViewModel;

namespace GoKartUnite.Interfaces
{
    public interface IKarterStat
    {
        Task CreateStatRecord(KarterStatViewModel ViewModel, Track track, Karter karter, TimeSpan BestLapFormatted);

    }
}
