using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MVCCoreDocker.HealthCheck
{
    internal class ReadinessHealthCheck : IHealthCheck
    {
        private ILogger _log;
        public ReadinessHealthCheck(ILogger log)
        {
            _log = log;
        }
        public bool StartupTaskCompleted { get; set; } = false;
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Some Readiness check
            _log.LogInformation("Readiness health check executed.");
            if (StartupTaskCompleted)
            {
                return Task.FromResult(
                    HealthCheckResult.Healthy("The startup task is finished."));
            }
            return Task.FromResult(
                HealthCheckResult.Unhealthy("The startup task is still running."));
        }
    }
}
