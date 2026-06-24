namespace ExcelDataImporter.Application.DTOs;

public record ImportOperationDto
(
    int Id,
    string FileName,
    DateTime ImportedAt,
    int TotalRows,
    int SuccessRows,
    int ErrorRows,
    string Status
);
