namespace PRN232.LMS.Student.API.Helpers;

public static class FieldSelectionHelper
{
    public static List<string> ParseFields(string? fields)
    {
        if (string.IsNullOrWhiteSpace(fields))
            return new List<string>();

        return fields.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
    }

    public static List<string> GetInvalidFields<T>(
        List<string> selectedFields,
        IReadOnlyDictionary<string, Func<T, object?>> fieldSelectors)
    {
        if (selectedFields.Count == 0)
            return new List<string>();

        return selectedFields
            .Where(field => !fieldSelectors.ContainsKey(field))
            .ToList();
    }

    public static List<object> Apply<T>(
        List<T> items,
        List<string> selectedFields,
        IReadOnlyDictionary<string, Func<T, object?>> fieldSelectors)
    {
        if (selectedFields.Count == 0)
            return items.Cast<object>().ToList();

        return items.Select(item =>
        {
            var dict = new Dictionary<string, object?>();
            foreach (var field in selectedFields)
            {
                if (fieldSelectors.TryGetValue(field, out var selector))
                {
                    dict[field] = selector(item);
                }
            }
            return (object)dict;
        }).ToList();
    }
}
