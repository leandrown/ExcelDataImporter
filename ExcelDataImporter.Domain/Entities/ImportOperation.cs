using ExcelDataImporter.Domain.Enums;

namespace ExcelDataImporter.Domain.Entities;

public class ImportOperation
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public DateTime ImportedAt { get; set; }
    public int TotalRows { get; set; }
    public int SuccessRows { get; set; }
    public int ErrorRows { get; set; }
    public ImportStatus Status { get; set; }
    public ICollection<ImportRow> Rows { get; set; } = [];
}
