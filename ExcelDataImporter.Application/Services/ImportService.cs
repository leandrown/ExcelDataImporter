using ClosedXML.Excel;
using ExcelDataImporter.Application.DTOs;
using ExcelDataImporter.Application.Helpers;
using ExcelDataImporter.Application.Interfaces;
using ExcelDataImporter.Domain.Entities;
using ExcelDataImporter.Domain.Enums;

namespace ExcelDataImporter.Application.Services;

public class ImportService(IImportRepository repository) : IImportService
{
    public async Task<IEnumerable<ImportOperationDto>> GetAllOperationsAsync()
    {
        var operations = await repository.GetAllAsync();
        return operations.Select(o => new ImportOperationDto(
            o.Id,
            o.FileName,
            o.ImportedAt,
            o.TotalRows,
            o.SuccessRows,
            o.ErrorRows,
            o.Status.ToString()
        ));
    }

    public async Task<IEnumerable<ImportRowDto>> GetErrorsByOperationAsync(int operationId)
    {
        var operation = await repository.GetByIdWithRowsAsync(operationId);
        if (operation is null) return [];
        return operation.Rows.Where(r => r.HasError).Select(MapRowHelper.MapRow);
    }

    public async Task<IEnumerable<ImportRowDto>> GetRowsByOperationAsync(int operationId)
    {
        var operation = await repository.GetByIdWithRowsAsync(operationId);
        if (operation is null) return [];
        return operation.Rows.Select(MapRowHelper.MapRow);
    }

    public async Task<ImportResultDto> ImportAsync(Stream fileStream, string fileName)
    {
        var rows = new List<ImportRow>();

        using var workbook = new XLWorkbook(fileStream);
        var worksheet = workbook.Worksheet(1);
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        // Row 1 is the header - data starts at row 2
        for (int r = 2; r <= lastRow; r++)
        {
            var row = worksheet.Row(r);
            var name = row.Cell(1).GetString().Trim();
            var email = row.Cell(2).GetString().Trim();
            var phone = row.Cell(3).GetString().Trim();
            var notes = row.Cell(4).GetString().Trim();

            var (hasError, errorMessage) = ValidateHelper.Validate(name, email);

            rows.Add(new ImportRow
            {
                RowNumber = r,
                Name = name,
                Email = email,
                Phone = phone,
                Notes = notes,
                HasError = hasError,
                ErrorMessage = errorMessage
            });
        }

        var operation = new ImportOperation
        {
            FileName = fileName,
            ImportedAt = DateTime.UtcNow,
            TotalRows = rows.Count,
            SuccessRows = rows.Count(r => !r.HasError),
            ErrorRows = rows.Count(r => r.HasError),
            Status = rows.Any(r => r.HasError) ? rows.All(r => r.HasError) ? ImportStatus.Failed : ImportStatus.CompletedWithErrors : ImportStatus.Completed,
            Rows = rows
        };

        var saved = await repository.AddAsync(operation);

        return new ImportResultDto
        (
            saved.Id,
            saved.FileName,
            saved.TotalRows,
            saved.SuccessRows,
            saved.ErrorRows,
            saved.Status.ToString(),
            rows.Where(r => r.HasError).Select(MapRowHelper.MapRow).ToList()
        );
    }
}
