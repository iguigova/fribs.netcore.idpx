using com.idpx.core.oauth.data;
using System.Threading.Tasks;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForShopify : OAuth2Client<UserData>, IOAuthClient
    {
        public OAuth2ClientForShopify(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
        }

        public override string AuthorizeRequest { get { return $"{_loginUrl}?client_id={_clientId}&state={_statePackageEncoded}&response_type=code&redirect_uri={_redirectUrl}&scope={_scope}&grant_options[]=per-user"; } }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            #region Sample Response
            /*
{{
"access_token": "fc98fca68101844255ccf54a8bec4c93",
"scope": "read_customers",
"expires_in": 86398,
"associated_user_scope": "read_customers",
"associated_user": {
"id": 38418251856,
"first_name": "Developer",
"last_name": "LogonLabs",
"email": "developer@logonlabs.com",
"account_owner": true,
"locale": "en-CA",
"collaborator": false,
"email_verified": false
}
}}                  
             */
            #endregion

            _userData.ParseUserData(data?.associated_user?.id?.Value.ToString(), data?.associated_user?.email?.Value, data?.associated_user?.first_name?.Value, data?.associated_user?.last_name?.Value);
        }
    }
}