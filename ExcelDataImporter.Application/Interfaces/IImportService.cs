using ExcelDataImporter.Application.DTOs;

namespace ExcelDataImporter.Application.Interfaces;

public interface IImportService
{
    Task<ImportResultDto> ImportAsync(Stream fileStream, string fileName);
    Task<IEnumerable<ImportOperationDto>> GetAllOperationsAsync();
    Task<IEnumerable<ImportRowDto>> GetRowsByOperationAsync(int operationId);
    Task<IEnumerable<ImportRowDto>> GetErrorsByOperationAsync(int operationId);
}
