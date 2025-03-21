using GoKartUnite.Data;
using GoKartUnite.Interfaces;
using GoKartUnite.Models;
using GoKartUnite.Models.Groups;
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

            await _context.KarterTrackStats.AddAsync(model);
            await _context.SaveChangesAsync();

            await CreateGroupNotification(model);
            await CreateMessageInAllUserGroups(model.KarterId, track.Title, model.BestLapTime);
        }

        public async Task<List<KarterTrackStats>> GetStatsForKarter(int karterId)
        {
            List<KarterTrackStats> stats = await _context.KarterTrackStats.Include(x => x.ForKarter).Include(x => x.RecordedTrack)
                .Where(x => x.ForKarter.Id == karterId)
                .ToListAsync();

            return stats;
        }

        public async Task CreateGroupNotification(KarterTrackStats stat)
        {
            GroupNotification notif = new GroupNotification
            {
                Stat = stat,
                StatId = stat.Id,
            };

            await _context.GroupNotifications.AddAsync(notif);
            await _context.SaveChangesAsync();
        }

        public async Task CreateMessageInAllUserGroups(int userId, string TrackTitle, TimeSpan StatTime)
        {
            List<int> groupIds = await _context.Groups
                .Where(x => x.HostId == userId || x.MemberKarters.Any(x => x.KarterId == userId))
                .SelectMany(x => x.MemberKarters.Select(x => x.GroupId))
                .ToListAsync();

            var messages = groupIds.Select(groupId => new GroupMessage
            {
                AuthorId = userId,
                GroupCommentOnId = groupId,
                MessageContent = $"Karter has achieved a time of {StatTime} at {TrackTitle}! Congrats!"
            }).ToList();


            await _context.GroupMessages.AddRangeAsync(messages);
            await _context.SaveChangesAsync();
        }
    }
}
