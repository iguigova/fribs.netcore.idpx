using com.idpx.core.oauth.data;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForTwitch: OAuth2Client<UserData>, IOAuthClient
    {
        public OAuth2ClientForTwitch(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
        }

        public override string AuthorizeRequest { get { return $"{_loginUrl}?client_id={_clientId}&state={_statePackageEncoded}&response_type=code&redirect_uri={_redirectUrl}&scope={_scope}&claims={{\"id_token\":{{\"email\":null,\"email_verified\":null,\"preferred_username\":null,\"picture\":null}}}}&{_prompt}"; } }

        public override HttpContent RefreshTokenRequest(string token) { return null; }

        public override HttpContent RevokeTokenRequest(string token)
        {
            return new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "client-id", _clientId },
                { "token", token },
            });
        }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            // nothing to do it
        }
    }
}