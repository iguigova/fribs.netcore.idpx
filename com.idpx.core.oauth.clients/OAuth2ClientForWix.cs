using com.idpx.core.oauth.data;
using System.Threading.Tasks;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForWix : OAuth2Client<UserData>, IOAuthClient
    {
        public OAuth2ClientForWix(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
        }

        public override string AuthorizeRequest { get { return $"{_loginUrl}?appid={_clientId}&state={_statePackageEncoded}&redirect_uri={_redirectUrl}"; } }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            // TODO: DataUrl = ?
        }
    }
}