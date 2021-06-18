using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using com.idpx.core.oauth.data;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace com.idpx.core.oauth.clients
{
	public abstract class OAuth10aClient : OAuthApiClient, IOAuthClient
	{
		// https://msdn.microsoft.com/en-us/library/azure/dn645542.aspx

		// https://www.cubrid.org/blog/dancing-with-oauth-understanding-how-authorization-works

		protected string _requestTokenUrl;
		protected string _loginUrl;
		protected string _tokenUrl;
		protected string _revokeTokenUrl;
		protected string _dataUrl;
		protected string _redirectUrl;
		protected string _clientId;
		protected string _clientSecret;

		protected string _prompt;
		protected string _promptKey;
		protected string _promptValue;

		protected Action<string, string> _onTokenUpdate;

		public OAuth10aClient(string requestTokenUrl, string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string redirectUrl, string stateId, string prompt, Action<string, string> onTokenUpdate) : base(stateId)
		{
			_requestTokenUrl = requestTokenUrl;
			_loginUrl = loginUrl;
			_tokenUrl = tokenUrl;
			_revokeTokenUrl = revokeTokenUrl;
			_dataUrl = dataUrl;
			_redirectUrl = redirectUrl + $"?state={System.Web.HttpUtility.UrlEncode(stateId)}";
			_clientId = clientId;
			_clientSecret = clientSecret; // HttpUtility.UrlEncode(clientSecret);

			_prompt = prompt;
			if (!string.IsNullOrEmpty(_prompt))
			{
				var p = prompt.Split('=');
				_promptKey = p[0];
				_promptValue = p[1];
			}

			_onTokenUpdate = onTokenUpdate;
		}

		protected async Task<(string requestToken, string requestTokenSecret)> RequestTokenAsync()
		{
			string requestToken;
			string requestTokenSecret;

			// https://developer.twitter.com/en/docs/basics/authentication/guides/creating-a-signature.html

			var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

			var signatureBaseString = string.Join('&', new List<string>()
			{
				"POST",
				Uri.EscapeDataString(_requestTokenUrl),
				Uri.EscapeDataString(string.Join('&', ZipDataTuples(EscapeDataTuples(new List<(string, string)>()
					{
						( "oauth_callback", _redirectUrl ),
						( "oauth_consumer_key", _clientId ),
						( "oauth_nonce", _stateId.ToString() ),
						( "oauth_signature_method", "HMAC-SHA1" ),
						( "oauth_timestamp", timestamp),
						( "oauth_version", "1.0" )
					}).OrderBy(s => s.Item1))))
			});

			var signingKey = $"{Uri.EscapeDataString(_clientSecret)}&";

			var signature = CreateHMACSHA1Token(signatureBaseString, signingKey);

			// https://developer.twitter.com/en/docs/basics/authentication/guides/authorizing-a-request

			var headerAuthorizationString = string.Join(", ", ZipDataTuples(EscapeDataTuples(new List<(string, string)>()
					{
						( "oauth_callback", _redirectUrl ),
						( "oauth_consumer_key", _clientId ),
						( "oauth_nonce", _stateId.ToString() ),
						( "oauth_signature", signature),
						( "oauth_signature_method", "HMAC-SHA1" ),
						( "oauth_timestamp", timestamp ),
						( "oauth_version", "1.0" )
					}).OrderBy(s => s.Item1), (s) => { return $"\"{s}\""; }));

			var requestTokenResponse = await PostAsync(_requestTokenUrl, new NameValueCollection
			{
				{ HttpRequestHeader.Authorization.ToString(), $"OAuth {headerAuthorizationString}"},
			});

			var requestTokens = DeserializeDataTuples(UnzipDataTuples(requestTokenResponse.Split('&')), new { oauth_token = (string)null, oauth_token_secret = (string)null, oauth_callback_confirmed = (bool?)null });

			if (requestTokens.oauth_callback_confirmed ?? true)
			{
				requestToken = requestTokens.oauth_token;
				requestTokenSecret = requestTokens.oauth_token_secret;
			}
			else
			{
				requestToken = null;
				requestTokenSecret = null;
			}

			return (requestToken, requestTokenSecret);
		}

		protected IEnumerable<(string, string)> ExcludeNullDataTuples(IEnumerable<(string, string)> tuples)
		{
			foreach (var tuple in tuples)
			{
				if (tuple.Item1 != null)
				{
					yield return (Uri.EscapeDataString(tuple.Item1), Uri.EscapeDataString(tuple.Item2));
				}
			}
		}

		protected IEnumerable<(string, string)> EscapeDataTuples(IEnumerable<(string, string)> tuples)
		{
			foreach (var tuple in tuples)
			{
				yield return (Uri.EscapeDataString(tuple.Item1), Uri.EscapeDataString(tuple.Item2));
			}
		}

		protected IEnumerable<string> ZipDataTuples(IEnumerable<(string, string)> tuples, Func<string, string> item2Transform = null)
		{
			item2Transform = item2Transform ?? (x => x);

			foreach (var tuple in tuples)
			{
				yield return $"{tuple.Item1}={item2Transform.Invoke(tuple.Item2)}";
			}
		}

		protected IEnumerable<(string, string)> UnzipDataTuples(IEnumerable<string> tuples)
		{
			foreach (var tuple in tuples)
			{
				var items = tuple.Split('=');

				yield return (items[0], items[1]);
			}
		}

		protected T DeserializeDataTuples<T>(IEnumerable<(string, string)> tuples, T anonymousTypeObject)
		{
			var properties = new StringBuilder();

			foreach (var tuple in tuples)
			{
				properties.Append($"\"{tuple.Item1}\" : \"{tuple.Item2}\", ");
			}

			return JsonConvert.DeserializeAnonymousType($"{{{properties}}}", anonymousTypeObject);
		}

		protected string CreateHMACSHA1Token(string message, string secret)
		{
			secret = secret ?? "";
			var encoding = new System.Text.ASCIIEncoding();
			byte[] keyByte = encoding.GetBytes(secret);
			byte[] messageBytes = encoding.GetBytes(message);
			using (var hmacsha1 = new HMACSHA1(keyByte))
			{
				byte[] hashmessage = hmacsha1.ComputeHash(messageBytes);
				return Convert.ToBase64String(hashmessage);
			}
		}

		public virtual async Task AuthorizeAsync(HttpResponse response)
		{
			var (requestToken, requestTokenSecret) = await RequestTokenAsync();

			_onTokenUpdate?.Invoke(requestToken, requestTokenSecret);

			// https://developer.twitter.com/en/docs/basics/authentication/guides/creating-a-signature.html

			var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

			var signatureBaseString = string.Join('&', new List<string>()
			{
				"GET",
				Uri.EscapeDataString(_loginUrl),
				Uri.EscapeDataString(string.Join('&', ZipDataTuples(EscapeDataTuples(ExcludeNullDataTuples(new List<(string, string)>()
					{
						(_promptKey, _promptValue),
						( "oauth_token", requestToken ),
						( "oauth_callback", _redirectUrl ),
						( "oauth_consumer_key", _clientId ),
						( "oauth_nonce", _stateId.ToString() ),
						( "oauth_signature_method", "HMAC-SHA1" ),
						( "oauth_timestamp", timestamp ),
						( "oauth_version", "1.0")
					}).OrderBy(s => s.Item1)))))
			});

			var signingKey = $"{Uri.EscapeDataString(_clientSecret)}&{Uri.EscapeDataString(requestTokenSecret)}";

			var signature = CreateHMACSHA1Token(signatureBaseString, signingKey);

			// https://developer.twitter.com/en/docs/basics/authentication/guides/authorizing-a-request

			var headerAuthorizationString = string.Join(", ", ZipDataTuples(EscapeDataTuples(new List<(string, string)>()
					{
						( "oauth_callback", _redirectUrl ),
						( "oauth_consumer_key", _clientId ),
						( "oauth_nonce", _stateId.ToString() ),
						( "oauth_signature", signature ),
						( "oauth_signature_method", "HMAC-SHA1" ),
						( "oauth_timestamp", timestamp ),
						( "oauth_version", "1.0")
					}).OrderBy(s => s.Item1), (s) => { return $"\"{s}\""; }));

			_httpClientSingleton.DefaultRequestHeaders.Add(HttpRequestHeader.Authorization.ToString(), $"OAuth {headerAuthorizationString}");

			response.RedirectTo($"{_loginUrl}?{_prompt}&oauth_token={requestToken}");
		}

		public async virtual Task<string> GetTokenAsync(string code, params string[] state)
		{
			var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

			var signatureBaseString = string.Join('&', new List<string>()
			{
				"POST",
				Uri.EscapeDataString(_tokenUrl),
				Uri.EscapeDataString(string.Join('&', ZipDataTuples(EscapeDataTuples(new List<(string, string)>()
					{
						( "oauth_verifier", code),
						( "oauth_token", state[0]),
						( "oauth_callback", _redirectUrl ),
						( "oauth_consumer_key", _clientId ),
						( "oauth_nonce", _stateId.ToString() ),
						( "oauth_signature_method", "HMAC-SHA1" ),
						( "oauth_timestamp", timestamp ),
						( "oauth_version", "1.0" )
					}).OrderBy(s => s.Item1))))
			});

			var signingKey = $"{Uri.EscapeDataString(_clientSecret)}&{Uri.EscapeDataString(state[1])}";

			var signature = CreateHMACSHA1Token(signatureBaseString, signingKey);

			// https://developer.twitter.com/en/docs/basics/authentication/guides/authorizing-a-request

			var headerAuthorizationString = string.Join(", ", ZipDataTuples(EscapeDataTuples(new List<(string, string)>()
					{
						( "oauth_token", state[0]),
						( "oauth_callback", _redirectUrl ),
						( "oauth_consumer_key", _clientId ),
						( "oauth_nonce", _stateId.ToString() ),
						( "oauth_signature", signature),
						( "oauth_signature_method", "HMAC-SHA1" ),
						( "oauth_timestamp", timestamp ),
						( "oauth_version", "1.0" )
					}).OrderBy(s => s.Item1), (s) => { return $"\"{s}\""; }));

			var getTokenResponse = await PostAsync($"{_tokenUrl}?oauth_verifier={code}", new NameValueCollection
			{
				{ HttpRequestHeader.Authorization.ToString(), $"OAuth {headerAuthorizationString}"},
			});

			var tokens = DeserializeDataTuples(UnzipDataTuples(getTokenResponse.Split('&')), new { oauth_token = (string)null, oauth_token_secret = (string)null });

			return JsonConvert.SerializeObject(new { access_token = tokens.oauth_token, access_token_secret = tokens.oauth_token_secret });
		}

		public virtual async Task<string> RefreshTokenAsync(string token)
		{
			throw new Exception($"Requested operation is not supported for {this.GetType().Name}");
		}

		public virtual async Task RevokeTokenAsync(string token)
		{
			if (_revokeTokenUrl == null)
			{
				throw new Exception($"Requested operation is not supported for {this.GetType().Name}");
			}
		}

		public bool CanRefreshToken { get { return false; } }
		public bool CanRevokeToken { get { return false; } }

		protected virtual async Task<string> GetDataAsync(string url, string data)
		{
			var tokens = JsonConvert.DeserializeAnonymousType(data, new { access_token = (string)null, access_token_secret = (string)null });

			var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

			var signatureBaseString = string.Join('&', new List<string>()
				{
					"GET",
					Uri.EscapeDataString(_dataUrl),
					Uri.EscapeDataString(string.Join('&', ZipDataTuples(EscapeDataTuples(new List<(string, string)>()
					{
						( "oauth_token", tokens.access_token),
						( "oauth_consumer_key", _clientId ),
						( "oauth_nonce", _stateId.ToString() ),
						( "oauth_signature_method", "HMAC-SHA1" ),
						( "oauth_timestamp", timestamp ),
						( "oauth_version", "1.0" )
					}).OrderBy(s => s.Item1))))
				});

			var signingKey = $"{Uri.EscapeDataString(_clientSecret)}&{Uri.EscapeDataString(tokens.access_token_secret)}";

			var signature = CreateHMACSHA1Token(signatureBaseString, signingKey);

			// https://developer.twitter.com/en/docs/basics/authentication/guides/authorizing-a-request

			var headerAuthorizationString = string.Join(", ", ZipDataTuples(EscapeDataTuples(new List<(string, string)>()
				{
					( "oauth_token", tokens.access_token),
					( "oauth_consumer_key", _clientId ),
					( "oauth_nonce", _stateId.ToString() ),
					( "oauth_signature", signature),
					( "oauth_signature_method", "HMAC-SHA1" ),
					( "oauth_timestamp", timestamp ),
					( "oauth_version", "1.0" )
				}).OrderBy(s => s.Item1), (s) => { return $"\"{s}\""; }));

			return await GetAsync(url, new NameValueCollection
			{
				{ HttpRequestHeader.Authorization.ToString(), $"OAuth {headerAuthorizationString}"},
			});
		}

		public virtual async Task<IUserData> GetUserDataAsync(string data)
		{
			_userData = new UserData(data);

			OnUserData(JsonConvert.DeserializeObject<dynamic>(!string.IsNullOrEmpty(_dataUrl) ? await GetDataAsync(_dataUrl, data) : data));

			return _userData;
		}

		protected abstract void OnUserData(dynamic data);
	}
}