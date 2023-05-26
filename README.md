# Simplz.Grafana.Loki

## Add package to project

```cli
dotnet add package Simplz.Grafana.Loki
```

## Add configuration

```json
{
    "GranafaLokiOptions":
    {
        "BaseAddress": "BaseAddress",
        "Username": "Username",
        "APIKey": "APIKey"
    }
}
```

## Inject dependancy

```csharp
builder.Services.AddGranafaLokiService(builder.Configuration.GetRequiredSection("GranafaLokiOptions"));
```

## Sample log retrieval

```csharp
var rawLogs = await GranafaLokiService.GetLogsAsync("appName", DateTimeOffset.UtcNow.AddHours(-5));
var timestampedLogs = response.ConvertToTimestampedDictionary();
```

## Sample log retrieval with querybuilder

Please note that sequence of operations, excluding start/end, matters

```csharp
GrafanaLokiQueryBuilder queryBuilder = new();
queryBuilder.SelectApp("app");
queryBuilder.AddStartTime(DateTimeOffset.UtcNow.AddHours(-5));
queryBuilder.ExpandAsJson();
queryBuilder.FilterJsonByValue("level", "error");
queryBuilder.FilterJsonByValue("source", "Microsoft", exact: false);
var rawLogs = await GetLogsAsync<LokiResponse<Result>>(queryBuilder);
var timestampedLogs = response.ConvertToTimestampedDictionary();
```
