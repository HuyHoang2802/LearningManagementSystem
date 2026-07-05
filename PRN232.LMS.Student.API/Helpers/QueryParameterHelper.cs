namespace PRN232.LMS.Student.API.Helpers;

public static class QueryParameterHelper
{
    public static List<string> ParseCommaSeparatedValues(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return new List<string>();

        return value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
    }
}
