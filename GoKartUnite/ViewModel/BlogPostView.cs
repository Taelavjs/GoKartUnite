﻿using GoKartUnite.Models;
using System.ComponentModel.DataAnnotations;

namespace GoKartUnite.ViewModel
{
    public class BlogPostView
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; } = string.Empty;
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Descripttion { get; set; } = string.Empty;
        public string? Author { get; set; }
        public int? authorId { get; set; }
        public int Upvotes { get; set; } = 0;
        public string TaggedTrack { get; set; } = string.Empty;
        public BlogType blogType { get; set; } = BlogType.Post;
    }
}
