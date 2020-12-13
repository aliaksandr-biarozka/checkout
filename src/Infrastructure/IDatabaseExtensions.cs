using System;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace Infrastructure
{
    internal static class IDatabaseExtensions
    {
        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="expiry">The expiry to set.</param>
        /// <param name="when">Which condition to set the value under (detaults to always).</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>True if the string was set, false otherwise.</returns>
        /// <remarks>https://redis.io/commands/set</remarks>
        internal static Task<bool> StringSetAsync<T>(this IDatabase database, RedisKey key, T value, TimeSpan? expiry = null, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            return database.StringSetAsync(key, JsonConvert.SerializeObject(value), expiry, when, flags);
        }

        /// <summary>
        /// Add the specified member to the set stored at key.
        /// Specified members that are already a member of this set are ignored.
        /// If key does not exist, a new set is created before adding the specified members.
        /// </summary>
        /// <param name="key">The key of the set.</param>
        /// <param name="value">The value to add to the set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>True if the specified member was not already present in the set, else False</returns>
        /// <remarks>https://redis.io/commands/sadd</remarks>
        internal static Task<bool> SetAddAsync<T>(this IDatabase database, RedisKey key, T value, CommandFlags flags = CommandFlags.None)
        {
            return database.SetAddAsync(key, JsonConvert.SerializeObject(value), flags);
        }

        /// <summary>
        /// Get the value of key. If the key does not exist the special value nil is returned. An error is returned if the value stored at key is not a string, because GET only handles string values.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value of key, or nil when key does not exist.</returns>
        /// <remarks>https://redis.io/commands/get</remarks>
        internal static Task<T> StringGetAsync<T>(this IDatabase database, RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return database.StringGetAsync(key, flags).ContinueWith(o => o.Result.IsNull ? default : JsonConvert.DeserializeObject<T>(o.Result, new JsonSerializerSettings { ContractResolver = new ResolvePrivateSetters { NamingStrategy = new SnakeCaseNamingStrategy() }}));
        }

        class ResolvePrivateSetters : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var prop = base.CreateProperty(member, memberSerialization);
                
                if (!prop.Writable)
                {
                    var property = member as PropertyInfo;
                    if (property != null)
                    {
                        var hasPrivateSetter = property.GetSetMethod(true) != null;
                        prop.Writable = hasPrivateSetter;
                    }
                }
                else
                {
                    var field = member as FieldInfo;
                    if (field != null)
                    {
                        prop.Writable = true;
                    }
                }

                if (!prop.Readable)
                {
                    var field = member as FieldInfo;
                    if (field != null)
                    {
                        prop.Readable = true;
                    }
                }

                return prop;
            }
        }
    }
}