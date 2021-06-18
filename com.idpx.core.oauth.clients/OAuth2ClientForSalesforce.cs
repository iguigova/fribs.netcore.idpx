using System.Threading.Tasks;
using System.Web;
using com.idpx.core.oauth.data;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForSalesforce : OAuth2Client<UserDataForSalesforce>, IOAuthClient
    {
        public OAuth2ClientForSalesforce(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
            if (_includeAuthDataInResponse && _scope.IndexOf("offline_access") < 0)
            {
                _scope += HttpUtility.UrlEncode(" offline_access");
            }
        }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            // nothing to do it
        }
    }
}