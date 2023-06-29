namespace Simplz.Grafana.Loki;

public static class MiscExtensions
{
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

    public static IEnumerable<Models.LogItem> ConvertToLogItems(this Models.LokiResponse<Models.Result>? response)
    {
        if (response is not null && response.status.Equals("success"))
            return response.data.result.Select(s => new Models.LogItem(s.stream, s.values.First().First().GetFromEpoch())).OrderByDescending(s => s.Timestamp);
        return Enumerable.Empty<Models.LogItem>();
    }

#if NET
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
#endif
}
