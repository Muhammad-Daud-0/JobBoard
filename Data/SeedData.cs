using JobBoard.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace JobBoard.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var db = services.GetRequiredService<AppDbContext>();

            string[] roles = { "Admin", "Employer", "Candidate" };
            foreach (var role in roles)
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

            // Seed admin user
            if (await userManager.FindByEmailAsync("admin@jobboard.com") == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@jobboard.com",
                    Email = "admin@jobboard.com",
                    FullName = "Admin User",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(admin, "Admin@123!");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Seed categories
            if (!db.Categories.Any())
            {
                db.Categories.AddRange(
                    new Category { Name = "Software Development" },
                    new Category { Name = "Design" },
                    new Category { Name = "Marketing" },
                    new Category { Name = "Finance" }
                );
                await db.SaveChangesAsync();
            }
        }
    }
}
