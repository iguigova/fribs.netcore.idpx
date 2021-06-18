using com.idpx.core.oauth.data;
using System.Threading.Tasks;
using System.Web;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForMicrosoft: OAuth2Client<UserData>, IOAuthClient
    {
        // https://msdn.microsoft.com/en-us/library/azure/dn645542.aspx

        public OAuth2ClientForMicrosoft(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
            if (_includeAuthDataInResponse && _scope.IndexOf("offline_access") < 0)
            {
                _scope += HttpUtility.UrlEncode(" offline_access");
            }
        }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            _userData.ParseUserData(null, null, data?.givenName?.Value, data?.surname?.Value);
        }
    }
}