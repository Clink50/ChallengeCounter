using NodaTime;

namespace ChallengeCounter.Api.Helpers;

public static class Helpers
{
    public static DateTimeZone GetZone(string? timezone)
    {
        if (string.IsNullOrWhiteSpace(timezone)) return DateTimeZone.Utc;
        try { return DateTimeZoneProviders.Tzdb[timezone]; }
        catch { return DateTimeZone.Utc; }
    }
}