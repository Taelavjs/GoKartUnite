using GoKartUnite.Models;
using GoKartUnite.Models.Groups;
using Microsoft.EntityFrameworkCore;

namespace GoKartUnite.Data
{
    public class GoKartUniteContext : DbContext
    {
        public GoKartUniteContext(DbContextOptions<GoKartUniteContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ---------- BlogNotifications ----------
            modelBuilder.Entity<BlogNotifications>()
                .ToTable("BlogNotifications", "dbo");

            modelBuilder.Entity<BlogNotifications>()
                .HasOne(bn => bn.LinkedPost)
                .WithMany()
                .HasForeignKey(bn => bn.BlogID);

            modelBuilder.Entity<BlogNotifications>()
                .HasOne(bn => bn.Author)
                .WithMany()
                .HasForeignKey(bn => bn.userId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- BlogPost ----------


            // ---------- Comment ----------


            // ---------- FollowTrack ----------
            modelBuilder.Entity<FollowTrack>()
                .HasKey(ft => new { ft.KarterId, ft.TrackId });

            // ---------- Friendships ----------
            modelBuilder.Entity<Friendships>()
                .HasKey(f => new { f.KarterFirstId, f.KarterSecondId });

            modelBuilder.Entity<Friendships>()
                .HasOne(f => f.KarterFirst)
                .WithMany()
                .HasForeignKey(f => f.KarterFirstId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friendships>()
                .HasOne(f => f.KarterSecond)
                .WithMany()
                .HasForeignKey(f => f.KarterSecondId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- Group ----------
            modelBuilder.Entity<Group>()
                .HasOne(g => g.HostKarter)
                .WithMany()
                .HasForeignKey(g => g.HostId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // ---------- Membership ----------
            modelBuilder.Entity<Membership>()
                .HasKey(m => new { m.GroupId, m.KarterId });

            modelBuilder.Entity<Membership>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.KarterId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Membership>()
                .HasOne(m => m.Group)
                .WithMany(g => g.MemberKarters)
                .HasForeignKey(m => m.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------- Upvotes ----------
            modelBuilder.Entity<Upvotes>()
                .HasOne(u => u.Karter)
                .WithMany()
                .HasForeignKey(u => u.VoterId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- Karter ----------
            modelBuilder.Entity<Karter>()
                .HasOne(k => k.Track)
                .WithMany(t => t.Karters)
                .HasForeignKey(k => k.TrackId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- FollowTrack ----------
            modelBuilder.Entity<FollowTrack>()
                .HasOne(ft => ft.track)
                .WithMany(t => t.Followers) // matches the property added in Track
                .HasForeignKey(ft => ft.TrackId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FollowTrack>()
                .HasOne(ft => ft.karter)
                .WithMany()
                .HasForeignKey(ft => ft.KarterId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        // ---------- DbSet Properties ----------
        public DbSet<Karter> Karter { get; set; } = default!;
        public DbSet<Track> Track { get; set; } = default!;
        public DbSet<Friendships> Friendships { get; set; } = default!;
        public DbSet<FollowTrack> FollowTracks { get; set; } = default!;
        public DbSet<BlogPost> BlogPosts { get; set; } = default!;
        public DbSet<BlogNotifications> BlogNotifications { get; set; } = default!;
        public DbSet<Comment> Comments { get; set; } = default!;
        public DbSet<UserRoles> UserRoles { get; set; } = default!;
        public DbSet<Role> Role { get; set; } = default!;
        public DbSet<TrackAdmins> TrackAdmin { get; set; } = default!;
        public DbSet<KarterTrackStats> KarterTrackStats { get; set; } = default!;

        // Group-related
        public DbSet<Membership> Memberships { get; set; } = default!;
        public DbSet<Group> Groups { get; set; } = default!;
        public DbSet<GroupMessage> GroupMessages { get; set; } = default!;
        public DbSet<GroupNotification> GroupNotifications { get; set; } = default!;
    }
}
