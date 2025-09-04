using Microsoft.EntityFrameworkCore;
using ValidateIrasId.Application.DTO;
using ValidateIrasId.Infrastructure.EntitiesConfiguration;

namespace ValidateIrasId.Infrastructure;

public class HarpProjectDataDbContext(DbContextOptions<HarpProjectDataDbContext> options) : DbContext(options)
{
    public DbSet<HarpProjectRecord> HarpProjectRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new HarpProjectRecordConfiguration());
    }
}