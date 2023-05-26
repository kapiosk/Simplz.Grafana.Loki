using System.Text.Json;

namespace Simplz.Grafana.Loki;

public static class MiscExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, };

    public static string Decode(this string text)
    {
        return Uri.UnescapeDataString(text);
    }

    public static string Encode(this string text)
    {
        return Uri.EscapeDataString(text);
    }

    public static DateTime GetFromEpoch(this string epoch)
    {
        if (long.TryParse(epoch, out var number))
            return DateTimeOffset.FromUnixTimeMilliseconds(number / 1000000).UtcDateTime;
        return DateTime.MinValue;
    }

    public static string GetEpoch(this DateTime dateTime)
    {
        var epoch = new DateTimeOffset(dateTime).ToUnixTimeMilliseconds() * 1000000;
        return epoch.ToString();
    }

    public static IEnumerable<Dictionary<string, string>> ConvertToTimestampedDictionary(this Models.LokiResponse<Models.Result>? response)
    {
        if (response is not null && response.status.Equals("success"))
        {
            var list = from stream in response.data.result
                       let timestamp = stream.values.First().First().GetFromEpoch()
                       orderby timestamp descending
                       select new { stream.stream, timestamp };
            foreach (var log in list)
            {
                var data = log.stream;
                if (!data.TryAdd("LogTimestamp", log.timestamp.ToString()))
                    data.TryAdd("LokiLogTimestamp", log.timestamp.ToString());
                yield return data;
            }
        }
    }
}
