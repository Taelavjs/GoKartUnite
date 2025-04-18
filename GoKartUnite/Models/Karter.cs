using GoKartUnite.Data;
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

        public virtual List<UserRoles> UserRoles { get; set; } = new List<UserRoles>();
        public virtual List<Friendships> Friendships { get; set; } = new List<Friendships>();
        public virtual List<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
        public virtual List<BlogNotifications> Notification { get; set; } = new List<BlogNotifications>();
        public virtual List<KarterTrackStats> Stats { get; set; } = new List<KarterTrackStats>();
        public virtual List<Comment> Comments { get; set; } = new List<Comment>();
    }

    public enum FriendshipStatus
    {
        User,
        Requested,
        Received,
        Friend,
        UserSelf
    }

    public enum SortKartersBy
    {
        Alphabetically,
        ReverseAlphabetically,
        YearsExperience,
        ReverseYearsExperience
    }
}
