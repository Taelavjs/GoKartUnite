using GoKartUnite.Data;
using GoKartUnite.Interfaces;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;

namespace GoKartUnite.Handlers
{
    public class KarterStat : IKarterStat
    {
        private readonly GoKartUniteContext _context;
        public KarterStat(GoKartUniteContext context)
        {
            _context = context;
        }

        public async Task CreateStatRecord(KarterStatViewModel ViewModel, Track track, Karter karter, TimeSpan BestLapFormatted)
        {
            if (track == null)
            {
                return;
            }

            KarterTrackStats model = new KarterTrackStats
            {
                RaceName = ViewModel.RaceName,
                BestLapTime = BestLapFormatted,
                DateOnlyRecorded = ViewModel.DateOnlyRecorded,
                isChampionshipRace = ViewModel.isChampionshipRace,
                RaceLength = ViewModel.RaceLength,
                TEMPERATURE = ViewModel.TEMPERATURE,
                WEATHERSTATUS = ViewModel.WEATHERSTATUS,
                RecordedTrack = track,
                ForKarter = karter
            };

            _context.KarterTrackStats.Add(model);
            _context.SaveChanges();
        }
    }
}
