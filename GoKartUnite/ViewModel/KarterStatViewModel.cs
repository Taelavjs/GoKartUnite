using GoKartUnite.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoKartUnite.ViewModel
{
    public class KarterStatViewModel
    {
        [MinLength(5, ErrorMessage = "Please Minimuim 2 Characters")]
        [MaxLength(10, ErrorMessage = "Please Max 10 Characters")]
        public string RaceName { get; set; } = string.Empty;
        [MinLength(5, ErrorMessage = "Please Minimuim 2 Characters")]
        [MaxLength(10, ErrorMessage = "Please Max 10 Characters")]
        public string TrackTitle { get; set; } = string.Empty;


        [TimeSpanRangeAttribute("00:01:00", "24:00:00")]
        public int RaceLength { get; set; }
        public bool isChampionshipRace { get; set; } = false;

        public DateOnly DateOnlyRecorded { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public WEATHERSTATUS WEATHERSTATUS { get; set; } = WEATHERSTATUS.None;
        public TEMPERATURE TEMPERATURE { get; set; } = TEMPERATURE.None;
        [NotMapped] // Prevents EF from trying to map this
        public string BestLapTime { get; set; }


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

                if (timeSpan.Hours > 0 || timeSpan.Days > 0)
                {
                    return new ValidationResult("Only minutes, seconds, and milliseconds are allowed (no hours).");
                }
            }
            return ValidationResult.Success;
        }
    }
}
