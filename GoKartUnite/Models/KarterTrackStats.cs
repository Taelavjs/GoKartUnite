namespace GoKartUnite.Models
{
    public class KarterTrackStats
    {
        public int Id { get; set; }
        public string RaceName { get; set; }
        public int RaceLength { get; set; }
        public bool isChampionshipRace { get; set; } = false;
        public Track? Track { get; set; }
        public Karter? Karter { get; set; }
        public DateTime DateOnlyRecorded { get; set; } = DateTime.Today;
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
