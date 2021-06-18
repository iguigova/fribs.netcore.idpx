using com.idpx.core.oauth.data;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForInstagram : OAuth2Client<UserData>, IOAuthClient
    {
        public OAuth2ClientForInstagram(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
        }

        public override string AuthorizeRequest { get { return $"{_loginUrl}?app_id={_clientId}&state={_statePackageEncoded}&response_type=code&redirect_uri={_redirectUrl}&scope={_scope}"; } }
        
        public override HttpContent TokenRequest(string code)
        {
            return new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "app_id", _clientId},
                { "app_secret", _clientSecret},
                { "redirect_uri", _redirectUrl}
            });
        }

        public override string DataRequest { get { return $"{_dataUrl}&accesstoken={_userData.AccessToken}"; } }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            #region Sample Data
            /*
{
  "id": "17841405793187218",
  "username": "jayposiris"
}
             */
            #endregion

            _userData.ParseUserData(data?.id.Value, null, data?.username.Value);
        }
    }
}