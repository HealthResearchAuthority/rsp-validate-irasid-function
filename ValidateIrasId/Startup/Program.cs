using System.Diagnostics.CodeAnalysis;
using System.Net;
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

        config
            .AddJsonFile("local.settings.json", true)
            .AddEnvironmentVariables();

        builder.Services.AddHeaderPropagation(options => options.Headers.Add(RequestHeadersKeys.CorrelationId));

        // register dependencies
        builder.Services.AddMemoryCache();
        builder.Services.AddDbContext<HarpProjectDataDbContext>(options =>
        {
            options.UseSqlServer(config.GetConnectionString("HarpProjectDataConnectionString"));
        });

        builder.Services.AddScoped<IHarpProjectDataRepository, HarpProjectDataRepository>();
        builder.Services.AddScoped<IValidateIrasIdService, ValidateIrasIdService>();
        builder.Services.AddScoped<ValidateIrasIdFunction>();

        builder.Services.AddHttpContextAccessor();

        if (!builder.Environment.IsDevelopment())
        {
            // Load configuration from Azure App Configuration

            builder.Services.AddAzureAppConfiguration(config);
        }

        var app = builder.Build();

        await app.MigrateAndSeedDatabaseAsync();

        HttpClient.DefaultProxy = new WebProxy();
        await app.RunAsync();
    }
}