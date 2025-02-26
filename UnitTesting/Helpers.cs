using GoKartUnite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
