using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace com.idpx
{
	public class ApiClient : IApiClient
	{
		//singleton as recommended because its easier on socket resources.
		protected static HttpClient _httpClientSingleton { get { return new HttpClient(); } }

		public async Task<string> PostAsync(string url, NameValueCollection headers = null, HttpContent content = null)
		{
			return await SendAsync(url, headers, new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri(url),
				Content = content
			});
		}

		public async Task<string> GetAsync(string url, NameValueCollection headers = null)
		{
			return await SendAsync(url, headers, new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri(url)
			});
		}

		private async Task<string> SendAsync(string url, NameValueCollection headers, HttpRequestMessage request)
		{
			request.Headers.Clear();

			if (headers != null)
			{
				foreach (var header in headers.AllKeys)
				{
					request.Headers.Add(header, headers[header]);
				}
			}

			if (request.Headers.UserAgent.Count == 0) //set a default
			{
				request.Headers.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("IDPX", "1.1+")); 
			}

			if (request.Headers.Accept.Count == 0) //set a default
			{
				request.Headers.Add(HttpRequestHeader.Accept.ToString(), "application/json");
			}

			//try
			//{				
					var response = await _httpClientSingleton.SendAsync(request); 

					if (!response.IsSuccessStatusCode)
					{
						throw new Exception(await response.Content.ReadAsStringAsync()); //.ToObjectDictionary()["Result"].ToString()); 
					}

					return await response.Content.ReadAsStringAsync();
			//}
			//catch (Exception ex)
			//{

			//}
		}

		private static Regex _passwordPattern = new Regex(@"(password)=([^&]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private string RemovePasswordInformation(string text)
		{
			return _passwordPattern.Replace(text, "$1=*****");
		}
	}
}
