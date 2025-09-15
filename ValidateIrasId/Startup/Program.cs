using System.Diagnostics.CodeAnalysis;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rsp.ValidateIRASID.Application.Configuration;
using Rsp.ValidateIRASID.Application.Constants;
using ValidateIrasId.Application.Contracts.Repositories;
using ValidateIrasId.Application.Contracts.Services;
using ValidateIrasId.Infrastructure;
using ValidateIrasId.Infrastructure.Repositories;
using ValidateIrasId.Services;
using ValidateIrasId.Startup.Extensions;

namespace Rsp.ValidateIRASID;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = FunctionsApplication.CreateBuilder(args);
        builder.ConfigureFunctionsWebApplication();

        var configuration = builder.Configuration;
        var config = builder.Configuration;

        config
            .AddJsonFile("local.settings.json", true)
            .AddEnvironmentVariables();

        builder.Services.AddHeaderPropagation(options => options.Headers.Add(RequestHeadersKeys.CorrelationId));

        if (!builder.Environment.IsDevelopment())
        {
            var azureAppConfigSection = configuration.GetSection(nameof(AppSettings));
            var azureAppConfiguration = azureAppConfigSection.Get<AppSettings>();

            // Load configuration from Azure App Configuration
            builder.Configuration.AddAzureAppConfiguration(options =>
            {
                options.Connect
                    (
                        new Uri(azureAppConfiguration!.AzureAppConfiguration.Endpoint),
                        new ManagedIdentityCredential(azureAppConfiguration.AzureAppConfiguration.IdentityClientId)
                    )
                    .Select(KeyFilter.Any);
            });

            builder.Services.AddAzureAppConfiguration();
        }

        // register dependencies
        builder.Services.AddMemoryCache();
        builder.Services.AddDbContext<HarpProjectDataDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("validateIRASIDDatabaseConnection"));
        });

        builder.Services.AddScoped<IHarpProjectDataRepository, HarpProjectDataRepository>();
        builder.Services.AddScoped<IValidateIrasIdService, ValidateIrasIdService>();

        builder.Services.AddHttpContextAccessor();

        var app = builder.Build();

        await app.MigrateAndSeedDatabaseAsync();

        await app.RunAsync();
    }
}