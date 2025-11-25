public static class DateTimeExtensions
{
    public const string isoFormat = "yyyy-MM-dd'T'HH:mm.ss'Z'";
    public static string ToISOString(this System.DateTime dateTime)
    {
        return dateTime.ToString(isoFormat);
    }

    public static System.DateTime FromISOString(string isoString)
    {
        return System.DateTime.ParseExact(isoString, isoFormat, null);
    }
}
