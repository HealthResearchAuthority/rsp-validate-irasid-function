using Microsoft.EntityFrameworkCore;
using ValidateIrasId.Application.DTO;
using ValidateIrasId.Infrastructure;
using ValidateIrasId.Infrastructure.Repositories;

namespace ValidateIrasIdTests;

public class HarpProjectDataRepositoryTests
{
    private readonly HarpProjectDataDbContext _context;
    private readonly HarpProjectDataRepository _repository;

    public HarpProjectDataRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<HarpProjectDataDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        _context = new HarpProjectDataDbContext(options);
        _repository = new HarpProjectDataRepository(_context);

        SeedData();
    }

    private void SeedData()
    {
        var records = new List<HarpProjectRecord>
        {
            new HarpProjectRecord
            {
                Id = "abc123",
                IrasId = 45655,
                RecID = 789,
                RecName = "Dr. Test",
                ShortStudyTitle = "Short Title",
                StudyDecision = "Approved",
                DateRegistered = new DateTime(2023, 5, 10, 0, 0, 0, DateTimeKind.Utc),
                FullResearchTitle = "Full Title of the Research Study"
            },
            new HarpProjectRecord
            {
                Id = "def456",
                IrasId = 99955,
                RecID = null,
                RecName = null,
                ShortStudyTitle = null,
                StudyDecision = null,
                DateRegistered = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                FullResearchTitle = null
            }
        };

        _context.HarpProjectRecords.AddRange(records);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetRecordByIrasIdAsync_ReturnsFullDtoCorrectly()
    {
        // Act
        var result = await _repository.GetRecordByIrasIdAsync(45655);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("abc123", result!.Id);
        Assert.Equal(45655, result.IrasId);
        Assert.Equal(789, result.RecID);
        Assert.Equal("Dr. Test", result.RecName);
        Assert.Equal("Short Title", result.ShortStudyTitle);
        Assert.Equal("Approved", result.StudyDecision);
        Assert.Equal(new DateTime(2023, 5, 10, 0, 0, 0, DateTimeKind.Utc), result.DateRegistered);
        Assert.Equal("Full Title of the Research Study", result.FullResearchTitle);
    }

    [Fact]
    public async Task GetRecordByIrasIdAsync_ReturnsRecordWithNullables()
    {
        // Act
        var result = await _repository.GetRecordByIrasIdAsync(99955);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("def456", result!.Id);
        Assert.Equal(99955, result.IrasId);
        Assert.Null(result.RecID);
        Assert.Null(result.RecName);
        Assert.Null(result.ShortStudyTitle);
        Assert.Null(result.StudyDecision);
        Assert.Equal(new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc), result.DateRegistered);
        Assert.Null(result.FullResearchTitle);
    }

    [Fact]
    public async Task GetRecordByIrasIdAsync_ReturnsNull_WhenNotFound()
    {
        // Act
        var result = await _repository.GetRecordByIrasIdAsync(9999999);

        // Assert
        Assert.Null(result);
    }
}