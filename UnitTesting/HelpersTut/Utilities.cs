using GoKartUnite.Data;
using GoKartUnite.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static System.Formats.Asn1.AsnWriter;

namespace UnitTesting.HelpersTut
{
    public static class Utilities
    {


        public static List<int> InitializeKarterDbForTests(GoKartUniteContext db)
        {
            var karters = GetSeedingMessages();
            db.Karter.AddRange(karters);
            db.SaveChanges();
            return karters.Select(x => x.Id).ToList();
        }

        public static async Task ReinitializeKarterDbForTests(GoKartUniteContext db)
        {
            db.Karter.RemoveRange(db.Karter);
            db.SaveChanges();
            InitializeKarterDbForTests(db);
        }

        public static List<Karter> GetSeedingMessages()
        {
            return new List<Karter>
        {
            new Karter
            {
                Email = "dummy@example.com",
                NameIdentifier = "123",
                Name = "Dummy Karter",
                YearsExperience = 3,
                TrackId = null
            },
            new Karter
            {
                Email = "second@example.com",
                NameIdentifier = "456",
                Name = "Second Karter",
                YearsExperience = 1,
                TrackId = null
            }
        };
        }
    }
}
