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

        public static HttpClient ChangeUserAuthRole(HttpClient client, string RoleName)
        {
            client.DefaultRequestHeaders.Remove("Test-Roles");
            client.DefaultRequestHeaders.Add("Test-Roles", RoleName);
            return client;
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

        static public Track VerifiedTrack = new Track
        {
            Title = "Thunder Raceway",
            Description = "A fast and twisty track perfect for competitive karting.",
            Location = Locations.SOUTHWEST,
            IsVerifiedByAdmin = true,
            FormattedGoogleLocation = "123 Thunder Rd, Kart City, KC",
            GooglePlacesId = "ChIJd_dummy_track_id",
            Latitude = 37.7749,
        };

        static public Track NonVerifiedTrack = new Track
        {
            Title = "Shadow Ridge",
            Description = "A newly opened track under review for official races.",
            Location = Locations.EAST,
            IsVerifiedByAdmin = false,
            FormattedGoogleLocation = "456 Shadow Ln, Ghostville, GV",
            GooglePlacesId = "ChIJd_unverified_track_id",
            Latitude = 34.0522,
            Longitude = -118.2437
        };
    }
}
