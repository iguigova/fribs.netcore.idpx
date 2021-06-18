using Newtonsoft.Json;
using System;

namespace com.idpx.data
{
	public class States
    {
		public static TimeSpan TTL { get { return new TimeSpan(0, 5, 0); } }
    }

	public class State
	{
		public string Id { get; set; }

		public string ProviderId { get; set; }

		public string OAuth10aToken { get; set; }

		public string OAuth10aTokenSecret { get; set; }

		public string OAuth10aVerifier { get; set; }

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}
