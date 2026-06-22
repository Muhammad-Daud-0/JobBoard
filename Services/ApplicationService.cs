using JobBoard.Models.DTOs.Applications;
using JobBoard.Models.Entities;
using JobBoard.Repositories;
using JobBoard.Services.Interfaces;

namespace JobBoard.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IUnitOfWork _uow;

        public ApplicationService(IUnitOfWork uow) => _uow = uow;

        public async Task ApplyAsync(ApplyDto dto, string candidateId, int jobId, string? cvPath = null, string? cvFileName = null)
        {
            var alreadyApplied = await _uow.Applications.HasAppliedAsync(candidateId, jobId);
            if (alreadyApplied)
                throw new InvalidOperationException("You have already applied to this job.");

            var application = new JobApplication
            {
                CoverLetter = dto.CoverLetter,
                CandidateId = candidateId,
                JobId = jobId,
                AppliedAt = DateTime.UtcNow,
                CvPath = cvPath,
                CvFileName = cvFileName
            };

            await _uow.Applications.AddAsync(application);
            await _uow.SaveChangesAsync();
        }

        public async Task<IEnumerable<ApplicationListDto>> GetCandidateAppsAsync(string candidateId)
        {
            var apps = await _uow.Applications.GetCandidateApplicationsAsync(candidateId);
            return apps.Select(MapToDto);
        }

        public async Task<IEnumerable<ApplicationListDto>> GetJobApplicantsAsync(int jobId)
        {
            var apps = await _uow.Applications.GetJobApplicantsAsync(jobId);
            return apps.Select(MapToDto);
        }

        public async Task UpdateStatusAsync(int id, ApplicationStatus status)
        {
            var app = await _uow.Applications.GetByIdAsync(id);
            if (app == null) return;

            app.Status = status;
            _uow.Applications.Update(app);
            await _uow.SaveChangesAsync();
        }

        private static ApplicationListDto MapToDto(JobApplication a) => new()
        {
            Id = a.Id,
            JobTitle = a.Job?.Title ?? "",
            CompanyName = a.Job?.Employer?.CompanyName ?? a.Job?.Employer?.FullName ?? "N/A",
            Location = a.Job?.Location ?? "",
            CoverLetter = a.CoverLetter,
            Status = a.Status,
            AppliedAt = a.AppliedAt,
            JobId = a.JobId,
            CandidateName = a.Candidate?.FullName ?? "",
            CandidateEmail = a.Candidate?.Email ?? "",
            CandidateId = a.CandidateId,
            EmployerId = a.Job?.EmployerId ?? "",
            CvPath = a.CvPath,
            CvFileName = a.CvFileName
        };
    }
}
