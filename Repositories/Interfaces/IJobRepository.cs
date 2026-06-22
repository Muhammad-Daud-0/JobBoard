using JobBoard.Models.Entities;

namespace JobBoard.Repositories.Interfaces
{
    public interface IJobRepository : IRepository<Job>
    {
        Task<IEnumerable<Job>> GetJobsAsync(string? search, int? categoryId, JobType? type, int page, int pageSize);
        Task<int> CountAsync(string? search, int? categoryId, JobType? type);
        Task<IEnumerable<Job>> GetByEmployerAsync(string employerId);
        Task<Job?> GetWithDetailsAsync(int id);
    }
}
