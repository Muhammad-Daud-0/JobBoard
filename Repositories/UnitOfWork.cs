using JobBoard.Data;
using JobBoard.Repositories.Interfaces;

namespace JobBoard.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IJobRepository Jobs { get; }
        IApplicationRepository Applications { get; }
        IChatRepository ChatMessages { get; }
        Task<int> SaveChangesAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;

        public IJobRepository Jobs { get; }
        public IApplicationRepository Applications { get; }
        public IChatRepository ChatMessages { get; }

        public UnitOfWork(AppDbContext db)
        {
            _db = db;
            Jobs = new JobRepository(db);
            Applications = new ApplicationRepository(db);
            ChatMessages = new ChatRepository(db);
        }

        public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();

        public void Dispose() => _db.Dispose();
    }
}
