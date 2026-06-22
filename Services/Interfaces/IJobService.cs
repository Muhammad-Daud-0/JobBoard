using JobBoard.Models.DTOs.Jobs;

namespace JobBoard.Services.Interfaces
{
    public interface IJobService
    {
        Task<PagedResult<JobListDto>> GetJobsAsync(JobFilterDto filter);
        Task<JobDetailDto?> GetByIdAsync(int id);
        Task<int> CreateJobAsync(CreateJobDto dto, string employerId);
        Task UpdateJobAsync(int id, UpdateJobDto dto);
        Task DeleteJobAsync(int id);
        Task<IEnumerable<JobListDto>> GetEmployerJobsAsync(string employerId);
        Task<IEnumerable<JobListDto>> GetAllJobsAsync();
    }
}
