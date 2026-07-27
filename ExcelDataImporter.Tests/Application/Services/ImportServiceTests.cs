using ExcelDataImporter.Application.Interfaces;
using ExcelDataImporter.Application.Services;
using ExcelDataImporter.Domain.Entities;
using ExcelDataImporter.Domain.Enums;
using ExcelDataImporter.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ExcelDataImporter.Tests.Application.Services;

public class ImportServiceTests
{
    private readonly Mock<IImportRepository> _repositoryMock = new ();
    private readonly Mock<ILogger<ImportService>> _loggerMock = new ();
    private readonly ImportService _sut; // "sut" stands for "System Under Test"

    public ImportServiceTests()
    {
        _sut = new ImportService(_repositoryMock.Object, _loggerMock.Object);

        // Simulates EF Core assigning an Id on save, similar to what happens
        // when SaveChangesAsync runs against a real database.
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<ImportOperation>()))
            .ReturnsAsync((ImportOperation op) =>
            {
                op.Id = 1; // Simulate EF Core assigning an Id
                return op;
            });
    }

    [Fact]
    public async Task ImportAsync_WithAllValidRows_ReturnsCompletedStatus()
    {
        using var stream = ExcelTestDataBuilder.CreateWorkbookStream(
            ("Ana Souza", "ana@example.com", "11 91234-5678", "Nota"),
            ("Bruno Lima", "bruno@example.com", "", "")
        );

        var result = await _sut.ImportAsync(stream, "contacts.xlsx");

        result.Status.Should().Be(ImportStatus.Completed.ToString());
        result.TotalRows.Should().Be(2);
        result.SuccessRows.Should().Be(2);
        result.ErrorRows.Should().Be(0);
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ImportAsync_WithSomeInvalidRows_ReturnsCompletedWithErrorsStatus()
    {
        using var stream = ExcelTestDataBuilder.CreateWorkbookStream(
            ("Ana Souza", "ana@example.com", "", ""),
            ("", "sem-nome@example.com", "", ""),
            ("Carlos Dias", "email-invalido", "", "")
        );

        var result = await _sut.ImportAsync(stream, "contacts.xlsx");

        result.Status.Should().Be(ImportStatus.CompletedWithErrors.ToString());
        result.TotalRows.Should().Be(3);
        result.SuccessRows.Should().Be(1);
        result.ErrorRows.Should().Be(2);
        result.Errors.Should().HaveCount(2);
    }

    [Fact]
    public async Task ImportAsync_WithAllInvalidRows_ReturnsFailedStatus()
    {
        using var stream = ExcelTestDataBuilder.CreateWorkbookStream(
            ("", "sem-nome@example.com", "", ""),
            ("Carlos Dias", "email-invalido", "", "")
        );

        var result = await _sut.ImportAsync(stream, "contacts.xlsx");

        result.Status.Should().Be(ImportStatus.Failed.ToString());
        result.SuccessRows.Should().Be(0);
        result.ErrorRows.Should().Be(2);
    }

    [Fact]
    public async Task ImportAsync_CallsRepositoryAddAsyncExactlyOnce()
    {
        var stream = ExcelTestDataBuilder.CreateWorkbookStream(
            ("Ana Souza", "ana@example.com", "", "")
        );

        await _sut.ImportAsync(stream, "contacts.xlsx");

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<ImportOperation>()), Times.Once);
    }

    [Fact]
    public async Task GetRowsByOperationAsync_WhenOperationNotFound_ReturnsEmpty()
    {
        _repositoryMock
            .Setup(r => r.GetByIdWithRowsAsync(It.IsAny<int>()))
            .ReturnsAsync((ImportOperation?) null);

        var result = await _sut.GetRowsByOperationAsync(999);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetErrorsByOperationAsync_ReturnsOnlyRowsWithErrors()
    {
        var operation = new ImportOperation
        {
            Id = 1,
            FileName = "contacts.xlsx",
            Rows = [
                new ImportRow { RowNumber = 2, Name = "Ana", Email = "ana@example.com", HasError = false },
                new ImportRow { RowNumber = 3, Name = "", Email = "b@example.com", HasError = true, ErrorMessage = "Name is required." }
            ]
        };

        _repositoryMock
            .Setup(r => r.GetByIdWithRowsAsync(1))
            .ReturnsAsync(operation);

        var result = await _sut.GetErrorsByOperationAsync(1);

        result.Should().ContainSingle().Which.ErrorMessage.Should().Be("Name is required.");
    }
}
