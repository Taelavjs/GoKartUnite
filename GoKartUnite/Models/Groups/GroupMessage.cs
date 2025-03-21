﻿using System.ComponentModel.DataAnnotations.Schema;

namespace GoKartUnite.Models.Groups
{
    public class GroupMessage
    {
        public int Id { get; set; }

        public int AuthorId { get; set; }
        public virtual Karter Author { get; set; }
        public string MessageContent { get; set; } = string.Empty;
        public DateTime DateTimePosted { get; set; } = DateTime.UtcNow;

        public virtual Group GroupCommentedOn { get; set; }
        [ForeignKey(nameof(GroupCommentedOn))]
        public int GroupCommentOnId { get; set; }
    }
}
