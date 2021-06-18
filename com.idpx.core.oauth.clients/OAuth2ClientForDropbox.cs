using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using com.idpx.core.oauth.data;
using Newtonsoft.Json;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForDropbox : OAuth2Client<UserData>, IOAuthClient
    {
        public OAuth2ClientForDropbox(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
        }

        public override string AuthorizeRequest { get { return $"{_loginUrl}?client_id={_clientId}&state={_statePackageEncoded}&response_type=code&redirect_uri={_redirectUrl}&scope={_scope}&{_prompt}"; } }

        public override HttpContent RefreshTokenRequest(string token) { return null; }

        public override NameValueCollection RevokeTokenRequestHeaders(string token) { return new NameValueCollection() { { HttpRequestHeader.Authorization.ToString(), $"Bearer {token}" } }; }

        public override HttpContent RevokeTokenRequest(string token) { return null; }

        public override async Task<IUserData> GetUserDataAsync(string data)
        {
            _userData = new UserData(data);

            await OnUserDataAsync(JsonConvert.DeserializeObject(await PostAsync(_dataUrl, DataRequestHeaders)));

            return _userData;
        }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            #region Sample Response
            /*  https://www.dropbox.com/developers/documentation/http/documentation#users-get_current_account
            {
               "account_id": "dbid:AAALv4f3z2PmGZ46SKYJaMZFitQqO9mDn0g",
               "name": {
                   "given_name": "Developer",
                   "surname": "LogonLabs",
                   "familiar_name": "Developer",
                   "display_name": "Developer LogonLabs",
                   "abbreviated_name": "DL"
               },
               "email": "developer@logonlabs.com",
               "email_verified": true,
               "disabled": false,
               "country": "CA",
               "locale": "en-GB",
               "referral_link": "https://www.dropbox.com/referrals/AAB2Z71Fwjygubsn_n1E2dOGVFGvPD5gumg?src=app9-5687424",
               "is_paired": false,
               "account_type": {
                   ".tag": "basic"
               },
               "root_info": {
                   ".tag": "user",
                   "root_namespace_id": "5865991104",
                   "home_namespace_id": "5865991104"
               },
               "profile_photo_url": "https://dl-web.dropbox.com/account_photo/get/dbaphid%3AAAHWGmIXV3sUuOmBfTz0wPsiqHUpBWvv3ZA?vers=1556069330102\u0026size=128x128",
           }
            */
            #endregion

            _userData.ParseUserData(data?.account_id?.Value, data?.email?.Value, data?.name?.given_name?.Value, data?.name?.surname?.Value, data?.profile_photo_url?.Value);
        }
    }
}