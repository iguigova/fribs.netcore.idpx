using com.idpx.core.oauth.data;
using System.Net;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForYahoo : OAuth2Client<UserData>, IOAuthClient
    {
        public OAuth2ClientForYahoo(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
        }
        
        public override HttpContent TokenRequest(string code)
        {
            return new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", _redirectUrl}
            });
        }

        public override NameValueCollection DataRequestHeaders { get { return new NameValueCollection() { { HttpRequestHeader.Authorization.ToString(), "Basic " + Security.EncodeToBase64(_clientId + ":" + _clientSecret) } }; } }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            // nothing to do here
        }
    }
}