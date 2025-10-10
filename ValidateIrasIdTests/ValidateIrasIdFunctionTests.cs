using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using ValidateIrasId.Application.Contracts.Services;
using ValidateIrasId.Application.DTO;
using ValidateIrasId.Functions;

namespace ValidateIrasIdTests;

public class ValidateIrasIdFunctionTests
{
    [Fact]
    public async Task Run_ReturnsBadRequest_WhenIrasIdIsMissing()
    {
        var loggerMock = new Mock<ILogger<ValidateIrasIdFunction>>();
        var serviceMock = new Mock<IValidateIrasIdService>();
        var function = new ValidateIrasIdFunction(loggerMock.Object, serviceMock.Object);

        var contextMock = new Mock<FunctionContext>();
        var request = new FakeHttpRequestData(contextMock.Object, new Uri("http://localhost/api?wrongParam=123"));

        var response = await function.Run(request);

        var fakeResponse = response.ShouldBeOfType<FakeHttpResponseData>();
        fakeResponse.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var responseBody = fakeResponse.GetBodyAsString();
        responseBody.ShouldContain("\"Status\":\"BadRequest\"");
        responseBody.ShouldContain("Missing or invalid");
    }

    [Fact]
    public async Task Run_ReturnsNotFound_WhenRecordDoesNotExist()
    {
        var loggerMock = new Mock<ILogger<ValidateIrasIdFunction>>();
        var serviceMock = new Mock<IValidateIrasIdService>();
        serviceMock.Setup(r => r.GetRecordByIrasIdAsync(It.IsAny<int>()))
                   .ReturnsAsync((HarpProjectRecordDataDTO?)null);

        var function = new ValidateIrasIdFunction(loggerMock.Object, serviceMock.Object);

        var contextMock = new Mock<FunctionContext>();
        var request = new FakeHttpRequestData(contextMock.Object, new Uri("http://localhost/api?irasId=999999"));

        var response = await function.Run(request);

        var fakeResponse = Assert.IsType<FakeHttpResponseData>(response);
        fakeResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var responseBody = fakeResponse.GetBodyAsString();
        responseBody.ShouldContain("\"Status\":\"NotFound\"");
        responseBody.ShouldContain("No record found for IRAS ID: 999999");
    }

    [Fact]
    public async Task Run_ReturnsOk_WhenRecordExists()
    {
        var loggerMock = new Mock<ILogger<ValidateIrasIdFunction>>();
        var serviceMock = new Mock<IValidateIrasIdService>();

        var testRecord = new HarpProjectRecordDataDTO
        {
            IRASID = 123456,
            RecID = 316,
            RecName = "Test Committee",
            ShortProjectTitle = "Test Study",
            LongProjectTitle = "Full Research Title Example"
        };

        serviceMock.Setup(r => r.GetRecordByIrasIdAsync(123456))
                   .ReturnsAsync(testRecord);

        var function = new ValidateIrasIdFunction(loggerMock.Object, serviceMock.Object);

        var contextMock = new Mock<FunctionContext>();
        var request = new FakeHttpRequestData(contextMock.Object, new Uri("http://localhost/api?irasId=123456"));

        var response = await function.Run(request);

        var fakeResponse = Assert.IsType<FakeHttpResponseData>(response);
        fakeResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        fakeResponse.Headers.GetValues("Content-Type").FirstOrDefault().ShouldBe("application/json");

        var responseBody = fakeResponse.GetBodyAsString();
        responseBody.ShouldContain("\"Status\":\"Success\"");
        responseBody.ShouldContain("\"IRASID\":123456");
        responseBody.ShouldContain("\"RecName\":\"Test Committee\"");
        responseBody.ShouldContain("\"ShortProjectTitle\":\"Test Study\"");
        responseBody.ShouldContain("\"LongProjectTitle\":\"Full Research Title Example\"");
    }
}

public class FakeHttpResponseData : HttpResponseData
{
    private readonly MemoryStream _bodyStream = new MemoryStream();

    public FakeHttpResponseData(FunctionContext req, HttpStatusCode statusCode) : base(req)
    {
        StatusCode = statusCode;
        Headers = new HttpHeadersCollection();
        Body = _bodyStream;
    }

    public override HttpStatusCode StatusCode { get; set; }
    public override HttpHeadersCollection Headers { get; set; }
    public override Stream Body { get; set; }
    public override HttpCookies Cookies => null;

    public async Task WriteStringAsync(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        await Body.WriteAsync(bytes, 0, bytes.Length);
        Body.Position = 0;
    }

    public string GetBodyAsString()
    {
        Body.Position = 0;
        return new StreamReader(Body).ReadToEnd();
    }
}

public class FakeHttpRequestData : HttpRequestData
{
    public FakeHttpRequestData(FunctionContext context, Uri uri, HttpStatusCode statusCode = HttpStatusCode.OK) : base(context)
    {
        Url = uri;
        Headers = new HttpHeadersCollection();
        Body = new MemoryStream();
        StatusCode = statusCode;
    }

    public override Stream Body { get; }
    public override HttpHeadersCollection Headers { get; }
    public override Uri Url { get; }

    public override string Method => "GET";

    public HttpStatusCode StatusCode { get; set; }

    public override IEnumerable<ClaimsIdentity> Identities => Array.Empty<ClaimsIdentity>();

    public override IReadOnlyCollection<IHttpCookie> Cookies => Array.Empty<IHttpCookie>();

    public override HttpResponseData CreateResponse()
    {
        return new FakeHttpResponseData(FunctionContext, StatusCode);
    }
}