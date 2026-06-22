using JobBoard.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace JobBoard.Models.DTOs.Applications
{
    public class ApplyDto
    {
        public string CoverLetter { get; set; } = string.Empty;
        public IFormFile? CvFile { get; set; }
        public bool UseProfileCv { get; set; }
    }

    public class ApplicationListDto
    {
        public int Id { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string CoverLetter { get; set; } = string.Empty;
        public ApplicationStatus Status { get; set; }
        public DateTime AppliedAt { get; set; }
        public int JobId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string CandidateEmail { get; set; } = string.Empty;
        public string CandidateId { get; set; } = string.Empty;
        public string EmployerId { get; set; } = string.Empty;
        public string? CvPath { get; set; }
        public string? CvFileName { get; set; }
    }
}
