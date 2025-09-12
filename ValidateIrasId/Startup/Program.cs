using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ValidateIrasId.Application.Contracts.Repositories;
using ValidateIrasId.Application.Contracts.Services;
using ValidateIrasId.Infrastructure;
using ValidateIrasId.Infrastructure.Repositories;
using ValidateIrasId.Services;
using ValidateIrasId.Startup.Extensions;

namespace ValidateIrasId.Startup;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = FunctionsApplication.CreateBuilder(args);
        builder.ConfigureFunctionsWebApplication();

        var config = builder.Configuration;

        config
            .AddJsonFile("local.settings.json", true)
            .AddEnvironmentVariables();

        // register dependencies
        builder.Services.AddMemoryCache();
        builder.Services.AddDbContext<HarpProjectDataDbContext>(options =>
        {
            options.UseSqlServer(config.GetConnectionString("HarpProjectDataConnectionString"));
        });

        builder.Services.AddScoped<IHarpProjectDataRepository, HarpProjectDataRepository>();
        builder.Services.AddScoped<IValidateIrasIdService, ValidateIrasIdService>();

        builder.Services.AddHttpContextAccessor();

        var app = builder.Build();

        await app.MigrateAndSeedDatabaseAsync();

        await app.RunAsync();
    }
}