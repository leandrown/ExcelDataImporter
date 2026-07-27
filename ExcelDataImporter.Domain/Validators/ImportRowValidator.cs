namespace ExcelDataImporter.Domain.Validators;

public static class ImportRowValidator
{
    public static (bool HasError, string? ErrorMessage) Validate(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            return (true, "Name is required.");
        if (string.IsNullOrWhiteSpace(email))
            return (true, "Email is required.");
        if (!email.Contains("@"))
            return (true, $"'{email}' is not a valid email address.");
        return (false, null);
    }
}
