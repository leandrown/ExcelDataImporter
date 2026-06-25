namespace ExcelDataImporter.Application.Helpers;

public static class ValidateHelper
{
    public static (bool hasError, string? message) Validate(string name, string email)
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
