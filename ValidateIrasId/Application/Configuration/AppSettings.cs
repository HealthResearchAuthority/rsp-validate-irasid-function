namespace Rsp.ValidateIRASID.Application.Configuration;

public class AppSettings
{
    /// <summary>
    /// Label to use when reading App Configuration from AzureAppConfiguration
    /// </summary>
    public const string ServiceLabel = "validateirasidfunction";
    public AzureAppConfiguration AzureAppConfiguration { get; set; } = null!;
}