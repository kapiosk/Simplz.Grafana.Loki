# Simplz.Grafana.Loki

## Add package to project

```shell
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

```cs
builder.Services.AddGranafaLokiService(builder.Configuration.GetRequiredSection("GranafaLokiOptions"));
```

## Sample log retrieval

```cs
var rawLogs = await GranafaLokiService.GetLogsAsync("appName", DateTimeOffset.UtcNow.AddHours(-5));
var timestampedLogs = rawLogs.ConvertToLogItems();
```

## Sample log retrieval with querybuilder

Please note that sequence of operations, excluding start/end, matters

```cs
GrafanaLokiQueryBuilder queryBuilder = new();

//Filter my label app having value "appName", mandatory
queryBuilder.SelectApp("appName");

//Set from time of logs to retrieve, mandatory
queryBuilder.AddStartTime(DateTimeOffset.UtcNow.AddHours(-5));

//Convert log to Json object
queryBuilder.ExpandAsJson();

//Filter by field level with value "error"
queryBuilder.FilterJsonByValue("level", "error");

//Filter by field source, containing string "Microsoft" essentially calls:
//queryBuilder.FilterJsonByRegex("source", "Microsoft.*");
queryBuilder.FilterJsonByValue("source", "Microsoft", exact: false);

var timestampedLogs = await GranafaLokiService.GetLogItemsAsync(queryBuilder);
```
