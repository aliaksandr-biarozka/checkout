using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace API.Infrastructure.Services
{
    public class ThreadPoolLogger : IDisposable
    {
        private readonly int _logFrequency;
        private bool _disposed;
        private readonly ILogger _logger;

        public ThreadPoolLogger(int logFrequencyInMilliseconds, ILogger<ThreadPoolLogger> logger)
        {
            if (logFrequencyInMilliseconds < 0)
                throw new ArgumentOutOfRangeException($"{nameof(logFrequencyInMilliseconds)} can not be less than zero");

            _logFrequency = logFrequencyInMilliseconds;
            _logger = logger;

            _ = StartLogging();
        }

        private async Task StartLogging()
        {
            while(!_disposed)
            {
                await Task.Delay(_logFrequency);

                var stats = GetThreadPoolStats();

                _logger.LogInformation(String.Format("[{0}] IOCP:(Busy={1},Min={2},Max={3}), WORKER:(Busy={4},Min={5},Max={6})",
                    DateTimeOffset.UtcNow.ToString("u"),
                    stats.BusyIoThreads, stats.MinIoThreads, stats.MaxIoThreads,
                    stats.BusyWorkerThreads, stats.MinWorkerThreads, stats.MaxWorkerThreads
                ));
            }
        }

        /// <summary>
        /// Returns the current thread pool usage statistics for the CURRENT AppDomain/Process
        /// </summary>
        private static ThreadPoolUsageStats GetThreadPoolStats()
        {
            //BusyThreads =  TP.GetMaxThreads() –TP.GetAVailable();
            //If BusyThreads >= TP.GetMinThreads(), then threadpool growth throttling is possible.

            int maxIoThreads, maxWorkerThreads;
            ThreadPool.GetMaxThreads(out maxWorkerThreads, out maxIoThreads);

            int freeIoThreads, freeWorkerThreads;
            ThreadPool.GetAvailableThreads(out freeWorkerThreads, out freeIoThreads);

            int minIoThreads, minWorkerThreads;
            ThreadPool.GetMinThreads(out minWorkerThreads, out minIoThreads);

            int busyIoThreads = maxIoThreads - freeIoThreads;
            int busyWorkerThreads = maxWorkerThreads - freeWorkerThreads;

            return new ThreadPoolUsageStats
            {
                BusyIoThreads = busyIoThreads,
                MinIoThreads = minIoThreads,
                MaxIoThreads = maxIoThreads,
                BusyWorkerThreads = busyWorkerThreads,
                MinWorkerThreads = minWorkerThreads,
                MaxWorkerThreads = maxWorkerThreads,
            };
        }

        void IDisposable.Dispose()
        {
            _disposed = true;
        }

        struct ThreadPoolUsageStats
        {
            public int BusyIoThreads { get; set; }

            public int MinIoThreads { get; set; }

            public int MaxIoThreads { get; set; }

            public int BusyWorkerThreads { get; set; }

            public int MinWorkerThreads { get; set; }

            public int MaxWorkerThreads { get; set; }
        }
    }
}
