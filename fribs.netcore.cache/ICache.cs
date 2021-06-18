using System;

namespace fribs.netcore.cache
{
	public interface ICache
	{
		string GetCacheKey<T>(params object[] keyParts);

		U PopCached<T, U>(Action<Exception> onException, params object[] keyParts);

		U PopCached<T, U>(Func<U, bool> cond, Action<Exception> onException, params object[] keyParts);

		U GetCached<T, U>(Func<U> func, TimeSpan? ttl, Action<Exception> onException, params object[] keyParts);

		U GetCached<T, U>(Func<Tuple<object[], U>> func, TimeSpan? ttl, Action<Exception> onException, params object[] keyParts);

		void SetCached<T>(object value, TimeSpan? ttl, bool resetTTL, Action<Exception> onException, params object[] keyParts);

		void InvalidateCached<T>(Action<Exception> onException, params object[] keyParts);

		void InvalidateCached<T>(Func<bool> cond, Action<Exception> onException, params object[] keyParts);

		int InvalidateAll(Action<Exception> onException, string pattern);

		void InvalidateAll<T, U>(Func<U, bool> cond, Action<Exception> onException, params object[] keyParts);
	}
}
