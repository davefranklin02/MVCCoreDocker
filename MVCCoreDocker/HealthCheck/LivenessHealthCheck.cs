using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MVCCoreDocker.HealthCheck
{
    internal class LivenessHealthCheck : IHealthCheck
    {

        private ILogger _log;
        public LivenessHealthCheck(ILogger log)
        {
            _log = log;
        }

 
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Some Liveness check
            _log.LogInformation("LivenessHealthCheck executed.");
            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }
}
