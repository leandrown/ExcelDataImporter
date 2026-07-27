using ClosedXML.Excel;

namespace ExcelDataImporter.Tests.TestHelpers;

/// <summary>
/// Builds in-memory .xlsx workbooks for tests, avoiding dependencies on physical files.
/// </summary>
public class ExcelTestDataBuilder
{
    public static MemoryStream CreateWorkbookStream(params (string Name, string Email, string Phone, string Notes)[] rows)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Sheet1");

        worksheet.Cell(1, 1).Value = "Name";
        worksheet.Cell(1, 2).Value = "Email";
        worksheet.Cell(1, 3).Value = "Phone";
        worksheet.Cell(1, 4).Value = "Notes";

        for (int i = 0; i < rows.Length; i++)
        {
            var r = i + 2;
            worksheet.Cell(r, 1).Value = rows[i].Name;
            worksheet.Cell(r, 2).Value = rows[i].Email;
            worksheet.Cell(r, 3).Value = rows[i].Phone;
            worksheet.Cell(r, 4).Value = rows[i].Notes;
        }

        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return stream;
    }
}
