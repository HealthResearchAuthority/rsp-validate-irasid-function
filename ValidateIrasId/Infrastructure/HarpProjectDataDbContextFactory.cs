using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace ValidateIrasId.Infrastructure
{
    public class HarpProjectDataDbContextFactory
        : IDesignTimeDbContextFactory<HarpProjectDataDbContext>
    {
        public HarpProjectDataDbContext CreateDbContext(string[] args)
        {
            // 1. Try environment variable first (used in CI/CD pipeline)
            var envConn = Environment.GetEnvironmentVariable("HarpProjectDataConnectionString");

            if (!string.IsNullOrWhiteSpace(envConn))
            {
                return BuildDbContext(envConn);
            }

            // 2. Fallback to local.settings.json for local development
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var localConn = config.GetConnectionString("HarpProjectDataConnectionString");

            if (string.IsNullOrWhiteSpace(localConn))
            {
                throw new InvalidOperationException(
                    "Could not find connection string. " +
                    "Set HarpProjectDataConnectionString environment variable OR " +
                    "ensure local.settings.json contains it under ConnectionStrings.");
            }

            return BuildDbContext(localConn);
        }

        private HarpProjectDataDbContext BuildDbContext(string conn)
        {
            var optionsBuilder = new DbContextOptionsBuilder<HarpProjectDataDbContext>();
            optionsBuilder.UseSqlServer(conn);

            return new HarpProjectDataDbContext(optionsBuilder.Options);
        }
    }
}