using com.idpx.core.oauth.data;
using System.Threading.Tasks;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForWithings : OAuth2Client<UserData>, IOAuthClient
    {
        public OAuth2ClientForWithings(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
        }

        public override async Task<IUserData> GetUserDataAsync(string data)
        {
            return new UserDataForWithings(data);
        }

        protected override async Task OnUserDataAsync(dynamic data)
        {
        }
    }
}