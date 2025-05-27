using GoKartUnite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting
{
    internal class Helpers
    {
        public static Karter GenerateValidKarters(string initialName, int iter = 0)
        {
            return new Karter
            {
                Name = initialName + iter,
                YearsExperience = 3,
                Email = "TaelaVhs@gmail.com",

            };
        }
        public static Track GenerateValidTrack(string initialName, int iter = 0, Locations location = Locations.NORTHEAST)
        {
            return new Track { Title = initialName + iter, Location = location, IsVerifiedByAdmin = true };
        }
    }

    internal class ConstValues
    {
        static public Karter SelfKarter = new Karter
        {
            Email = "dummy@example.com",
            NameIdentifier = "123",
            Name = "Dummy Karter",
            YearsExperience = 3,
            TrackId = null
        };
    }
}
