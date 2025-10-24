using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rsp.ValidateIRASID.Application.Constants;
using ValidateIrasId.Application.Contracts.Repositories;
using ValidateIrasId.Application.Contracts.Services;
using ValidateIrasId.Functions;
using ValidateIrasId.Infrastructure;
using ValidateIrasId.Infrastructure.Repositories;
using ValidateIrasId.Services;
using ValidateIrasId.Startup.Configuration;
using ValidateIrasId.Startup.Extensions;

namespace Rsp.ValidateIRASID;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = FunctionsApplication.CreateBuilder(args);

        builder.ConfigureFunctionsWebApplication();

        var config = builder.Configuration;
        var services = builder.Services;

        if (builder.Environment.IsDevelopment())
        {
            builder.Configuration.AddUserSecrets(Assembly.GetAssembly(typeof(Program))!);
        }

        if (!builder.Environment.IsDevelopment())
        {
            // Load configuration from Azure App Configuration
            services.AddAzureAppConfiguration(config);
        }

        config.AddEnvironmentVariables();

        services.AddHeaderPropagation(options => options.Headers.Add(RequestHeadersKeys.CorrelationId));

        // register dependencies
        services.AddMemoryCache();
        services.AddDbContext<HarpProjectDataDbContext>(options =>
        {
            options.UseSqlServer(config.GetConnectionString("HarpProjectDataConnectionString"));
        });

        services.AddScoped<IHarpProjectDataRepository, HarpProjectDataRepository>();
        services.AddScoped<IValidateIrasIdService, ValidateIrasIdService>();
        services.AddScoped<ValidateIrasIdFunction>();

        services.AddHttpContextAccessor();

        var app = builder.Build();

        await app.MigrateAndSeedDatabaseAsync();

        await app.RunAsync();
    }
}