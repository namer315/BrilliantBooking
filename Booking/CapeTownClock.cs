namespace ERPBooking.Booking;

public static class CapeTownClock
{
    private static readonly TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById(
        OperatingSystem.IsWindows() ? "South Africa Standard Time" : "Africa/Johannesburg");

    public static DateTime Now =>
        TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, TimeZone).DateTime;

    public static DateOnly Today => DateOnly.FromDateTime(Now);

    public static TimeOnly CurrentTime => TimeOnly.FromDateTime(Now);
}
