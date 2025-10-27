using System.Diagnostics.CodeAnalysis;
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

        // NOTE: When using ConfigureFunctionsWebApplication, both local.settings.json and User Secrets
        // are automatically added to the configuration. However, there is a bug
        // https://github.com/Azure/azure-functions-dotnet-worker/issues/2230
        // where the order of configuration providers is incorrect, causing User Secrets
        // overriden by local.settings.json values. The work around is
        // to not define configuration in local.settings.json when using User Secrets.

        // Also, there is no need to manually add the following as they are added automatically
        // by ConfigureFunctionsWebApplication. Follwoing code is just left here for reference.

        ///if (builder.Environment.IsDevelopment())
        ///{
        ///    config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
        ///    config.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
        ///}

        if (!builder.Environment.IsDevelopment())
        {
            // Load configuration from Azure App Configuration
            services.AddAzureAppConfiguration(config);
        }

        config.AddEnvironmentVariables();

        // check if we have configuration strings
        var harpProjectDataConnString = config.GetConnectionString("HarpProjectDataConnectionString");

        if (string.IsNullOrWhiteSpace(harpProjectDataConnString))
        {
            throw new InvalidOperationException("HarpProjectDataConnectionString is not configured.");
        }

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