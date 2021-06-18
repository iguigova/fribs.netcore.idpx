using com.idpx.core.oauth.data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForQuickbooks: OAuth2Client<UserData>, IOAuthClient
    {
        public OAuth2ClientForQuickbooks(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
        }

        public override NameValueCollection RefreshTokenRequestHeaders { get { return new NameValueCollection() { { HttpRequestHeader.Authorization.ToString(), "Basic " + Security.EncodeToBase64(_clientId + ":" + _clientSecret) } }; } }

        public override HttpContent RefreshTokenRequest(string token)
        {
            return new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", token }
            });
        }

        public override NameValueCollection RevokeTokenRequestHeaders(string token) { return new NameValueCollection() { { HttpRequestHeader.Authorization.ToString(), "Basic " + Security.EncodeToBase64(_clientId + ":" + _clientSecret) } }; }

        public override HttpContent RevokeTokenRequest(string token)
        {
            return new StringContent(JsonConvert.SerializeObject(new { token }), Encoding.UTF8, "application/json");
        }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            #region Sample Response
            /*
                        { {
              "sub": "cac77cbc-92fe-469e-87bc-6b4b591b71ff",
              "givenName": "Developer",
              "familyName": "LogonLabs",
              "email": "developer@logonlabs.com",
              "emailVerified": true
            }}
            */
            #endregion

            #region Response Data
            /*
            { {
              "sub": "56ccbf16-f5d7-45a9-b808-34309287e111",
              "email": "qa@softwarecreated.com",
              "emailVerified": true
            }}
            */
            #endregion

            if (data?.emailVerified?.Value)
            {
                _userData.ParseUserData(null, data?.email?.Value, data?.givenName?.Value, data?.familyName?.Value);
            }
        }
    }
}