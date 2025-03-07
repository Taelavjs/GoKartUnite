using GoKartUnite.Models;
using System.ComponentModel.DataAnnotations;

namespace GoKartUnite.ViewModel
{
    public class BlogPostView
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; } = string.Empty;
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;
        public Karter? Author { get; set; }
        public int? authorId { get; set; }
        public int Upvotes { get; set; } = 0;
        public string TaggedTrackTitle { get; set; } = string.Empty;
        public Track? TaggedTrack { get; set; }
        public BlogType blogType { get; set; } = BlogType.Post;

        [FutureDateAttribute]
        public DateTime? ReleaseDateTime { get; set; }
    }


    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateTime)
            {
                if (dateTime > DateTime.Now)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("The date must be in the future.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
