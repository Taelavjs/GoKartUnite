using FluentValidation;
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
        [MaxLength(255)]
        [EmailAddress]
        public string? Email { get; set; }

        [Display(Name = "Karter")]
        [MinLength(3)]
        [StringLength(15)]

        public required string Name { get; set; }
        [Range(0, 20)]
        public required int YearsExperience { get; set; }

        // Relationships
        [Display(Name = "Local Track Id")]
        public int? TrackId { get; set; }
        public Track? Track { get; set; }
        
        public virtual ICollection<Friendships>? Friendships { get; set; }
        public virtual ICollection<BlogPost>? BlogPosts { get; set; }

    }
}
