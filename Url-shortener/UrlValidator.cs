using System.Text.RegularExpressions;

/// <summary>
/// Validates long URL so we only store safe, redirectable addresses and reject typos or injection (scheme optional, TLD and allowed chars enforced).
/// </summary>
public static class UrlValidator
{
    /// <summary>Restricts to RFC 3986–style and common path/query chars so stored URLs are safe to redirect to.</summary>
    private static readonly Regex AllowedChars = new Regex(@"^[a-zA-Z0-9\-._~:/?#\[\]@!$&'()*+,;=%{}]+$", RegexOptions.Compiled);

    /// <summary>Requires a dot-plus-letters segment so we don’t accept bare hostnames like "localhost" as full URLs.</summary>
    private static readonly Regex HasTld = new Regex(@"\.[a-zA-Z]+\b", RegexOptions.Compiled);

    /// <summary>Data URL format: data:[mediatype][;base64],&lt;data&gt; — comma required.</summary>
    private static readonly Regex DataUrlPattern = new Regex(@"^data:[a-zA-Z0-9+/\-.;]+,.+", RegexOptions.Compiled);

    /// <summary>
    /// Validates input so the form only submits when we can safely normalize and store the URL; <paramref name="error"/> explains why validation failed.
    /// </summary>
    public static bool IsValid(string? input, out string? error)
    {
        error = null;
        if (string.IsNullOrWhiteSpace(input))
        {
            error = "Введите адрес.";
            return false;
        }

        string trimmed = input.Trim(' ');
        if (trimmed.Length == 0)
        {
            error = "Введите адрес.";
            return false;
        }

        string toCheck = UrlEditor.ModifyUrl(trimmed);

        // Data URL: no TLD required, validate format only.
        if (toCheck.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            if (!DataUrlPattern.IsMatch(toCheck))
            {
                error = "Некорректный data URL: ожидается data:[тип][;base64],<данные>.";
                return false;
            }
            return true;
        }

        if (!AllowedChars.IsMatch(toCheck))
        {
            error = "Адрес содержит недопустимые символы. Допускаются: буквы, цифры, - . _ ~ : / ? # [ ] @ ! $ & ' ( ) * + , ; = % { }.";
            return false;
        }

        if (!HasTld.IsMatch(toCheck))
        {
            error = "Адрес должен содержать домен с зоной (например .com, .org) — точка и буквы.";
            return false;
        }

        return true;
    }
}
