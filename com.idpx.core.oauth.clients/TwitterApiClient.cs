using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using LogonLabs.IdPx.API.Clients;
using LogonLabs.IdPx.Data;
using LogonLabs.IdPx.Impl;
using LogonLabs.Logging;
using LogonLabs.Shared;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace LogonLabs.IdPx.OAuth.Clients
{
	public class TwitterApiClient : ApiClient, IOAuthApiClient
	{
		private readonly IIdPxImpl _impl;

		// https://msdn.microsoft.com/en-us/library/azure/dn645542.aspx

		private string _loginUrl;
		private string _tokenUrl;
		private string _dataUrl;
		private string _redirectUrl;
		private string _clientId;
		private string _secret;
		private string _stateId;

		public TwitterApiClient(IIdPxImpl impl, string loginUrl, string tokenUrl, string dataUrl, string clientId, string secret, string redirectUrl, string stateId) : base(null)
		{
			_impl = impl;

			_loginUrl = loginUrl;
			_tokenUrl = tokenUrl;
			_dataUrl = dataUrl;
			_redirectUrl = redirectUrl;
			_clientId = clientId;
			_secret = secret;
            _stateId = stateId;

            OnException = (ex) => Logger.Error(ex, stateId: _stateId);
			OnInfo = (s) => Logger.Info(s, stateId: _stateId);
		}

		public void RequestToken(out string requestToken, out string requestTokenSecret)
		{
			// https://developer.twitter.com/en/docs/basics/authentication/guides/creating-a-signature.html

			var signatureBaseString = string.Join('&', new List<string>()
			{
				"POST", 
				Uri.EscapeDataString("https://api.twitter.com/oauth/request_token"),
				Uri.EscapeDataString(string.Join('&', ZipDataTuples(EscapeDataTuples(new List<(string, string)>()
					{
						( "oauth_callback", _redirectUrl ),
						( "oauth_consumer_key", _clientId ),
						( "oauth_nonce", _stateId.ToString() ),
						( "oauth_signature_method", "HMAC-SHA1" ),
						( "oauth_timestamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString() ),
						( "oauth_version", "1.0" )
					}).OrderBy(s => s.Item1))))
			});

			Logger.Info(signatureBaseString, stateId: _stateId);

			var signingKey = $"{Uri.EscapeDataString(_secret)}&";

			var signature = CreateHMACSHA1Token(signatureBaseString, signingKey);

			Logger.Info(signature, stateId: _stateId);

			// https://developer.twitter.com/en/docs/basics/authentication/guides/authorizing-a-request

			var headerAuthorizationString = string.Join(", ", ZipDataTuples(EscapeDataTuples(new List<(string, string)>()
					{
						( "oauth_callback", _redirectUrl ),
						( "oauth_consumer_key", _clientId ),
						( "oauth_nonce", _stateId.ToString() ),
						( "oauth_signature", signature),
						( "oauth_signature_method", "HMAC-SHA1" ),
						( "oauth_timestamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString() ),
						( "oauth_version", "1.0" )
					}).OrderBy(s => s.Item1), (s) => { return $"\"{s}\""; }));

			Logger.Info(headerAuthorizationString, stateId: _stateId);

			_client.Headers.Add(HttpRequestHeader.Authorization, $"OAuth {headerAuthorizationString}");

			var requestTokenResponse = Post("https://api.twitter.com/oauth/request_token");

			var requestTokens = DeserializeDataTuples(UnzipDataTuples(requestTokenResponse.Split('&')), new { oauth_token = (string)null, oauth_token_secret = (string)null, oauth_callback_confirmed = false });

			if (requestTokens.oauth_callback_confirmed)
			{
				requestToken = requestTokens.oauth_token;
				requestTokenSecret = requestTokens.oauth_token_secret;
			}
			else
			{
				requestToken = null;
				requestTokenSecret = null;
			}
		}

		private IEnumerable<(string, string)> EscapeDataTuples(IEnumerable<(string, string)> tuples)
		{
			foreach (var tuple in tuples)
			{
				yield return (Uri.EscapeDataString(tuple.Item1), Uri.EscapeDataString(tuple.Item2));
			}
		}

		private IEnumerable<string> ZipDataTuples(IEnumerable<(string, string)> tuples, Func<string, string> item2Transform = null)
		{
			item2Transform = item2Transform ?? (x => x);

			foreach (var tuple in tuples)
			{
				yield return $"{tuple.Item1}={item2Transform.Invoke(tuple.Item2)}";
			}
		}

		private IEnumerable<(string, string)> UnzipDataTuples(IEnumerable<string> tuples)
		{
			foreach(var tuple in tuples)
			{
				var items = tuple.Split('=');

				yield return (items[0], items[1]);
			}
		}

		public T DeserializeDataTuples<T>(IEnumerable<(string, string)> tuples, T anonymousTypeObject)
		{
			var properties = new StringBuilder();

			foreach(var tuple in tuples)
			{
				properties.Append($"\"{tuple.Item1}\" : \"{tuple.Item2}\", ");
			}

			return JsonConvert.DeserializeAnonymousType($"{{{properties}}}", anonymousTypeObject);
		}

		private string CreateHMACSHA1Token(string message, string secret)
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

		public void Authorize(HttpResponse response, string request = null)
		{
			RequestToken(out string requestToken, out string requestTokenSecret);

			//_cache.GetCached<IdPxStates, State>(() =>
			//{
			//	var state = _cache.PopCached<IdPxStates, State>((ex) => Logger.Error(ex, stateId: _stateId), _stateId);
			//	state.TwitterToken = requestToken;
			//	state.TwitterTokenSecret = requestTokenSecret;
			//	return state;
			//}, IdPxStates.TTL, (ex) => Logger.Error(ex, stateId: _stateId), requestToken);

            var state = _impl.PopState(_stateId.ToGuid());
            state.TwitterToken = requestToken;
            state.TwitterTokenSecret = requestTokenSecret;
            _impl.SetState(state);
            _impl.SetStateIdByToken(_stateId.ToGuid(), requestToken); //to help us retrieve it

			// https://developer.twitter.com/en/docs/basics/authentication/guides/creating-a-signature.html

			var signatureBaseString = string.Join('&', new List<string>()
			{
				"GET",
				Uri.EscapeDataString(_loginUrl),
				Uri.EscapeDataString(string.Join('&', ZipDataTuples(EscapeDataTuples(new List<(string, string)>()
					{
						( "oauth_token", requestToken),
						( "oauth_callback", _redirectUrl ),
						( "oauth_consumer_key", _clientId ),
						( "oauth_nonce", _stateId.ToString() ),
						( "oauth_signature_method", "HMAC-SHA1" ),
						( "oauth_timestamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString() ),
						( "oauth_version", "1.0" )
					}).OrderBy(s => s.Item1))))
			});

			Logger.Info(signatureBaseString, stateId: _stateId);

			var signingKey = $"{Uri.EscapeDataString(_secret)}&{Uri.EscapeDataString(requestTokenSecret)}";

			var signature = CreateHMACSHA1Token(signatureBaseString, signingKey);

			Logger.Info(signature, stateId: _stateId);

			// https://developer.twitter.com/en/docs/basics/authentication/guides/authorizing-a-request

			var headerAuthorizationString = string.Join(", ", ZipDataTuples(EscapeDataTuples(new List<(string, string)>()
					{
						( "oauth_callback", _redirectUrl ),
						( "oauth_consumer_key", _clientId ),
						( "oauth_nonce", _stateId.ToString() ),
						( "oauth_signature", signature),
						( "oauth_signature_method", "HMAC-SHA1" ),
						( "oauth_timestamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString() ),
						( "oauth_version", "1.0" )
					}).OrderBy(s => s.Item1), (s) => { return $"\"{s}\""; }));

			Logger.Info(headerAuthorizationString, stateId: _stateId);

			_client.Headers.Add(HttpRequestHeader.Authorization, $"OAuth {headerAuthorizationString}");

			response.StatusCode = (int) HttpStatusCode.Redirect;

			response.Headers.Add("Location", $"{_loginUrl}?oauth_token={requestToken}");
		}

		public string GetToken(string code, State state, string data = null)
		{
			var signatureBaseString = string.Join('&', new List<string>()
			{
				"POST",
				Uri.EscapeDataString(_tokenUrl),
				Uri.EscapeDataString(string.Join('&', ZipDataTuples(EscapeDataTuples(new List<(string, string)>()
					{
						( "oauth_verifier", code),
						( "oauth_token", state.TwitterToken),
						( "oauth_callback", _redirectUrl ),
						( "oauth_consumer_key", _clientId ),
						( "oauth_nonce", _stateId.ToString() ),
						( "oauth_signature_method", "HMAC-SHA1" ),
						( "oauth_timestamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString() ),
						( "oauth_version", "1.0" )
					}).OrderBy(s => s.Item1))))
			});

			Logger.Info(signatureBaseString, stateId: _stateId);

			var signingKey = $"{Uri.EscapeDataString(_secret)}&{Uri.EscapeDataString(state.TwitterTokenSecret)}";

			var signature = CreateHMACSHA1Token(signatureBaseString, signingKey);

			Logger.Info(signature, stateId: _stateId);

			// https://developer.twitter.com/en/docs/basics/authentication/guides/authorizing-a-request

			var headerAuthorizationString = string.Join(", ", ZipDataTuples(EscapeDataTuples(new List<(string, string)>()
					{
						( "oauth_token", state.TwitterToken),
						( "oauth_callback", _redirectUrl ),
						( "oauth_consumer_key", _clientId ),
						( "oauth_nonce", _stateId.ToString() ),
						( "oauth_signature", signature),
						( "oauth_signature_method", "HMAC-SHA1" ),
						( "oauth_timestamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString() ),
						( "oauth_version", "1.0" )
					}).OrderBy(s => s.Item1), (s) => { return $"\"{s}\""; }));

			Logger.Info(headerAuthorizationString, stateId: _stateId);

			_client.Headers.Add(HttpRequestHeader.Authorization, $"OAuth {headerAuthorizationString}");

			var getTokenResponse = Post($"{_tokenUrl}?oauth_verifier={code}");

			var tokens = DeserializeDataTuples(UnzipDataTuples(getTokenResponse.Split('&')), new { oauth_token = (string)null, oauth_token_secret = (string)null });

			return JsonConvert.SerializeObject(new { access_token = tokens.oauth_token, access_token_secret = tokens.oauth_token_secret });
		}

		public string RefreshToken(string token)
		{
			throw new NotImplementedException();
		}

		public string UserInfo(string token, string defaultDataUrl = null, string authorizationHeader = null, string newDataUrl = null, string method = null)
		{
			var tokens = JsonConvert.DeserializeAnonymousType(token, new { access_token = (string)null, access_token_secret = (string)null });

			var signatureBaseString = string.Join('&', new List<string>()
			{
				"GET",
				Uri.EscapeDataString(newDataUrl ?? _dataUrl),
				Uri.EscapeDataString(string.Join('&', ZipDataTuples(EscapeDataTuples(new List<(string, string)>()
					{
						( "include_entities", "false"),
						( "skip_status", "true"),
						( "include_email", "true"),
						( "oauth_token", tokens.access_token),
						( "oauth_callback", _redirectUrl ),
						( "oauth_consumer_key", _clientId ),
						( "oauth_nonce", _stateId.ToString() ),
						( "oauth_signature_method", "HMAC-SHA1" ),
						( "oauth_timestamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString() ),
						( "oauth_version", "1.0" )
					}).OrderBy(s => s.Item1))))
			});

			Logger.Info(signatureBaseString, stateId: _stateId);

			var signingKey = $"{Uri.EscapeDataString(_secret)}&{Uri.EscapeDataString(tokens.access_token_secret)}";

			var signature = CreateHMACSHA1Token(signatureBaseString, signingKey);

			Logger.Info(signature, stateId: _stateId);

			// https://developer.twitter.com/en/docs/basics/authentication/guides/authorizing-a-request

			var headerAuthorizationString = string.Join(", ", ZipDataTuples(EscapeDataTuples(new List<(string, string)>()
					{
						( "oauth_token", tokens.access_token),
						( "oauth_callback", _redirectUrl ),
						( "oauth_consumer_key", _clientId ),
						( "oauth_nonce", _stateId.ToString() ),
						( "oauth_signature", signature),
						( "oauth_signature_method", "HMAC-SHA1" ),
						( "oauth_timestamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString() ),
						( "oauth_version", "1.0" )
					}).OrderBy(s => s.Item1), (s) => { return $"\"{s}\""; }));

			Logger.Info(headerAuthorizationString, stateId: _stateId);

			_client.Headers.Add(HttpRequestHeader.Authorization, $"OAuth {headerAuthorizationString}");

			return Get($"{_dataUrl}?include_entities=false&skip_status=true&include_email=true");
		}
	}
}