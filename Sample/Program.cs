using Hangfire;
using Hangfire.JobLogs;
using Hangfire.PerformContextAccessor;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Web;
using Sample.Jobs;

var builder = WebApplication.CreateBuilder(args);

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
    config.UseDashboardJobLogs();
});

// Add the processing server as IHostedService
builder.Services.AddHangfireServer();

builder.Services.AddControllers();
builder.Logging.ClearProviders();
builder.Host.UseNLog();

var app = builder.Build();

app.UseHangfireDashboard();

app.MapControllers();

RecurringJob.AddOrUpdate<SimpleJob>("test", sj => sj.DoJobAsync(), "*/5 * * * * *");

app.Run();