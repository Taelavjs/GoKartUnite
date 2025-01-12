namespace GoKartUnite.Models
{
    public class TrackAdmins
    {
        public int Id { get; set; }
        public int KarterId { get; set; }
        public virtual Track? ManagedTrack { get; set; }
        public int TrackId { get; set; }
    }
}
