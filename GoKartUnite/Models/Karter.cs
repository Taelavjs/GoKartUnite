using GoKartUnite.Data;
using GoKartUnite.Migrations;
using Humanizer.Localisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SendGrid.Helpers.Mail;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Policy;
using System.Threading;

namespace GoKartUnite.Models
{
    public class Karter
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? NameIdentifier { get; set; }



        public string Name { get; set; }
        public int YearsExperience { get; set; }

        // Relationships
        public int? TrackId { get; set; }
        public Track? Track { get; set; }

        public virtual ICollection<UserRoles>? UserRoles { get; set; }

        public virtual ICollection<Friendships>? Friendships { get; set; }
        public virtual ICollection<BlogPost>? BlogPosts { get; set; }
        public ICollection<BlogNotifications>? Notification { get; set; }


    }

    public enum FriendshipStatus
    {
        User,
        Requested,
        Received,
        Friend
    }

    public enum SortKartersBy
    {
        Alphabetically,
        ReverseAlphabetically,
        YearsExperience,
        ReverseYearsExperience
    }
}
