﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GoKartUnite.Models
{
    public class Friendships
    {
        public int KarterFirstId { get; set; }

        [ForeignKey(nameof(KarterFirstId))]
        public virtual Karter KarterFirst { get; set; }

        public int KarterSecondId { get; set; }

        [ForeignKey(nameof(KarterSecondId))]
        public virtual Karter KarterSecond { get; set; }

        public DateOnly? DateCreated { get; set; } = DateOnly.FromDateTime(DateTime.Now); // Made public for EF mapping

        public int requestedByInt { get; set; }

        public bool accepted { get; set; } = false;
        public Friendships(int karterFirstId, int karterSecondId)
        {
            if (karterFirstId > karterSecondId)
            {
                KarterFirstId = karterSecondId;
                KarterSecondId = karterFirstId;
            }
            else
            {
                KarterFirstId = karterFirstId;
                KarterSecondId = karterSecondId;
            }
            DateCreated = DateOnly.FromDateTime(DateTime.Now);
        }


    }
}
