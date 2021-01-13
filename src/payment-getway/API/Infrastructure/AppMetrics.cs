using System.Collections.Generic;

namespace API.Infrastructure
{
    public record ApplicationMetrics
    {
        public DataStore DataStore { get; init; }

        public HttpPolicy HttpPolicy { get; init; }

        public Tracking Tracking { get; init; }

        public int FlushIntervalInSeconds { get; init; }
    }

    public record DataStore
    {
        public string Address { get; init; }

        public string Database { get; init; }

        public string UserName { get; init; }

        public string Password { get; init; }

        public string Consistenency { get; init; }

        public string RetentionPolicy { get; init; }

        public bool CreateDataBaseIfNotExists { get; init; }
    }

    public record HttpPolicy
    {
        public int BackoffPeriodInSeconds { get; init; }

        public int FailuresBeforeBackoff { get; init; }

        public int TimeoutInSeconds { get; init; }
    }

    public record Tracking
    {
        public double ApdexTSeconds { get; init; }

        public IList<int> IgnoredHttpStatusCodes { get; init; }
    }
}
