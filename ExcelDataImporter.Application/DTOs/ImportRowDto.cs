namespace ExcelDataImporter.Application.DTOs;

public record ImportRowDto
(
    int RowNumber,
    string? Name,
    string? Email,
    string? Phone,
    string? Notes,
    bool HasError,
    string? ErrorMessage
);
