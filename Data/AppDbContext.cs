using JobBoard.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Job> Jobs => Set<Job>();
        public DbSet<JobApplication> JobApplications => Set<JobApplication>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Job>(e =>
            {
                e.HasOne(j => j.Employer)
                 .WithMany(u => u.Jobs)
                 .HasForeignKey(j => j.EmployerId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<JobApplication>(e =>
            {
                e.HasIndex(a => new { a.CandidateId, a.JobId }).IsUnique();
            });

            builder.Entity<ChatMessage>(e =>
            {
                e.HasOne(m => m.Sender)
                 .WithMany()
                 .HasForeignKey(m => m.SenderId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(m => m.Receiver)
                 .WithMany()
                 .HasForeignKey(m => m.ReceiverId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
