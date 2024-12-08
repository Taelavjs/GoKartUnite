using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GoKartUnite.Models
{
    public class Friendships
    {
        public required int KarterFirstId { get; set; }

        [ForeignKey(nameof(KarterFirstId))]
        public required Karter KarterFirst { get; set; }

        public required int KarterSecondId { get; set; }

        [ForeignKey(nameof(KarterSecondId))]
        public required Karter KarterSecond { get; set; }

        public DateOnly? DateCreated { get; set; } // Made public for EF mapping
    }
}
