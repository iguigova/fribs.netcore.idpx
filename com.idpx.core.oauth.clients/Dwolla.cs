using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using com.idpx.core.oauth.data;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace com.idpx.core.oauth.clients
{
	public class Dwolla : OAuthApiClient, IOAuthClient
	{
		// https://msdn.microsoft.com/en-us/library/azure/dn645542.aspx

		private string _tokenUrl;
		private string _dataUrl;
		private string _clientId;
		private string _clientSecret;

		public Dwolla(string tokenUrl, string dataUrl, string clientId, string clientSecret, string stateId) : base(stateId)
		{
			// It is assumed that secret, scope, and redirectUrl strings are url encoded

			_stateId = stateId;
			_tokenUrl = tokenUrl;
			_dataUrl = dataUrl;
			_clientId = clientId;
			_clientSecret = clientSecret;
		}
		public async Task AuthorizeAsync(HttpResponse response)
		{
		}

		public async Task<string> GetTokenAsync(string code, params string[] state)
		{
			return await PostAsync(_tokenUrl, new NameValueCollection()
			{
				{ HttpRequestHeader.Authorization.ToString(), "Basic " + Security.EncodeToBase64(_clientId + ":" + _clientSecret) }
			}, 
			new FormUrlEncodedContent(new Dictionary<string,string>()
			{
				{ "grant_type", "client_credentials" }
			}));
		}

		public async Task<string> RefreshTokenAsync(string token)
		{
			throw new Exception($"Requested operation is not supported for {this.GetType().Name}");
		}

		public async Task RevokeTokenAsync(string token)
		{
			throw new Exception($"Requested operation is not supported for {this.GetType().Name}");
		}

		public bool CanRefreshToken { get { return false; } }
		public bool CanRevokeToken { get { return false; } }

		public async Task<IUserData> GetUserDataAsync(string data)
		{
			_userData = new UserData(data);

			#region Sample Response
			/*
                        {
                "_links": {
                    "account": {
                        "href": "https://api-sandbox.dwolla.com/accounts/a903e93b-120e-48a2-a8c9-65ba14201bc7",
                        "type": "application/vnd.dwolla.v1.hal+json",
                        "resource-type": "account"
                    },
                    "events": {
                        "href": "https://api-sandbox.dwolla.com/events",
                        "type": "application/vnd.dwolla.v1.hal+json",
                        "resource-type": "event"
                    },
                    "webhook-subscriptions": {
                        "href": "https://api-sandbox.dwolla.com/webhook-subscriptions",
                        "type": "application/vnd.dwolla.v1.hal+json",
                        "resource-type": "webhook-subscription"
                    },
                    "customers": {
                        "href": "https://api-sandbox.dwolla.com/customers",
                        "type": "application/vnd.dwolla.v1.hal+json",
                        "resource-type": "customer"
                    }
                }
            }
                        */
			#endregion

			var response = await GetAsync(_dataUrl, new NameValueCollection 
			{
				{ HttpRequestHeader.Authorization.ToString(), $"Bearer {_userData.AccessToken}" },
				{ HttpRequestHeader.Accept.ToString(), "application/vnd.dwolla.v1.hal+json" }
			});

			var root = JsonConvert.DeserializeAnonymousType(response, new { _links = new { account = new { href = (string)null } } });

			#region Sample Response - root
			// { _links = { account = { href = "https://api-sandbox.dwolla.com/accounts/a903e93b-120e-48a2-a8c9-65ba14201bc7" } } }
			#endregion

			OnUserData(JsonConvert.DeserializeObject(await GetAsync(root._links.account.href, new NameValueCollection
			{
				{ HttpRequestHeader.Authorization.ToString(), $"Bearer {_userData.AccessToken}" },
				{ HttpRequestHeader.Accept.ToString(), "application/vnd.dwolla.v1.hal+json" }
			})));

			return _userData;
		}

		protected void OnUserData(dynamic data)
		{
			#region Sample Response
			/*
						{{
			  "_links": {
				"self": {
				  "href": "https://api-sandbox.dwolla.com/accounts/a903e93b-120e-48a2-a8c9-65ba14201bc7",
				  "type": "application/vnd.dwolla.v1.hal+json",
				  "resource-type": "account"
				},
				"receive": {
				  "href": "https://api-sandbox.dwolla.com/transfers",
				  "type": "application/vnd.dwolla.v1.hal+json",
				  "resource-type": "transfer"
				},
				"funding-sources": {
				  "href": "https://api-sandbox.dwolla.com/accounts/a903e93b-120e-48a2-a8c9-65ba14201bc7/funding-sources",
				  "type": "application/vnd.dwolla.v1.hal+json",
				  "resource-type": "funding-source"
				},
				"transfers": {
				  "href": "https://api-sandbox.dwolla.com/accounts/a903e93b-120e-48a2-a8c9-65ba14201bc7/transfers",
				  "type": "application/vnd.dwolla.v1.hal+json",
				  "resource-type": "transfer"
				},
				"customers": {
				  "href": "https://api-sandbox.dwolla.com/customers",
				  "type": "application/vnd.dwolla.v1.hal+json",
				  "resource-type": "customer"
				},
				"send": {
				  "href": "https://api-sandbox.dwolla.com/transfers",
				  "type": "application/vnd.dwolla.v1.hal+json",
				  "resource-type": "transfer"
				}
			  },
			  "id": "a903e93b-120e-48a2-a8c9-65ba14201bc7",
			  "name": "LogonLabs",
			  "timezoneOffset": -6.0,
			  "type": "Commercial"
			}}
			*/
			#endregion

			_userData.ParseUserData(data?.id?.Value.ToString(), data?.email?.Value, data?.name?.Value);
		}
	}
}