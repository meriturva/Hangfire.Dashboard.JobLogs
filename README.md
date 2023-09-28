# Hangfire.Dashboard.JobLogs

[![Latest version](https://img.shields.io/nuget/v/Bonura.Hangfire.Dashboard.JobLogs.svg)](https://www.nuget.org/packages?q=Bonura.Hangfire.Dashboard.JobLogs)

Show job logs on the dashboard.

![image](https://github.com/meriturva/Hangfire.Dashboard.JobLogs/assets/5664195/a364a19c-cbff-44d4-98b3-251d7aea6c28)

The project takes advantage of the integration with NLog thanks to [NLog.HangfireJobLogsTarget](https://github.com/meriturva/NLog.HangfireJobLogsTarget).

Note: the project is really simple but it is a good starting point (in my opinion).

Installation
-------------

Hangfire.Dashboard.JobLogs is available as a NuGet package. You can install it using the NuGet Package Console window:

```
PM> Install-Package Bonura.Hangfire.Dashboard.JobLogs
```

Configuration
-------------
NLog configuration with `HangfireJobLogs` target and layout with `${hangfire-decorator}` (mandatory).
```json
"NLog": {
  "extensions": [
    {
      "assembly": "NLog.HangfireJobLogsTarget"
    }
  ],
  "default-wrapper": {
    "type": "AsyncWrapper",
    "overflowAction": "Block"
  },
  "targets": {
    "hangfire_dashboard": {
      "layout": "${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=toString}${hangfire-decorator}",
      "type": "HangfireJobLogs"
    }
  },
  "rules": {
    "hangfire": {
      "logger": "Sample.Jobs.*",
      "minLevel": "Info",
      "writeTo": "hangfire_dashboard"
    }
  }
}
```

Services configuration:
```csharp
// Add hangfire context services
builder.Services.AddHangfirePerformContextAccessor();

// Add Hangfire services.
builder.Services.AddHangfire((serviceProvider, config) =>
{
    // Add filter to handle PerformContextAccessor
    config.UsePerformContextAccessorFilter(serviceProvider);
    // Add a storage
    config.UseInMemoryStorage();
    // Add jobLogs
    config.UseJobLogs();
});

// Add the processing server as IHostedService
builder.Services.AddHangfireServer();
```

Job using `ILogger` (Microsoft.Extensions.Logging):
```csharp
public class SimpleJob
{
    private ILogger<SimpleJob> _logger;

    public SimpleJob(ILogger<SimpleJob> logger)
    {
        _logger = logger;
    }

    public async Task DoJobAsync()
    {
        _logger.LogInformation("Log message");
        await Task.CompletedTask;
    }
}
```

How it works
-------------
Basically, a log message produced by a hangfire job is captured by NLog `NLog.HangfireJobLogsTarget`. Thanks to `${hangfire-decorator}` and `PerformContextAccessor` log messages are sent to Hangfire storage.
Finally, log messages are shown on the detail page thanks to a simple `JobDetailsRenderer`.
