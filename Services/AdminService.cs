using JobBoard.Models.DTOs.Jobs;
using JobBoard.Models.Entities;
using JobBoard.Repositories;
using JobBoard.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace JobBoard.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminService(IUnitOfWork uow, UserManager<ApplicationUser> userManager)
        {
            _uow = uow;
            _userManager = userManager;
        }

        public async Task<IEnumerable<object>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            var result = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.CompanyName,
                    user.CreatedAt,
                    Roles = roles
                });
            }

            return result;
        }

        public async Task<IEnumerable<JobListDto>> GetAllJobsAsync()
        {
            var jobs = await _uow.Jobs.GetAllAsync();
            return jobs.Select(j => new JobListDto
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
            });
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
                await _userManager.DeleteAsync(user);
        }
    }
}
