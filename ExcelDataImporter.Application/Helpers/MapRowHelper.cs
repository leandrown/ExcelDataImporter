using ExcelDataImporter.Application.DTOs;
using ExcelDataImporter.Domain.Entities;

namespace ExcelDataImporter.Application.Helpers;

public static class MapRowHelper
{
    public static ImportRowDto MapRow(ImportRow r) =>
        new(r.RowNumber, r.Name, r.Email, r.Phone, r.Notes, r.HasError, r.ErrorMessage);
}
