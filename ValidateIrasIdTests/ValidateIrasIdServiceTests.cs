using Microsoft.Extensions.Logging;
using Moq;
using ValidateIrasId.Application.Contracts.Repositories;
using ValidateIrasId.Application.DTO;
using ValidateIrasId.Services;

namespace ValidateIrasIdTests;

public class ValidateIrasIdServiceTests
{
    private readonly Mock<IHarpProjectDataRepository> _repositoryMock;
    private readonly Mock<ILogger<ValidateIrasIdService>> _loggerMock;
    private readonly ValidateIrasIdService _service;

    public ValidateIrasIdServiceTests()
    {
        _repositoryMock = new Mock<IHarpProjectDataRepository>();
        _loggerMock = new Mock<ILogger<ValidateIrasIdService>>();
        _service = new ValidateIrasIdService(_loggerMock.Object, _repositoryMock.Object);
    }

    [Fact]
    public async Task GetRecordByIrasIdAsync_ReturnsRecord_WhenExists()
    {
        // Arrange
        var irasId = 1234;
        var expectedRecord = new HarpProjectRecord
        {
            Id = "abc",
            IrasId = irasId,
            RecID = 1,
            RecName = "Test",
            ShortStudyTitle = "Short",
            StudyDecision = "Approved",
            DateRegistered = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            FullResearchTitle = "Full Title"
        };

        _repositoryMock
            .Setup(r => r.GetRecordByIrasIdAsync(irasId))
            .ReturnsAsync(expectedRecord);

        // Act
        var result = await _service.GetRecordByIrasIdAsync(irasId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedRecord.IrasId, result!.IrasId);
        Assert.Equal(expectedRecord.RecName, result.RecName);
    }

    [Fact]
    public async Task GetRecordByIrasIdAsync_ReturnsNull_WhenNotFound()
    {
        // Arrange
        var irasId = 999;
        _repositoryMock
            .Setup(r => r.GetRecordByIrasIdAsync(irasId))
            .ReturnsAsync((HarpProjectRecord?)null);

        // Act
        var result = await _service.GetRecordByIrasIdAsync(irasId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetRecordByIrasIdAsync_LogsInformation()
    {
        // Arrange
        var irasId = 123;
        _repositoryMock
            .Setup(r => r.GetRecordByIrasIdAsync(irasId))
            .ReturnsAsync(new HarpProjectRecord { IrasId = irasId });

        // Act
        await _service.GetRecordByIrasIdAsync(irasId);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Fetching record for IRAS ID")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}