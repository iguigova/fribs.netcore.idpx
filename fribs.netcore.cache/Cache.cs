using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack;
using ServiceStack.Caching;
using ServiceStack.Redis;

namespace fribs.netcore.cache
{
	public class Cache : ICache
	{
		private ICacheClient _cache  //https://stackoverflow.com/questions/674879/who-disposes-of-an-idisposable-public-property
		{
			get
			{
				return _cacheManager.GetCacheClient();
			}
		}

		private IRedisClientsManager _cacheManager;
		public Cache(string connStr)
		{
			_cacheManager = new RedisXManagerPool(connStr);

			//switch (cacheType)
			//{
			//	case CacheType.Redis: _cacheManager = new RedisManagerPool(connStr); break;
			//	case CacheType.XRedis: _cacheManager = new RedisXManagerPool(connStr); break;
			//}
		}

		private CacheValue<T> GetSafe<T>(string key, Action<Exception> onException)
		{
			if (!string.IsNullOrEmpty(key)) 
			{
				try
				{
					var cacheValue = _cache.Get<CacheValue<string>>(key);

					if (cacheValue != null)
					{
						return new CacheValue<T> { IsCached = true, Value = ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(cacheValue.Value) };
					}
				}
				catch (Exception ex)
				{
					onException?.Invoke(ex);
				}
			}

			return new CacheValue<T> { IsCached = false, Value = default(T) }; ;
		}

		private void SetSafe<T, U>(string cacheKey, U value, TimeSpan? ttl, Action<Exception> onException, params object[] keyParts)
		{
			try
			{
				if (ttl.HasValue)
				{
					IndexCacheKey<T>(cacheKey, ttl, onException, keyParts);

					_cache.Set(cacheKey, new CacheValue<string>() { IsCached = true, Value = ServiceStack.Text.JsonSerializer.SerializeToString(value) }, ttl);
				}
			}
			catch (Exception ex)
			{
				onException?.Invoke(ex);
			}
		}

		public string GetCacheKey<T>(params object[] keyParts)
		{
			return UrnId.CreateWithParts<T>(keyParts.Map<string>(s => (s ?? "_").ToString().ToLower()).ToArray());
		}

		private void IndexCacheKey<T>(string cacheKey, TimeSpan? ttl, Action<Exception> onException, params object[] keyParts)
		{
			// TODO: Only if type T requires it

			try
			{
				if (ttl.HasValue)
				{
					keyParts = keyParts.Where(s => s != null).ToArray();

					foreach (var keyPart in keyParts) 
					{
						var cacheKeyPartKey = GetCacheKeyPartKey<T>(keyPart);

						_cache.Set(cacheKeyPartKey, new CacheValue<string>() { IsCached = true, Value = $"{_cache.Get<CacheValue<string>>(cacheKeyPartKey)?.Value ?? string.Empty},{cacheKey}" }, ttl);
					}
				}
			}
			catch (Exception ex)
			{
				onException?.Invoke(ex);
			}
		}

		private string GetCacheKeyPartKey<T>(object keyPart)
		{
			return UrnId.CreateWithParts<T>("index", keyPart.ToString().ToLower());
		}

		private IEnumerable<string> PopCacheKeys<T>(Action<Exception> onException, params object[] keyParts)
		{
			var cacheKeys = new List<string>();

			keyParts = keyParts.Where(s => s != null).ToArray();

			foreach(var keyPart in keyParts)
			{
				var cacheKeyPartKey = GetCacheKeyPartKey<T>(keyPart);

				var cacheKey = GetSafe<string>(cacheKeyPartKey, onException);

				_cache.Remove(cacheKeyPartKey);

				if (cacheKey.IsCached)
				{
					cacheKeys.AddRange(cacheKey.Value.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToList());
				}
			}

			return cacheKeys;
		}

		public U PopCached<T, U>(Action<Exception> onException, params object[] keyParts)
		{
			return PopCached<T, U>((u) => { return true; }, onException, keyParts);
		}

		public U PopCached<T, U>(Func<U, bool> cond, Action<Exception> onException, params object[] keyParts)
		{
			var cached = GetSafe<U>(GetCacheKey<T>(keyParts), onException).Value;

			InvalidateCachedKeys<T>(() => { return cond(cached); }, onException, PopCacheKeys<T>(onException, keyParts));

			return cached;
		}

		public U GetCached<T, U>(Func<U> func, TimeSpan? ttl, Action<Exception> onException, params object[] keyParts)
		{
			var cacheKey = GetCacheKey<T>(keyParts);

			var cacheValue = GetSafe<U>(cacheKey, onException);

			if (!cacheValue.IsCached)
			{
				cacheValue.Value = func();   // NOTE: NO LONGER USES EF objects - EF uses circular references. Memcached handles the case on its own. There is no recommended solution for this situation. We use Automapper to strip the circular reference

				SetSafe<T, U>(cacheKey, cacheValue.Value, ttl, onException, keyParts);
			}

			return cacheValue.Value;
		}

		public U GetCached<T, U>(Func<Tuple<object[], U>> func, TimeSpan? ttl, Action<Exception> onException, params object[] keyParts)
		{
			var cacheKey = _cache.GetKeysStartingWith(GetCacheKey<T>(keyParts)).FirstNonDefault() ?? GetCacheKey<T>(keyParts);

			var cacheValue = GetSafe<U>(cacheKey, onException);

			if (!cacheValue.IsCached)
			{
				var tuple = func();

				cacheValue.Value = tuple.Item2; // NOTE: NO LONGER USES EF objects - EF uses circular references. Memcached handles the case on its own. There is no recommended solution for this situation. We use Automapper to strip the circular reference

				SetSafe<T, U>(GetCacheKey<T>(tuple.Item1), cacheValue.Value, ttl, onException, tuple.Item1);
			}

			return cacheValue.Value;
		}

		public void SetCached<T>(object value, TimeSpan? ttl, bool resetTTL, Action<Exception> onException, params object[] keyParts)
		{
			if (value != null)
			{
				var cacheKey = GetCacheKey<T>(keyParts);
				
				SetSafe<T, object>(cacheKey, value, resetTTL && _cache.GetTimeToLive(cacheKey) < ttl ? ttl?.Add(TimeSpan.FromMinutes(10)) : ttl, onException, keyParts); //Expire in ttl + 10 (add 10 so it does not have to add expiry time every single call)
			}
		}

		public void InvalidateCached<T>(Action<Exception> onException, params object[] keyParts)
		{
			InvalidateCachedKeys<T>(() => true, onException, PopCacheKeys<T>(onException, keyParts));
		}

		public void InvalidateCached<T>(Func<bool> cond, Action<Exception> onException, params object[] keyParts)
		{
			InvalidateCachedKeys<T>(cond, onException, PopCacheKeys<T>(onException, keyParts));
		}

		private void InvalidateCachedKeys<T>(Func<bool> cond, Action<Exception> onException, IEnumerable<string> keys)
		{
			if (cond())
			{
				try
				{
					if (!keys.Any())
					{
						_cache.RemoveByPattern($"{GetCacheKey<T>()}*"); // Invalidate the entire namespace
					}

					_cache.RemoveAll(keys);
				}
				catch (Exception ex)
				{
					onException?.Invoke(ex);
				}
			}
		}

		public int InvalidateAll(Action<Exception> onException, string pattern)
		{
			var keys = _cache.GetKeysByPattern(pattern).ToList();

			try
			{
				_cache.RemoveAll(keys);
			}
			catch (Exception ex)
			{
				onException?.Invoke(ex);
			}

			return keys.Count();
		}

		public void InvalidateAll<T, U>(Func<U, bool> cond, Action<Exception> onException, params object[] keyParts)
		{
			var cacheKeys = PopCacheKeys<T>(onException, keyParts);

			foreach (var cacheKey in cacheKeys)
			{
				var cached = GetSafe<U>(cacheKey, onException);

				if (cached.IsCached)
				{
					InvalidateCachedKeys<T>(() => { return cond(cached.Value); }, onException, new List<string>() { cacheKey });
				}
			}
		}
	}
}
