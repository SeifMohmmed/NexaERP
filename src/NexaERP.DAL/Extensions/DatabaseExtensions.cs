using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NexaERP.DAL.Database;

namespace NexaERP.DAL.Extensions;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrationsAsync(this IServiceProvider services)
    {
        using IServiceScope scope = services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        ILogger logger =
            scope.ServiceProvider
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("DatabaseMigration");

        try
        {
            await dbContext.Database.MigrateAsync();

            logger.LogInformation(
                "Application database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while applying database migrations.");

            throw;
        }
    }

}
