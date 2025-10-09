using Microsoft.EntityFrameworkCore;
using Shouldly;
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
        result.ShouldNotBeNull();
        result!.Id.ShouldBe("abc123");
        result.IrasId.ShouldBe(45655);
        result.RecID.ShouldBe(789);
        result.RecName.ShouldBe("Dr. Test");
        result.ShortStudyTitle.ShouldBe("Short Title");
        result.StudyDecision.ShouldBe("Approved");
        result.DateRegistered.ShouldBe(new DateTime(2023, 5, 10, 0, 0, 0, DateTimeKind.Utc));
        result.FullResearchTitle.ShouldBe("Full Title of the Research Study");
    }

    [Fact]
    public async Task GetRecordByIrasIdAsync_ReturnsRecordWithNullables()
    {
        // Act
        var result = await _repository.GetRecordByIrasIdAsync(99955);

        // Assert
        result.ShouldNotBeNull();
        result!.Id.ShouldBe("def456");
        result.IrasId.ShouldBe(99955);
        result.RecID.ShouldBeNull();
        result.RecName.ShouldBeNull();
        result.ShortStudyTitle.ShouldBeNull();
        result.StudyDecision.ShouldBeNull();
        result.DateRegistered.ShouldBe(new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        result.FullResearchTitle.ShouldBeNull();
    }

    [Fact]
    public async Task GetRecordByIrasIdAsync_ReturnsNull_WhenNotFound()
    {
        // Act
        var result = await _repository.GetRecordByIrasIdAsync(9999999);

        // Assert
        result.ShouldBeNull();
    }
}