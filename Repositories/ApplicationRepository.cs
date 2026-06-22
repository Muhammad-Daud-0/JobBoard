using JobBoard.Data;
using JobBoard.Models.Entities;
using JobBoard.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly AppDbContext _db;

        public ApplicationRepository(AppDbContext db) => _db = db;

        public async Task<JobApplication?> GetByIdAsync(int id) =>
            await _db.JobApplications.FindAsync(id);

        public async Task<IEnumerable<JobApplication>> GetAllAsync() =>
            await _db.JobApplications.ToListAsync();

        public async Task AddAsync(JobApplication entity) =>
            await _db.JobApplications.AddAsync(entity);

        public void Update(JobApplication entity) => _db.JobApplications.Update(entity);

        public void Delete(JobApplication entity) => _db.JobApplications.Remove(entity);

        public async Task<IEnumerable<JobApplication>> GetCandidateApplicationsAsync(string candidateId) =>
            await _db.JobApplications
                .Include(a => a.Job)
                    .ThenInclude(j => j.Employer)
                .Include(a => a.Job)
                    .ThenInclude(j => j.Category)
                .Where(a => a.CandidateId == candidateId)
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();

        public async Task<IEnumerable<JobApplication>> GetJobApplicantsAsync(int jobId) =>
            await _db.JobApplications
                .Include(a => a.Candidate)
                .Where(a => a.JobId == jobId)
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();

        public async Task<bool> HasAppliedAsync(string candidateId, int jobId) =>
            await _db.JobApplications.AnyAsync(a => a.CandidateId == candidateId && a.JobId == jobId);

        public async Task<JobApplication?> GetWithDetailsAsync(int id) =>
            await _db.JobApplications
                .Include(a => a.Job)
                .Include(a => a.Candidate)
                .FirstOrDefaultAsync(a => a.Id == id);
    }
}
