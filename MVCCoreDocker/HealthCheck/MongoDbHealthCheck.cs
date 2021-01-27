using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCoreDocker.HealthCheck
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using MongoDB.Driver;

    namespace HealthChecks.DemoApplication.HealthChecks
    {
        public class MongoDbHealthCheck
            : IHealthCheck
        {
            private static readonly ConcurrentDictionary<MongoClientSettings, MongoClient> _mongoClient = new ConcurrentDictionary<MongoClientSettings, MongoClient>();
            private readonly MongoClientSettings _mongoClientSettings;
            private readonly string _specifiedDatabase;

            public MongoDbHealthCheck(string connectionString)
            {
                var mongoDbUrl = MongoUrl.Create(connectionString);

                _specifiedDatabase = mongoDbUrl.DatabaseName;
                _mongoClientSettings = MongoClientSettings.FromUrl(mongoDbUrl);
            }

            public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            {
                try
                {
                    var mongoClient = _mongoClient.GetOrAdd(_mongoClientSettings, settings => new MongoClient(settings));

                    if (!string.IsNullOrEmpty(_specifiedDatabase))
                    {
                        using var cursor = await mongoClient
                            .GetDatabase(_specifiedDatabase)
                            .ListCollectionNamesAsync(cancellationToken: cancellationToken);

                        await cursor.FirstAsync(cancellationToken);
                    }
                    else
                    {
                        using var cursor = await mongoClient.ListDatabaseNamesAsync(cancellationToken);
                        await cursor.FirstOrDefaultAsync(cancellationToken);
                    }

                    return HealthCheckResult.Healthy();
                }
                catch (Exception ex)
                {
                    return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
                }
            }
        }
    }
}
