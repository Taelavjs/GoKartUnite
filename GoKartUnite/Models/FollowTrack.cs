using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoKartUnite.Models
{
    [PrimaryKey(nameof(KarterId), nameof(TrackId))]
    public class FollowTrack
    {
        public int KarterId { get; set; }

        [ForeignKey(nameof(KarterId))]
        public Karter karter { get; set; }

        public int TrackId { get; set; }
        [ForeignKey(nameof(TrackId))]
        public Track track { get; set; }

        public DateTime FollowedAt { get; set; } = DateTime.Now;

        public FollowTrack(int karterId, int trackId)
        {
            KarterId = karterId;
            TrackId = trackId;
        }
    }
}
