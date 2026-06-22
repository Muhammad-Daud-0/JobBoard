using JobBoard.Data;
using JobBoard.Models.Entities;
using JobBoard.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly AppDbContext _db;

        public JobRepository(AppDbContext db) => _db = db;

        public async Task<Job?> GetByIdAsync(int id) =>
            await _db.Jobs.FindAsync(id);

        public async Task<IEnumerable<Job>> GetAllAsync() =>
            await _db.Jobs.Include(j => j.Employer).Include(j => j.Category).ToListAsync();

        public async Task AddAsync(Job entity) =>
            await _db.Jobs.AddAsync(entity);

        public void Update(Job entity) => _db.Jobs.Update(entity);

        public void Delete(Job entity) => _db.Jobs.Remove(entity);

        public async Task<IEnumerable<Job>> GetJobsAsync(
            string? search, int? categoryId, JobType? type, int page, int pageSize)
        {
            var query = _db.Jobs
                .Include(j => j.Employer)
                .Include(j => j.Category)
                .Include(j => j.Applications)
                .Where(j => j.Status == JobStatus.Open)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(j =>
                    j.Title.Contains(search) ||
                    j.Description.Contains(search) ||
                    j.Location.Contains(search));

            if (categoryId.HasValue)
                query = query.Where(j => j.CategoryId == categoryId.Value);

            if (type.HasValue)
                query = query.Where(j => j.Type == type.Value);

            return await query
                .OrderByDescending(j => j.PostedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync(string? search, int? categoryId, JobType? type)
        {
            var query = _db.Jobs.Where(j => j.Status == JobStatus.Open).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(j =>
                    j.Title.Contains(search) ||
                    j.Description.Contains(search) ||
                    j.Location.Contains(search));

            if (categoryId.HasValue)
                query = query.Where(j => j.CategoryId == categoryId.Value);

            if (type.HasValue)
                query = query.Where(j => j.Type == type.Value);

            return await query.CountAsync();
        }

        public async Task<IEnumerable<Job>> GetByEmployerAsync(string employerId) =>
            await _db.Jobs
                .Include(j => j.Category)
                .Include(j => j.Applications)
                .Where(j => j.EmployerId == employerId)
                .OrderByDescending(j => j.PostedAt)
                .ToListAsync();

        public async Task<Job?> GetWithDetailsAsync(int id) =>
            await _db.Jobs
                .Include(j => j.Employer)
                .Include(j => j.Category)
                .Include(j => j.Applications)
                    .ThenInclude(a => a.Candidate)
                .FirstOrDefaultAsync(j => j.Id == id);
    }
}
