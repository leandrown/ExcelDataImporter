using ExcelDataImporter.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExcelDataImporter.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ImportOperation> ImportOperations => Set<ImportOperation>();
    public DbSet<ImportRow> ImportRows => Set<ImportRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ImportOperation>(e =>
        {
            e.HasKey(o => o.Id);
            e.Property(o => o.FileName).HasMaxLength(200).IsRequired();
            e.Property(o => o.Status).HasConversion<string>();
        });

        modelBuilder.Entity<ImportRow>(e =>
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Name).HasMaxLength(200);
            e.Property(r => r.Email).HasMaxLength(200);
            e.Property(r => r.Phone).HasMaxLength(50);
            e.Property(r => r.ErrorMessage).HasMaxLength(500);

            e.HasOne(r => r.ImportOperation)
             .WithMany(o => o.Rows)
             .HasForeignKey(r => r.ImportOperationId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
