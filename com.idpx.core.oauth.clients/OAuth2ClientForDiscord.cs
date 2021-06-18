using com.idpx.core.oauth.data;
using System.Threading.Tasks;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForDiscord: OAuth2Client<UserData>, IOAuthClient
    {
        public OAuth2ClientForDiscord(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
        }
        protected override async Task OnUserDataAsync(dynamic data)
        {
            #region Sample Data
            /*
{
  "id": "80351110224678912",
  "username": "Nelly",
  "discriminator": "1337",
  "avatar": "8342729096ea3675442027381ff50dfe",
  "verified": true,
  "email": "nelly@discordapp.com",
  "flags": 64,
  "premium_type": 1
}
             */
            #endregion

            _userData.ParseUserData(data?.id.Value, data?.email.Value, data?.username.Value);
        }
    }
}