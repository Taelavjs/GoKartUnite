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

        public DbSet<GoKartUnite.Models.Karter> Karter { get; set; } = default!;
        public DbSet<GoKartUnite.Models.Track> Track { get; set; } = default!;
    }
}
