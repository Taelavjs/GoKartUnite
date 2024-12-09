using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GoKartUnite.Models;

namespace GoKartUnite.Data
{
    public class GoKartUniteContext : DbContext
    {
        public GoKartUniteContext (DbContextOptions<GoKartUniteContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Friendships>()
                .HasOne(f => f.KarterFirst)
                .WithMany()
                .HasForeignKey(f => f.KarterFirstId)
                .OnDelete(DeleteBehavior.Restrict); // Allow cascading delete

            modelBuilder.Entity<Friendships>()
                .HasOne(f => f.KarterSecond)
                .WithMany()
                .HasForeignKey(f => f.KarterSecondId)
                .OnDelete(DeleteBehavior.Restrict); // Disable cascading delete

            modelBuilder.Entity<Friendships>()
                .HasKey(f => new { f.KarterFirstId, f.KarterSecondId });
        }
        public DbSet<GoKartUnite.Models.Karter> Karter { get; set; } = default!;
        public DbSet<GoKartUnite.Models.Track> Track { get; set; } = default!;
        public DbSet<GoKartUnite.Models.Friendships> Friendships { get; set; } = default!;
    }
}
