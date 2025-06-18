using GoKartUnite.Data;
using GoKartUnite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting.HelpersTut
{
    public static class Utilities
    {
        public static void InitializeKarterDbForTests(GoKartUniteContext db)
        {
            db.Karter.AddRange(GetSeedingMessages());
            db.SaveChanges();
        }

        public static void ReinitializeKarterDbForTests(GoKartUniteContext db)
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
