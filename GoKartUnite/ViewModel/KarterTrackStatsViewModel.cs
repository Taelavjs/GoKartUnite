using GoKartUnite.Models;
using System.ComponentModel.DataAnnotations;

namespace GoKartUnite.ViewModel
{
    public class KarterTrackStatsViewModel
    {
        [MinLength(5, ErrorMessage = "Please Minimuim 2 Characters")]
        [MaxLength(10, ErrorMessage = "Please Max 10 Characters")]
        public string RaceName { get; set; } = string.Empty;

        [TimeSpanRangeAttribute("00:01:00", "24:00:00")]
        public TimeSpan RaceLength { get; set; }
        public bool isChampionshipRace { get; set; } = false;
        public Track? Track { get; set; }
        public DateTime DateOnlyRecorded { get; set; } = DateTime.Today;
        public WEATHERSTATUS WEATHERSTATUS { get; set; } = WEATHERSTATUS.None;
        public TEMPERATURE TEMPERATURE { get; set; } = TEMPERATURE.None;
        [TimeSpanRangeAttribute("00:00:20", "00:10:00")]
        public TimeSpan BestLapTime { get; set; }
    }

    public class TimeSpanRangeAttribute : ValidationAttribute
    {
        private readonly TimeSpan _min;
        private readonly TimeSpan _max;

        public TimeSpanRangeAttribute(string min, string max)
        {
            _min = TimeSpan.Parse(min);
            _max = TimeSpan.Parse(max);
            ErrorMessage = $"Time must be between {_min} and {_max}.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is TimeSpan timeSpan)
            {
                if (timeSpan < _min || timeSpan > _max)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }
}
