using JobBoard.Models.Entities;

namespace JobBoard.Models.DTOs.Jobs
{
    public class CreateJobDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string SalaryRange { get; set; } = string.Empty;
        public JobType Type { get; set; }
        public int CategoryId { get; set; }
    }

    public class UpdateJobDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string SalaryRange { get; set; } = string.Empty;
        public JobType Type { get; set; }
        public JobStatus Status { get; set; }
        public int CategoryId { get; set; }
    }

    public class JobListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string SalaryRange { get; set; } = string.Empty;
        public JobType Type { get; set; }
        public JobStatus Status { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime PostedAt { get; set; }
        public int ApplicationCount { get; set; }
        public string EmployerId { get; set; } = string.Empty;
    }

    public class JobDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string EmployerEmail { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string SalaryRange { get; set; } = string.Empty;
        public JobType Type { get; set; }
        public JobStatus Status { get; set; }
        public string Category { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public DateTime PostedAt { get; set; }
        public int ApplicationCount { get; set; }
        public string EmployerId { get; set; } = string.Empty;
    }

    public class JobFilterDto
    {
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public JobType? Type { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public JobType? Type { get; set; }
    }
}
