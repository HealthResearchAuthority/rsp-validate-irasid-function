using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ValidateIrasId.Application.Contracts.Services;
using ValidateIrasId.Application.DTO;

namespace ValidateIrasId.Functions;

/// <summary>
/// Azure Function that validates a HARP project record exists for a given IRAS ID.
/// </summary>
/// <remarks>
/// - Trigger: HTTP GET
/// - Route: /projectrecord/validate
/// - Query: ?irasId={integer}
///
/// Responses:
/// - 200 OK: Record found and returned in <see cref="ProjectRecordValidationResponse.Data"/>
/// - 400 Bad Request: Missing or invalid 'irasId' query parameter
/// - 404 Not Found: No record found for the provided IRAS ID
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="ValidateIrasIdFunction"/> class.
/// </remarks>
/// <param name="logger">Typed logger for diagnostic logging.</param>
/// <param name="service">Service used to retrieve project records by IRAS ID.</param>
public class ValidateIrasIdFunction(ILogger<ValidateIrasIdFunction> logger, IValidateIrasIdService service)
{
    /// <summary>
    /// Validates whether a project record exists for the supplied IRAS ID and returns the record if found.
    /// </summary>
    /// <param name="req">The incoming HTTP request data.</param>
    /// <param name="irasId">
    /// IRAS ID provided as a query string parameter (e.g., ?irasId=12345) or bound route value.
    /// </param>
    /// <returns>
    /// An <see cref="IActionResult"/> containing a <see cref="ProjectRecordValidationResponse"/>
    /// with HTTP 200, 400, or 404 depending on the outcome.
    /// </returns>
    /// <example>
    /// GET /api/projectrecord/validate?irasId=12345
    /// </example>
    [Function("ProjectRecordValidation")]
    public async Task<IActionResult> Run
    (
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "projectrecord/validate")] HttpRequest req,
        string? irasId
    )
    {
        // Always include a timestamp in the response for traceability and client-side diagnostics.
        var validationResponse = new ProjectRecordValidationResponse
        {
            TimeStamp = DateTime.UtcNow
        };

        // Validate that the provided 'irasId' can be parsed to an integer.
        // Return 400 Bad Request if the parameter is missing or invalid.
        if (string.IsNullOrWhiteSpace(irasId) || !int.TryParse(irasId, out var irasIdValue))
        {
            logger.LogWarning("Invalid or missing 'irasId' parameter in request: {Url}", req.GetDisplayUrl());

            validationResponse.Error = "Missing or invalid 'irasId' parameter.";

            return new BadRequestObjectResult(validationResponse);
        }

        // Query the backing service for a project record matching the provided IRAS ID.
        var projectRecord = await service.GetRecordByIrasIdAsync(irasIdValue);

        // If no record is found, return 404 Not Found with a descriptive error.
        if (projectRecord is null)
        {
            validationResponse.Error = $"No record found for IRAS ID: {irasId}";

            return new NotFoundObjectResult(validationResponse);
        }

        // On success, include the found record in the response and return 200 OK.
        validationResponse.Data = projectRecord;

        return new OkObjectResult(validationResponse);
    }
}