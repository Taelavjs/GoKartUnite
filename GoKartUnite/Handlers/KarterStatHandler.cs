using GoKartUnite.Data;
using GoKartUnite.Interfaces;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace GoKartUnite.Handlers
{
    public class KarterStatHandler : IKarterStatHandler
    {
        private readonly GoKartUniteContext _context;
        public KarterStatHandler(GoKartUniteContext context)
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
                TrackId = track.Id,
                RecordedTrack = track,
                KarterId = karter.Id,
                ForKarter = karter
            };

            _context.KarterTrackStats.Add(model);
            _context.SaveChanges();
        }

        public async Task<List<KarterTrackStats>> GetStatsForKarter(int karterId)
        {
            List<KarterTrackStats> stats = await _context.KarterTrackStats.Include(x => x.ForKarter).Include(x => x.RecordedTrack)
                .Where(x => x.ForKarter.Id == karterId)
                .ToListAsync();

            return stats;
        }
    }
}
