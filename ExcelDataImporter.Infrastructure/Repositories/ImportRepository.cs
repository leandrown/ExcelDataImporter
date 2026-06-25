using ExcelDataImporter.Application.Interfaces;
using ExcelDataImporter.Domain.Entities;
using ExcelDataImporter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExcelDataImporter.Infrastructure.Repositories;

public class ImportRepository(AppDbContext context) : IImportRepository
{
    public async Task<ImportOperation> AddAsync(ImportOperation operation)
    {
        context.ImportOperations.Add(operation);
        await context.SaveChangesAsync();
        return operation;
    }

    public async Task<IEnumerable<ImportOperation>> GetAllAsync()
    {
        return await context.ImportOperations.OrderByDescending(o => o.ImportedAt).ToListAsync();
    }

    public async Task<ImportOperation?> GetByIdWithRowsAsync(int id)
    {
        return await context.ImportOperations.Include(o => o.Rows).FirstOrDefaultAsync(o => o.Id == id);
    }
}
