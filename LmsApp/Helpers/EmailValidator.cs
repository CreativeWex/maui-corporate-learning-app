using System.Text.RegularExpressions;

namespace LmsApp.Helpers;

public static partial class EmailValidator
{
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();

    public static bool IsValid(string? email)
        => !string.IsNullOrWhiteSpace(email) && EmailRegex().IsMatch(email);
}
