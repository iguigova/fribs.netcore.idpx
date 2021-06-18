using com.idpx.core.oauth.data;
using System.Threading.Tasks;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForGoogle:  OAuth2Client<UserData>, IOAuthClient
    {
        public OAuth2ClientForGoogle(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
        }

        public override string AuthorizeRequest 
        { 
            get 
            {
                return _includeAuthDataInResponse ? base.AuthorizeRequest + "&access_type=offline" :  base.AuthorizeRequest;
            } 
        }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            // do nothing
        }
    }
}