using JobBoard.Models.DTOs.Jobs;
using JobBoard.Models.Entities;
using JobBoard.Repositories;
using JobBoard.Services.Interfaces;

namespace JobBoard.Services
{
    public class JobService : IJobService
    {
        private readonly IUnitOfWork _uow;

        public JobService(IUnitOfWork uow) => _uow = uow;

        public async Task<PagedResult<JobListDto>> GetJobsAsync(JobFilterDto filter)
        {
            var jobs = await _uow.Jobs.GetJobsAsync(filter.Search, filter.CategoryId, filter.Type, filter.Page, filter.PageSize);
            var total = await _uow.Jobs.CountAsync(filter.Search, filter.CategoryId, filter.Type);

            return new PagedResult<JobListDto>
            {
                Items = jobs.Select(MapToListDto),
                TotalCount = total,
                Page = filter.Page,
                PageSize = filter.PageSize,
                Search = filter.Search,
                CategoryId = filter.CategoryId,
                Type = filter.Type
            };
        }

        public async Task<JobDetailDto?> GetByIdAsync(int id)
        {
            var job = await _uow.Jobs.GetWithDetailsAsync(id);
            if (job == null) return null;
            return MapToDetailDto(job);
        }

        public async Task<int> CreateJobAsync(CreateJobDto dto, string employerId)
        {
            var job = new Job
            {
                Title = dto.Title,
                Description = dto.Description,
                Location = dto.Location,
                SalaryRange = dto.SalaryRange,
                Type = dto.Type,
                CategoryId = dto.CategoryId,
                EmployerId = employerId,
                PostedAt = DateTime.UtcNow
            };
            await _uow.Jobs.AddAsync(job);
            await _uow.SaveChangesAsync();
            return job.Id;
        }

        public async Task UpdateJobAsync(int id, UpdateJobDto dto)
        {
            var job = await _uow.Jobs.GetByIdAsync(id);
            if (job == null) return;

            job.Title = dto.Title;
            job.Description = dto.Description;
            job.Location = dto.Location;
            job.SalaryRange = dto.SalaryRange;
            job.Type = dto.Type;
            job.Status = dto.Status;
            job.CategoryId = dto.CategoryId;

            if (dto.Status == JobStatus.Closed)
                job.ClosedAt = DateTime.UtcNow;

            _uow.Jobs.Update(job);
            await _uow.SaveChangesAsync();
        }

        public async Task DeleteJobAsync(int id)
        {
            var job = await _uow.Jobs.GetByIdAsync(id);
            if (job == null) return;
            _uow.Jobs.Delete(job);
            await _uow.SaveChangesAsync();
        }

        public async Task<IEnumerable<JobListDto>> GetEmployerJobsAsync(string employerId)
        {
            var jobs = await _uow.Jobs.GetByEmployerAsync(employerId);
            return jobs.Select(MapToListDto);
        }

        public async Task<IEnumerable<JobListDto>> GetAllJobsAsync()
        {
            var jobs = await _uow.Jobs.GetAllAsync();
            return jobs.Select(MapToListDto);
        }

        private static JobListDto MapToListDto(Job j) => new()
        {
            Id = j.Id,
            Title = j.Title,
            CompanyName = j.Employer?.CompanyName ?? j.Employer?.FullName ?? "N/A",
            Location = j.Location,
            SalaryRange = j.SalaryRange,
            Type = j.Type,
            Status = j.Status,
            Category = j.Category?.Name ?? "",
            PostedAt = j.PostedAt,
            ApplicationCount = j.Applications?.Count ?? 0,
            EmployerId = j.EmployerId
        };

        private static JobDetailDto MapToDetailDto(Job j) => new()
        {
            Id = j.Id,
            Title = j.Title,
            Description = j.Description,
            CompanyName = j.Employer?.CompanyName ?? j.Employer?.FullName ?? "N/A",
            EmployerEmail = j.Employer?.Email ?? "",
            Location = j.Location,
            SalaryRange = j.SalaryRange,
            Type = j.Type,
            Status = j.Status,
            Category = j.Category?.Name ?? "",
            CategoryId = j.CategoryId,
            PostedAt = j.PostedAt,
            ApplicationCount = j.Applications?.Count ?? 0,
            EmployerId = j.EmployerId
        };
    }
}
