using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ValidateIrasId.Application.Contracts.Services;
using ValidateIrasId.Application.DTO;

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
        string? irasIdKey = query.AllKeys.FirstOrDefault(k => string.Equals(k, "irasId", StringComparison.OrdinalIgnoreCase));

        var responseObj = new ValidateIrasIdResponse
        {
            TimeStamp = DateTime.UtcNow
        };

        if (!int.TryParse(query[irasIdKey], out int irasId))
        {
            _logger.LogWarning("Invalid or missing 'irasId' parameter in request: {Url}", req.Url);
            responseObj.Status = "BadRequest";
            responseObj.Error = "Missing or invalid 'irasId' parameter.";

            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync(JsonSerializer.Serialize(responseObj));
            return badResponse;
        }

        var record = await _service.GetRecordByIrasIdAsync(irasId);

        if (record is null)
        {
            responseObj.Status = "NotFound";
            responseObj.Error = $"No record found for IRAS ID: {irasId}";

            var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
            await notFoundResponse.WriteStringAsync(JsonSerializer.Serialize(responseObj));
            return notFoundResponse;
        }

        responseObj.Status = "Success";
        responseObj.Data = record;

        var okResponse = req.CreateResponse(HttpStatusCode.OK);
        okResponse.Headers.Add("Content-Type", "application/json");
        await okResponse.WriteStringAsync(JsonSerializer.Serialize(responseObj));
        return okResponse;
    }
}