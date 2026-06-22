namespace JobBoard.Models.Entities
{
    public enum JobType { FullTime, PartTime, Remote, Contract }
    public enum JobStatus { Open, Closed, Paused }

    public class Job
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string SalaryRange { get; set; } = string.Empty;
        public JobType Type { get; set; }
        public JobStatus Status { get; set; } = JobStatus.Open;
        public DateTime PostedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ClosedAt { get; set; }

        public string EmployerId { get; set; } = string.Empty;
        public ApplicationUser Employer { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    }
}
