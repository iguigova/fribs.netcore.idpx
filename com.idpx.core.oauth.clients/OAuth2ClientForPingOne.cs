using com.idpx.core.oauth.data;
using System.Threading.Tasks;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForPingOne: OAuth2Client<UserData>, IOAuthClient
    {
        public OAuth2ClientForPingOne(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
        }

        //public override string AuthorizeRequest { get { return $"{_loginUrl}?client_id={_clientId}&state={_stateId}&response_type=code&redirect_uri={_redirectUrl}&scope={_scope}&acr_values=Single_Factor&prompt=login"; } }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            // nothing to do it
        }
    }
}