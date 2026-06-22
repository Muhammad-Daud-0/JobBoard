using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace JobBoard.Models.DTOs.Account
{
    public class UpdateProfileDto
    {
        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
        public string? FullName { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_.-]+$", ErrorMessage = "Username can only contain letters, numbers, underscores, dots, and hyphens.")]
        public string? UserName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? ProfileImageUrl { get; set; }
        public IFormFile? ProfileImageFile { get; set; }

        // Candidate profile fields
        public string? Bio { get; set; }
        public string? Skills { get; set; } // Comma-separated list of skills
        public string? CvPath { get; set; }
        public string? CvFileName { get; set; }
        public IFormFile? CvFile { get; set; }

        // Employer profile fields
        public string? CompanyName { get; set; } // Employer only
        public string? CompanyDescription { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? Location { get; set; }
    }

    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Current password is required.")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "New password must be at least 8 characters long.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm new password is required.")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
