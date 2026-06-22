using JobBoard.Models.DTOs.Applications;
using JobBoard.Models.Entities;

namespace JobBoard.Services.Interfaces
{
    public interface IApplicationService
    {
        Task ApplyAsync(ApplyDto dto, string candidateId, int jobId, string? cvPath = null, string? cvFileName = null);
        Task<IEnumerable<ApplicationListDto>> GetCandidateAppsAsync(string candidateId);
        Task<IEnumerable<ApplicationListDto>> GetJobApplicantsAsync(int jobId);
        Task UpdateStatusAsync(int id, ApplicationStatus status);
    }
}
