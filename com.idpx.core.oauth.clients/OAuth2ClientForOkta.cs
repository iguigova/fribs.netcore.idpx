using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using com.idpx.core.oauth.data;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForOkta : OAuth2Client<UserData>, IOAuthClient
    {
        // https://msdn.microsoft.com/en-us/library/azure/dn645542.aspx

        public OAuth2ClientForOkta(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
            if (string.IsNullOrEmpty(_dataUrl) && (Uri.TryCreate(loginUrl, UriKind.Absolute, out Uri loginUri)))
            {
                _dataUrl = $"https://{loginUri.Host}/oauth2/v1/userinfo";
            }

            if (string.IsNullOrEmpty(_revokeTokenUrl) && (Uri.TryCreate(loginUrl, UriKind.Absolute, out loginUri)))
            {
                _revokeTokenUrl = $"https://{loginUri.Host}/oauth2/v1/revoke";
            }

            if (_includeAuthDataInResponse && _scope.IndexOf("offline_access") < 0)
            {
                _scope += HttpUtility.UrlEncode(" offline_access");
            }
        }

        public override HttpContent RevokeTokenRequest(string token)
        {
            return new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "token", token },
                { "client_id", _clientId},
                { "client_secret", _clientSecret}
            });
        }

        public override string DataRequest { get { return $"{_dataUrl}?token={_userData.AccessToken}"; } }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            // {\"sub\":\"00ulsthfivZjo3Qon356\",\"name\":\"Developer LogonLabs\",\"locale\":\"en-US\",\"email\":\"developer@logonlabs.com\",\"preferred_username\":\"developer@logonlabs.com\",\"given_name\":\"Developer\",\"family_name\":\"LogonLabs\",\"zoneinfo\":\"America/Los_Angeles\",\"updated_at\":1558110983,\"email_verified\":true}"
            // {{  "sub": "00ulsthfivZjo3Qon356",  "name": "Developer LogonLabs",  "locale": "en-US",  "email": "developer@logonlabs.com",  "preferred_username": "developer@logonlabs.com",  "given_name": "Developer",  "family_name": "LogonLabs",  "zoneinfo": "America/Los_Angeles",  "updated_at": 1558110983,  "email_verified": true,  "groups": [    "Everyone",    "Test"  ]}}

            if (data?.groups != null && data.groups.Count > 0)
            {
                var groups = new List<string>();

                foreach (var group in data.groups.Children())
                {
                    groups.Add(group.Value);
                }

                _userData.Claims["groups"] = string.Join(',', groups);
            }

            if (data?.email_verified?.Value)
            {
                _userData.ParseUserData(data?.sub?.Value, data?.email?.Value, data?.given_name?.Value, data?.family_name?.Value);
            }
        }
    }
}