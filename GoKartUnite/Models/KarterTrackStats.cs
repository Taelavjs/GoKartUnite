namespace GoKartUnite.Models
{
    public class KarterTrackStats
    {
        public int Id { get; set; }
        public string RaceName { get; set; }
        public int RaceLength { get; set; }
        public bool isChampionshipRace { get; set; } = false;
        public Track? RecordedTrack { get; set; }
        public Karter? ForKarter { get; set; }
        public DateOnly DateOnlyRecorded { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public WEATHERSTATUS WEATHERSTATUS { get; set; } = WEATHERSTATUS.None;
        public TEMPERATURE TEMPERATURE { get; set; } = TEMPERATURE.None;
        public TimeSpan BestLapTime { get; set; }
    }

    public enum WEATHERSTATUS
    {
        None,
        Dry,
        Damp,
        Raining,
        Waterlogged
    }

    public enum TEMPERATURE
    {
        None,
        Hot,
        Warm,
        Cold
    }
}
