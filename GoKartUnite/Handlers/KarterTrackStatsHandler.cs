using GoKartUnite.Data;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;

namespace GoKartUnite.Handlers
{
    public class KarterTrackStatsHandler
    {
        private readonly GoKartUniteContext _context;
        public KarterTrackStatsHandler(GoKartUniteContext context, KarterHandler karterHandler, TrackHandler trackHandler)
        {

        }

        public async Task CreateStatRecord(KarterTrackStatsViewModel ViewModel, Track track)
        {
            if (track == null)
            {
                return;
            }

            KarterTrackStats model = new KarterTrackStats
            {
                RaceName = ViewModel.RaceName,
                BestLapTime = ViewModel.BestLapTime,
                DateOnlyRecorded = ViewModel.DateOnlyRecorded,
                isChampionshipRace = ViewModel.isChampionshipRace,
                RaceLength = ViewModel.RaceLength,
                TEMPERATURE = ViewModel.TEMPERATURE,
                WEATHERSTATUS = ViewModel.WEATHERSTATUS,
                Track = track,
            };

            _context.KarterTrackStats.Add(model);
        }
    }
}
