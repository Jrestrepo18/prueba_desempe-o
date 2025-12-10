using Microsoft.EntityFrameworkCore;
using TalentoPlus.Web.Data;

namespace TalentoPlus.Web
{
    public class MigrationHelper
    {
        public static async Task MigrateDatabaseAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                // Apply pending migrations
                var pendingMigrations = (await context.Database.GetPendingMigrationsAsync()).ToList();
                
                if (pendingMigrations.Any())
                {
                    Console.WriteLine($"Applying {pendingMigrations.Count} pending migrations...");
                    await context.Database.MigrateAsync();
                    Console.WriteLine("✅ Migrations applied successfully!");
                }
                else
                {
                    Console.WriteLine("✅ Database is up to date!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error during migration: {ex.Message}");
                throw;
            }
        }
    }
}
