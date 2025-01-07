﻿using System.Reflection;

namespace GoKartUnite.Models
{
    public class BlogPost
    {

        public int Id { get; set; }

        public int AuthorId { get; set; }
        public virtual Karter? Author { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Descripttion { get; set; } = string.Empty;
        public DateTime DateTimePosted { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Upvotes> Upvotes { get; set; } = new List<Upvotes>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public virtual Track? TaggedTrack { get; set; }
        public int? TaggedTrackId { get; set; }
    }
}
