using System.Threading.Tasks;
using com.idpx.core.oauth.data;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForKeycloak : OAuth2Client<UserData>, IOAuthClient
    {
        public OAuth2ClientForKeycloak(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint)
        {
        }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            // do nothing
        }
    }
}