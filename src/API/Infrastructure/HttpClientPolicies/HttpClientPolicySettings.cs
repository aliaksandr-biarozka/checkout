namespace API.Infrastructure.HttpClientPolicies
{
    public record HttpClientPolicySettings
    {
        public int RetryCount { get; init; }

        public int RetryCountBeforeBreaking { get; init; }

        public int DurationOfBreakInSeconds { get; init; }

        public int RequestTimeoutInSeconds { get; init; }
    }
}
