public static class UrlEditor
{
    //  Editing the URL
    //  Adding https:// to the URL if it's not present to avoid redirect errors
    public static string ModifyUrl(string longUrl){
    if (longUrl.StartsWith("http://") || longUrl.StartsWith("https://")) {
        return longUrl;
    }
    return "https://" + longUrl;
}
}