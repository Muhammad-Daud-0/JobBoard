namespace JobBoard.Models.Entities
{
    public enum ApplicationStatus { Pending, Reviewed, Shortlisted, Rejected }

    public class JobApplication
    {
        public int Id { get; set; }
        public string CoverLetter { get; set; } = string.Empty;
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        public string CandidateId { get; set; } = string.Empty;
        public ApplicationUser Candidate { get; set; } = null!;

        public int JobId { get; set; }
        public Job Job { get; set; } = null!;

        public string? CvPath { get; set; }
        public string? CvFileName { get; set; }
    }
}
