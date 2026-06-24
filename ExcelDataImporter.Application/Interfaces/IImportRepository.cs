using ExcelDataImporter.Domain.Entities;

namespace ExcelDataImporter.Application.Interfaces;

public interface IImportRepository
{
    Task<ImportOperation> AddAsync(ImportOperation operation);
    Task<IEnumerable<ImportOperation>> GetAllAsync();
    Task<ImportOperation?> GetByIdWithRowsAsync(int id);
}
