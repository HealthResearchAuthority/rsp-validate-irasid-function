using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ValidateIrasId.Infrastructure;

namespace ValidateIrasId.Startup.Extensions;

/// <summary>
/// Define an extension method on IHost to support migrating and seeding the database
/// </summary>
public static class HostExtensions
{
    /// <summary>
    /// Migrates and seed the database.
    /// </summary>
    /// <param name="app">The IHost instance</param>
    public static async Task MigrateAndSeedDatabaseAsync(this IHost app)
    {
        var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("Startup");

        try
        {
            logger.LogInformation("Performing Migrations");

            using var scope = app.Services.CreateScope();

            await using var context = scope.ServiceProvider.GetRequiredService<HarpProjectDataDbContext>();

            await context.Database.MigrateAsync();

            logger.LogInformation("Migrations Completed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database Migration failed");
        }
    }
}