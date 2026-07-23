using ExcelDataImporter.Domain.Validators;
using FluentAssertions;
using Xunit;

namespace ExcelDataImporter.Tests.Domain.Validators;

public class ImportRowValidatorTests
{
    [Fact]
    public void Validate_WithValidNameAndEmail_ReturnNoError()
    {
        var (hasError, errorMessage) = ImportRowValidator.Validate("Ana Souza", "ana@example.com");

        hasError.Should().BeFalse();
        errorMessage.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_WithMissingName_ReturnsNameRequiredError(string? name)
    {
        var (hasError, errorMessage) = ImportRowValidator.Validate(name!, "ana@example.com");
        
        hasError.Should().BeTrue();
        errorMessage.Should().Be("Name is required.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_WithMissingEmail_ReturnsEmailRequiredError(string? email)
    {
        var (hasError, errorMessage) = ImportRowValidator.Validate("Ana Souza", email!);

        hasError.Should().BeTrue();
        errorMessage.Should().Be("Email is required.");
    }

    [Theory]
    [InlineData("email-sem-arroba")]
    [InlineData("email.com")]
    [InlineData("outro-invalido")]
    public void Validate_WithEmailMissingAtSymbol_ReturnsInvalidEmailError(string email)
    {
        var (hasError, errorMessage) = ImportRowValidator.Validate("Ana Souza", email);

        hasError.Should().BeTrue();
        errorMessage.Should().Be($"'{email}' is not a valid email address.");
    }

    [Fact]
    public void Validate_ChecksNameBeforeEmail_WhenBothAreMissing()
    {
        // Name validation runs first, so an empty name should be reported
        // even when the email is also missing.
        var (hasError, errorMessage) = ImportRowValidator.Validate("", "");

        hasError.Should().BeTrue();
        errorMessage.Should().Be("Name is required.");
    }
}
