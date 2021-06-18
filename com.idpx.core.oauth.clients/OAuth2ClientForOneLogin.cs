using com.idpx.core.oauth.data;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForOneLogin : OAuth2Client<UserData>, IOAuthClient
    {
        public OAuth2ClientForOneLogin(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
            if (Uri.TryCreate(loginUrl, UriKind.Absolute, out Uri loginUri))
            {
                var path = loginUri.PathAndQuery.Substring(0, loginUri.PathAndQuery.LastIndexOf('/'));
                // path: /oidc or /oidc/2

                if (string.IsNullOrEmpty(_dataUrl))
                {                    
                    _dataUrl = $"https://{loginUri.Host}{path}/me";
                }

                if (string.IsNullOrEmpty(_revokeTokenUrl))
                {
                    _revokeTokenUrl = $"https://{loginUri.Host}{path}/token/revocation";
                }
            }
        }

        public override HttpContent RevokeTokenRequest(string token)
        {
            return new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "token", token },
                { "token_type_hint", "access_token"},
                { "client_id", _clientId},
                { "client_secret", _clientSecret}
            });
        }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            #region Sample Response
            /*
            {{
  "sub": "52458447",
  "email": "developer@logonlabs.com",
  "preferred_username": "developer@logonlabs.com",
  "name": "Developer LogonLabs"
}}
            */
            #endregion

            _userData.ParseUserData(data?.sub?.Value.ToString(), data?.email?.Value, data?.name?.Value);
        }
    }
}