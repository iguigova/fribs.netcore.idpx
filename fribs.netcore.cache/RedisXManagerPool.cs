using System;
using System.Linq;
using System.Collections.Generic;
using ServiceStack.Caching;
using ServiceStack.Redis;
using StackExchange.Redis;

namespace fribs.netcore.cache
{
	public class RedisXManagerPool : IRedisClientsManager
	{
		private ConnectionMultiplexer _connMultiplexer;
		private readonly ConfigurationOptions _connOptions;

		public RedisXManagerPool(string redisConnStr)
		{
			_connOptions = ConfigurationOptions.Parse(redisConnStr);
			_connMultiplexer = ConnectionMultiplexer.Connect(redisConnStr);
		}

		public void Dispose() { _connMultiplexer.Dispose(); }

		public ICacheClient GetCacheClient()
		{
			return new RedisXClient(_connMultiplexer, _connOptions);
		}

		public IRedisClient GetClient() { throw new NotImplementedException(); }

		public ICacheClient GetReadOnlyCacheClient() { throw new NotImplementedException(); }

		public IRedisClient GetReadOnlyClient() { throw new NotImplementedException(); }
	}

    public class RedisXClient : ICacheClient, IRemoveByPattern, ICacheClientExtended
    {
        private ConnectionMultiplexer _connMultiplexer;
        private ConfigurationOptions _connOptions;

        public RedisXClient(ConnectionMultiplexer connMultiplexer, ConfigurationOptions connOptions)
        {
            _connOptions = connOptions;
            _connMultiplexer = connMultiplexer;
        }

        // https://stackexchange.github.io/StackExchange.Redis/Basics
        public void Dispose() { }

        public bool Add<T>(string key, T value, DateTime expiresAt) { throw new NotImplementedException(); }

        public bool Add<T>(string key, T value) { throw new NotImplementedException(); }

        public bool Add<T>(string key, T value, TimeSpan expiresIn) { throw new NotImplementedException(); }

        public long Decrement(string key, uint amount) { throw new NotImplementedException(); }

        public void FlushAll() { throw new NotImplementedException(); }

        public T Get<T>(string key)
        {
            var db = _connMultiplexer.GetDatabase();

            var value = db.StringGet(key.ToLower());

            if (value.IsNull)
            {
                return default(T);
            }

            return ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(value);
        }

        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys) { throw new NotImplementedException(); }

        public long Increment(string key, uint amount) { throw new NotImplementedException(); }

        public bool Remove(string key)
        {
            var db = _connMultiplexer.GetDatabase();

            return db.KeyDelete(key.ToLower());
        }

        public void RemoveAll(IEnumerable<string> keys)
        {
            if (keys != null && keys.Count() > 0)
            {
                var db = _connMultiplexer.GetDatabase();

                db.KeyDelete(keys.Select(x => (RedisKey)x.ToLower()).ToArray());
            }
        }

        public bool Replace<T>(string key, T value, DateTime expiresAt) { throw new NotImplementedException(); }

        public bool Replace<T>(string key, T value, TimeSpan expiresIn) { throw new NotImplementedException(); }

        public bool Replace<T>(string key, T value) { throw new NotImplementedException(); }

        public bool Set<T>(string key, T value, DateTime expiresAt) { throw new NotImplementedException(); }

        public bool Set<T>(string key, T value)
        {
            var db = _connMultiplexer.GetDatabase();

            return db.StringSet(key.ToLower(), ServiceStack.Text.JsonSerializer.SerializeToString(value));
        }

        public bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            // TODO: NOTE that we are ignoring the return value here - false should probably throw an exception to be caught in the calling method as per ServiceStack Redis

            var db = _connMultiplexer.GetDatabase();

            return db.StringSet(key.ToLower(), ServiceStack.Text.JsonSerializer.SerializeToString(value)) && db.KeyExpire(key.ToLower(), expiresIn);
        }

        public void SetAll<T>(IDictionary<string, T> values) { throw new NotImplementedException(); }

        public void RemoveByPattern(string pattern)
        {
            var db = _connMultiplexer.GetDatabase();

            var keys = GetKeysByPattern(pattern);

            if (keys != null && keys.Count() > 0)
            {
                db.KeyDelete(keys.Select(x => (RedisKey)x.ToLower()).ToArray());
            }
        }

        public void RemoveByRegex(string regex) { throw new NotImplementedException(); }

        public IEnumerable<string> GetKeysByPattern(string pattern)
        {
            var db = _connMultiplexer.GetDatabase();

            // https://stackoverflow.com/questions/32775456/redis-wildcard-delete
            //TODO: !!! THIS IS NOT SAFE !!! 
            var server = _connMultiplexer.GetServer(_connMultiplexer.GetEndPoints().First());
            return server.Keys(pattern: pattern.ToLower(), database: _connOptions.DefaultDatabase ?? 0, pageSize: 5000).Select(s => s.ToString());
        }

        public TimeSpan? GetTimeToLive(string key)
        {
            var db = _connMultiplexer.GetDatabase();

            return db.KeyTimeToLive(key.ToLower());
        }

        public void RemoveExpiredEntries() 
        {
            // TODO: Introduced with ICacheClientExtended update...
            // we have Remove, RemoveAll, RemoveByPattern, and RemoveByRegex so far
        }
    }
}