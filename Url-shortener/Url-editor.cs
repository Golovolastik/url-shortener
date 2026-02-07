public static class UrlEditor
{
    //  Editing the URL
    //  Adding https:// to the URL if it's not present to avoid redirect errors
    public static string ModifyUrl(string longUrl)
{
    if (longUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
        || longUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        return longUrl;
    if (longUrl.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        return longUrl;
    return "https://" + longUrl;
}
}