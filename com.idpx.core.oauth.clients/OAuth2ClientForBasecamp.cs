using com.idpx.core.oauth.data;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForBasecamp : OAuth2Client<UserData>, IOAuthClient
    {
        public OAuth2ClientForBasecamp(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
        }

        public override string AuthorizeRequest { get { return $"{_loginUrl}?type=web_server&client_id={_clientId}&state={_statePackageEncoded}&redirect_uri={_redirectUrl}"; } }

        public override HttpContent TokenRequest(string code)
        {
            return new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "type", "web_server" },
                { "code", code },
                { "client_id", _clientId},
                { "client_secret", _clientSecret},
                { "redirect_uri", _redirectUrl}
            });
        }

        public override HttpContent RefreshTokenRequest(string token)
        {
            return new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "grant_type", "refresh" },
                { "refresh_token", token },
                { "client_id", _clientId},
                { "client_secret", _clientSecret},
                { "redirect_uri", _redirectUrl}
            });
        }

        public override HttpContent RevokeTokenRequest(string token)
        {
            return new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "token", token },
            });
        }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            _userData.ParseUserData(data?.identity?.id.Value.ToString(), data?.identity?.email_address?.Value, data?.identity?.first_name?.Value, data?.identity?.last_name?.Value);
        }
    }
}