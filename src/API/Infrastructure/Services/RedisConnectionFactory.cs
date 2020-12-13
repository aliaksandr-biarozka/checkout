using System;
using StackExchange.Redis;

namespace API.Infrastructure.Services
{
    public class RedisConnectionFactory
    {
        private readonly Lazy<ConnectionMultiplexer> _connectionMultiplexer;

        public RedisConnectionFactory(ConfigurationOptions configurationOptions)
        {
            if (configurationOptions == null)
                throw new ArgumentNullException($"{nameof(configurationOptions)} are not provided");

            _connectionMultiplexer = new Lazy<ConnectionMultiplexer>(() => {
                var connection = ConnectionMultiplexer.Connect(configurationOptions);

                return connection;
            });
        }

        public ConnectionMultiplexer Connection => _connectionMultiplexer.Value;
    }
}
