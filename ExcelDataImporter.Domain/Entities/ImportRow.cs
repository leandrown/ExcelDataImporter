namespace ExcelDataImporter.Domain.Entities;

public class ImportRow
{
    public int Id { get; set; }
    public int ImportOperationId { get; set; }
    public ImportOperation ImportOperation { get; set; } = null!;
    public int RowNumber { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Notes { get; set; }
    public bool HasError { get; set; }
    public string? ErrorMessage { get; set; }
}
