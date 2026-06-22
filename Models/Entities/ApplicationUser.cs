using Microsoft.AspNetCore.Identity;

namespace JobBoard.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? CompanyName { get; set; } // Employer only
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Candidate profile fields
        public string? Bio { get; set; }
        public string? Skills { get; set; } // Comma-separated list of skills
        public string? CvPath { get; set; } // Default CV file path
        public string? CvFileName { get; set; } // Default CV original filename

        // Employer profile fields
        public string? CompanyDescription { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? Location { get; set; }

        // General profile fields
        public string Address { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }

        public ICollection<Job> Jobs { get; set; } = new List<Job>();
        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    }
}
;