using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ValidateIrasId.Application.Contracts.Services;

namespace ValidateIrasId.Functions;

public class ValidateIrasIdFunction
{
    private readonly ILogger<ValidateIrasIdFunction> _logger;
    private readonly IValidateIrasIdService _service;

    public ValidateIrasIdFunction(ILogger<ValidateIrasIdFunction> logger, IValidateIrasIdService service)
    {
        _logger = logger;
        _service = service;
    }

    [Function("fnrspvalidateIRASID")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
    {
        var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
        if (!int.TryParse(query["irasId"], out int irasId))
        {
            _logger.LogWarning("Invalid or missing 'irasId' parameter in request: {Url}", req.Url);
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Missing or invalid 'irasId' parameter.");
            return badResponse;
        }

        var record = await _service.GetRecordByIrasIdAsync(irasId);

        if (record is null)
        {
            var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
            await notFoundResponse.WriteStringAsync($"No record found for IRAS ID: {irasId}");
            return notFoundResponse;
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");
        await response.WriteStringAsync(JsonSerializer.Serialize(record));
        return response;
    }
}