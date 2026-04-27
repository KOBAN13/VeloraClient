namespace Core.Utils.Extensions
{
    public static class StringExtensions
    {
        public static string FormatText(this string text)
        {
            return string.IsNullOrWhiteSpace(text)
                ? string.Empty
                : text.Trim()
                    .Replace("&", "&amp;")
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;");
        }
    }
}