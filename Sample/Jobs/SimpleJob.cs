using Hangfire;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Sample.Jobs
{
    public class SimpleJob
    {
        private ILogger<SimpleJob> _logger;

        public SimpleJob(ILogger<SimpleJob> logger)
        {
            _logger = logger;
        }

        [LatencyTimeoutAttribute(10)]
        public async Task DoJobAsync()
        {
            for (int i = 0; i < 10; i++)
            {
                _logger.LogInformation("Test log message1 - " + i);
                _logger.LogWarning("Test log message2 - " + i);
                await Task.Delay(1000);
            }
            await Task.CompletedTask;
        }
    }
}
