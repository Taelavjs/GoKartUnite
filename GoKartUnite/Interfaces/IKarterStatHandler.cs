﻿using GoKartUnite.Handlers;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;

namespace GoKartUnite.Interfaces
{
    public interface IKarterStatHandler
    {
        Task<bool> CreateStatRecord(KarterStatViewModel ViewModel, Track track, Karter karter, TimeSpan BestLapFormatted);
        Task<List<KarterTrackStats>> GetStatsForKarter(int karterId, string trackFilter);
    }
}
