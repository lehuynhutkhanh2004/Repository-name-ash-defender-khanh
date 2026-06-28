namespace AshDefender.Shared.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

        public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);

        public static string ToDisplayName(this string id) =>
            string.IsNullOrEmpty(id) ? string.Empty : System.Text.RegularExpressions.Regex.Replace(id, "([A-Z])", " $1").Trim();
    }
}
