using JobBoard.Models.Entities;

namespace JobBoard.Repositories.Interfaces
{
    public interface IApplicationRepository : IRepository<JobApplication>
    {
        Task<IEnumerable<JobApplication>> GetCandidateApplicationsAsync(string candidateId);
        Task<IEnumerable<JobApplication>> GetJobApplicantsAsync(int jobId);
        Task<bool> HasAppliedAsync(string candidateId, int jobId);
        Task<JobApplication?> GetWithDetailsAsync(int id);
    }
}
