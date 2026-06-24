namespace ExcelDataImporter.Application.DTOs;

public record ImportResultDto
(
    int OperationId,
    string FileName,
    int TotalRows,
    int SuccessRows,
    int ErrorRows,
    string Status,
    List<ImportRowDto> Errors
);
