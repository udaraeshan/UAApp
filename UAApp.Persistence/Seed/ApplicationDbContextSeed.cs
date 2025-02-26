using UAApp.Domain.Entities;
using UAApp.Persistence.DB;

namespace UAApp.Persistence.Seed
{
    public static class ApplicationDbContextSeed
    {
        public static async Task<Task> SeedDataAsync(ApplicationDbContext context)
        {
            if (!context.ApplicationUsers.Any())
            {
                var applicationUser = new ApplicationUser { PhoneNumber = "0272104803", FullName = "Global Administrator", IsActive = true, Password = BCrypt.Net.BCrypt.HashPassword("Ga@123"), UserEmail = "ga@ua.com", CreatedBy = "System", CreatedByName = "System", CreatedDate = DateTime.Now.ToUniversalTime(), UpdatedBy = "System", UpdatedByName = "System", UpdatedDate = DateTime.Now.ToUniversalTime() };
                context.ApplicationUsers.Add(applicationUser);
                await context.SaveChangesAsync();
            }
            return Task.CompletedTask;
        }
    }
}
