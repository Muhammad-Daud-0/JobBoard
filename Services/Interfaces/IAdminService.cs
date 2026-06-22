using JobBoard.Models.DTOs.Jobs;
using JobBoard.Models.Entities;

namespace JobBoard.Services.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<object>> GetAllUsersAsync();
        Task<IEnumerable<JobListDto>> GetAllJobsAsync();
        Task DeleteUserAsync(string userId);
    }
}
