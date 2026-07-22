using ExcelDataImporter.Application.DTOs;
using ExcelDataImporter.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExcelDataImporter.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ImportController(IImportService importService, ILogger<ImportController> logger) : ControllerBase
{
    /// <summary>Upload and import an Excel (.xlsx) file.</summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType<ImportResultDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Import(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            logger.LogWarning("Import request rejected: no file provided.");
            return BadRequest("No file provided.");
        }

        if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning("Import request rejected: unsupported file type for {FileName}.", file.FileName);
            return BadRequest("Invalid file format. Please upload an Excel (.xlsx) file.");
        }

        logger.LogInformation("Received import request for {FileName} ({FileSize} bytes).", file.FileName, file.Length);

        try
        {
            using var stream = file.OpenReadStream();
            var result = await importService.ImportAsync(stream, file.FileName);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error while importing {FileName}.", file.FileName);
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpectederror occurred while processing the file.");
        }
    }

    /// <summary>List all import operations.</summary>
    [HttpGet]
    [ProducesResponseType<IEnumerable<ImportOperationDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var operations = await importService.GetAllOperationsAsync();
        return Ok(operations);
    }

    /// <summary>Get all rows from a specific import operation.</summary>
    [HttpGet("{id:int}/records")]
    [ProducesResponseType<IEnumerable<ImportRowDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRecords(int id)
    {
        var rows = await importService.GetRowsByOperationAsync(id);
        if (!rows.Any()) return NotFound();
        return Ok(rows);
    }

    /// <summary>Get only error rows from a specific import operation.</summary>
    [HttpGet("{id:int}/errors")]
    [ProducesResponseType<IEnumerable<ImportRowDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetErrors(int id)
    {
        var errors = await importService.GetErrorsByOperationAsync(id);
        if (!errors.Any()) return NotFound();
        return Ok(errors);
    }
}
