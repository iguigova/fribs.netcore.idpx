using com.idpx.core.oauth.data;
using System.Net.Http;
using System.Threading.Tasks;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForSlack: OAuth2Client<UserData>, IOAuthClient
    {
        public OAuth2ClientForSlack(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
        }

        public override HttpContent RefreshTokenRequest(string token) { return null; }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            #region Sample Response
            // { { "ok": true,  "user": { "name": "Ilka",    "id": "UG37Y6ZPS",    "email": "iguigova@logonlabs.com"  },  "team": { "id": "TG5BARSRL"  } } }
            #endregion

            if (data?.ok.Value ?? false)
            {
                _userData.ParseUserData(data?.user?.id.Value, data?.user?.email.Value, data?.user?.name.Value);
            }
        }
    }
}