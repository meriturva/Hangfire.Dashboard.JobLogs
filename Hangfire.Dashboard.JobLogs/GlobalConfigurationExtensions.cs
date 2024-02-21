using System;
using System.Linq;
using System.Runtime.Caching;

namespace Hangfire.Dashboard.JobLogs
{
    /// <summary>
    /// Provides extension methods to setup Hangfire.JobLogs
    /// </summary>
    public static class GlobalConfigurationExtensions
    {
        static readonly ObjectCache renderedJobs = MemoryCache.Default;
        /// <summary>
        /// Configures Hangfire to use JobLogs.
        /// </summary>
        /// <param name="configuration">Global configuration</param>
        public static IGlobalConfiguration UseDashboardJobLogs(this IGlobalConfiguration configuration)
        {
            configuration.UseJobDetailsRenderer(100, dto =>
            {
                // avoid multiple renders for the same JobId
                if (renderedJobs.Get(dto.JobId) != null)
                {
                    return new NonEscapedString(string.Empty);
                }

                var jobStorageConnection = JobStorage.Current.GetConnection();
                var logString = "No Logs Found";
                var logsMessages = jobStorageConnection.GetAllEntriesFromHash($"joblogs-jobId:{dto.JobId}");

                if (logsMessages != null)
                {
                    logString = string.Join("<br>", logsMessages.Where(kvp => kvp.Value != null).Select(kvp => kvp.Value));
                }

                // cache rendered JobId
                var policy = new CacheItemPolicy
                {
                    AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddSeconds(1))
                };
                renderedJobs.Set(dto.JobId, true, policy);

                return new NonEscapedString($"<h3>Log messages</h3>" +
                    $"<div class=\"state-card \"><div class=\"state-card-body\">{logString}</div></div>" +
                    $"");
            });

            return configuration;
        }
    }
}
