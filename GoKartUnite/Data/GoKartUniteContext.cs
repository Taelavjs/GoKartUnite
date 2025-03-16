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

            modelBuilder.Entity<Friendships>()
                .HasKey(f => new { f.KarterFirstId, f.KarterSecondId });

            modelBuilder.Entity<FollowTrack>()
                .HasKey(f => new { f.KarterId, f.TrackId });

            modelBuilder.Entity<BlogNotifications>()
                .HasOne(bn => bn.LinkedPost)
                .WithMany()
                .HasForeignKey(bn => bn.BlogID);

            modelBuilder.Entity<BlogNotifications>()
                .ToTable("BlogNotifications", "dbo");

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany()
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.BlogPost)
                .WithMany(bp => bp.Comments)
                .HasForeignKey(c => c.BlogPostId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BlogPost>()
                .HasOne(c => c.Author)
                .WithMany()
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BlogNotifications>()
                .HasOne(c => c.Author)
                .WithMany()
                .HasForeignKey(c => c.userId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Upvotes>()
                .HasOne(c => c.Karter)
                .WithMany()
                .HasForeignKey(c => c.VoterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Membership>()
                .HasKey(f => new { f.GroupId, f.KarterId });

            modelBuilder.Entity<Membership>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.KarterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Membership>()
                .HasOne(f => f.Group)
                .WithMany()
                .HasForeignKey(f => f.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Group>()
    .HasOne(g => g.HostKarter)  // Group has one HostKarter
    .WithMany()  // A Karter can be a host of multiple groups
    .HasForeignKey(g => g.HostId)  // HostId is the foreign key
    .OnDelete(DeleteBehavior.Restrict);  // Prevent cascade delete (optional)

            // Configuring the Membership-Karter-Group relationships
            modelBuilder.Entity<Membership>()
                .HasOne(m => m.Group)
                .WithMany(g => g.MemberKarters)
                .HasForeignKey(m => m.GroupId)
                .OnDelete(DeleteBehavior.Cascade);  // When Group is deleted, remove Memberships (optional)

            modelBuilder.Entity<Membership>()
                .HasOne(m => m.User)
                .WithMany()  // A Karter can have multiple Memberships
                .HasForeignKey(m => m.KarterId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<GoKartUnite.Models.Karter> Karter { get; set; } = default!;
        public DbSet<GoKartUnite.Models.Track> Track { get; set; } = default!;
        public DbSet<GoKartUnite.Models.Friendships> Friendships { get; set; } = default!;
        public DbSet<GoKartUnite.Models.BlogPost> BlogPosts { get; set; } = default!;
        public DbSet<GoKartUnite.Models.FollowTrack> FollowTracks { get; set; } = default!;
        public DbSet<GoKartUnite.Models.BlogNotifications> BlogNotifications { get; set; } = default!;
        public DbSet<GoKartUnite.Models.Comment> Comments { get; set; } = default!;
        public DbSet<GoKartUnite.Models.UserRoles> UserRoles { get; set; } = default!;
        public DbSet<GoKartUnite.Models.Role> Role { get; set; } = default!;
        public DbSet<GoKartUnite.Models.TrackAdmins> TrackAdmin { get; set; } = default!;
        public DbSet<GoKartUnite.Models.KarterTrackStats> KarterTrackStats { get; set; } = default!;
        public DbSet<GoKartUnite.Models.Groups.Membership> Memberships { get; set; } = default!;
        public DbSet<Group> Groups { get; set; } = default!;
    }
}