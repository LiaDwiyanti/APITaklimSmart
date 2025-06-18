namespace APITaklimSmart.Helpers
{
    public class TimeHelper
    {
        private static readonly TimeZoneInfo JakartaTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById(
                OperatingSystem.IsWindows() ? "SE Asia Standard Time" : "Asia/Jakarta"
            );

        public static DateTime NowJakarta()
        {
            var jakartaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, JakartaTimeZone);
            return DateTime.SpecifyKind(jakartaTime, DateTimeKind.Unspecified);
        }
    }
}
