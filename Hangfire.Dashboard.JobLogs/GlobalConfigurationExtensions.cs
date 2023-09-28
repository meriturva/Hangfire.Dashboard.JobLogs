using Hangfire.Dashboard;
using System;
using System.Linq;

namespace Hangfire.JobLogs
{
    /// <summary>
    /// Provides extension methods to setup Hangfire.JobLogs
    /// </summary>
    public static class GlobalConfigurationExtensions
    {
        /// <summary>
        /// Configures Hangfire to use JobLogs.
        /// </summary>
        /// <param name="configuration">Global configuration</param>
        public static IGlobalConfiguration UseDashboardJobLogs(this IGlobalConfiguration configuration)
        {
            configuration.UseJobDetailsRenderer(100, dto =>
            {
                var jobStorageConnection = JobStorage.Current.GetConnection();
                var logsMessages = jobStorageConnection.GetAllEntriesFromHash($"joblogs-jobId:{dto.JobId}");

                var logString = String.Join("<br>", logsMessages.OrderBy(kvp => kvp.Value).Select(kvp => kvp.Value));

                return new NonEscapedString($"<h3>Log messages</h3>" +
                    $"<div class=\"state-card \"><div class=\"state-card-body\">{logString}</div></div>" +
                    $"");
            });

            return configuration;
        }
    }
}
