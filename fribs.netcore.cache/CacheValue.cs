namespace fribs.netcore.cache
{
	public class CacheValue<T>
	{
		public bool IsCached { get; set; }

		public T Value { get; set; }
	}
}
