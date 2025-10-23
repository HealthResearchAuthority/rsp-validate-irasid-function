using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using Shouldly;
using ValidateIrasId.Application.Contracts.Services;
using ValidateIrasId.Application.DTO;
using ValidateIrasId.Functions;

namespace ValidateIrasIdTests;

public class ValidateIrasIdFunctionTests : TestServiceBase<ValidateIrasIdFunction>
{
    [Fact]
    public async Task Run_ReturnsBadRequest_WhenIrasIdIsMissing()
    {
        var request = GenerateHttpRequest(new Uri("http://localhost/api?wrongParam=123"));

        var response = await Sut.Run(request, request.Query["irasId"]);

        var result = response.ShouldBeOfType<BadRequestObjectResult>();

        result.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        var model = result.Value
            .ShouldNotBeNull()
            .ShouldBeOfType<ProjectRecordValidationResponse>();

        model.Error.ShouldBe("Missing or invalid 'irasId' parameter.");
        model.Data.ShouldBeNull();
    }

    [Fact]
    public async Task Run_ReturnsNotFound_WhenRecordDoesNotExist()
    {
        var serviceMock = Mocker.GetMock<IValidateIrasIdService>();
        serviceMock
            .Setup(r => r.GetRecordByIrasIdAsync(It.IsAny<int>()))
            .ReturnsAsync((HarpProjectRecordDataDTO?)null);

        var request = GenerateHttpRequest(new Uri("http://localhost/api?irasId=999999"));

        var response = await Sut.Run(request, request.Query["irasId"]);

        var result = response.ShouldBeOfType<NotFoundObjectResult>();

        result.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        var model = result.Value
            .ShouldNotBeNull()
            .ShouldBeOfType<ProjectRecordValidationResponse>();

        model.Error.ShouldBe("No record found for IRAS ID: 999999");
        model.Data.ShouldBeNull();
    }

    [Fact]
    public async Task Run_ReturnsOk_WhenRecordExists()
    {
        var testRecord = new HarpProjectRecordDataDTO
        {
            IRASID = 123456,
            RecID = 316,
            RecName = "Test Committee",
            ShortProjectTitle = "Test Study",
            LongProjectTitle = "Full Research Title Example"
        };

        var serviceMock = Mocker.GetMock<IValidateIrasIdService>();
        serviceMock
            .Setup(r => r.GetRecordByIrasIdAsync(123456))
            .ReturnsAsync(testRecord);

        var request = GenerateHttpRequest(new Uri("http://localhost/api?irasId=123456"));

        var response = await Sut.Run(request, request.Query["irasId"]);

        var result = response.ShouldBeOfType<OkObjectResult>();

        result.StatusCode.ShouldBe(StatusCodes.Status200OK);
        var model = result.Value
            .ShouldNotBeNull()
            .ShouldBeOfType<ProjectRecordValidationResponse>();

        model.Data.ShouldNotBeNull();
        model.Data.IRASID.ShouldBe(testRecord.IRASID);
        model.Data.RecID.ShouldBe(testRecord.RecID);
        model.Data.RecName.ShouldBe(testRecord.RecName);
        model.Data.ShortProjectTitle.ShouldBe(testRecord.ShortProjectTitle);
        model.Data.LongProjectTitle.ShouldBe(testRecord.LongProjectTitle);
    }

    private static HttpRequest GenerateHttpRequest(Uri uri)
    {
        var context = new DefaultHttpContext();
        var request = context.Request;

        request.Scheme = uri.Scheme;
        request.Host = new HostString(uri.Host, uri.Port);
        request.Path = uri.PathAndQuery;

        var parameters = new Dictionary<string, StringValues>();
        var queryParams = HttpUtility.ParseQueryString(uri.Query.TrimStart('?'));

        foreach (string key in queryParams)
        {
            parameters.Add(key, queryParams[key]);
        }

        request.Query = new QueryCollection(parameters);
        return request;
    }
}