using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GoKartUnite.Models
{
    public class Friendships
    {
        public int KarterFirstId { get; set; }

        [ForeignKey(nameof(KarterFirstId))]
        public virtual Karter KarterFirst { get; set; }

        public int KarterSecondId { get; set; }

        [ForeignKey(nameof(KarterSecondId))]
        public virtual Karter KarterSecond { get; set; }

        public DateOnly? DateCreated { get; set; } = DateOnly.FromDateTime(DateTime.Now); // Made public for EF mapping

        public int requestedByInt { get; set; }

        public bool accepted { get; set; } = false;

        // Parameterless constructor for EF Core to use
        public Friendships() { }

        // Existing parameterized constructor for custom initialization
        public Friendships(int SentByKarterId, int ToKarterId)
        {
            requestedByInt = SentByKarterId;
            if (SentByKarterId > ToKarterId)
            {
                KarterFirstId = ToKarterId;
                KarterSecondId = SentByKarterId;
            }
            else
            {
                KarterFirstId = SentByKarterId;
                KarterSecondId = ToKarterId;
            }
            DateCreated = DateOnly.FromDateTime(DateTime.Now);
        }
    }

}
