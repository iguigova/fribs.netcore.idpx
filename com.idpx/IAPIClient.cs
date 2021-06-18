using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;

namespace com.idpx
{
	public interface IApiClient
	{
		Task<string> PostAsync(string url, NameValueCollection headers = null, HttpContent content = null);

		Task<string> GetAsync(string url, NameValueCollection headers = null);
	}
}
