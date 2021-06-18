using com.idpx.core.oauth.data;
using System.Threading.Tasks;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForAmazon: OAuth2Client<UserData>, IOAuthClient
    {
        public OAuth2ClientForAmazon(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
        }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            #region Sample Response
            /*
              {
                "user_id": "amznl.account.K2LI23KL2LK2",
                "email":"mhashimoto-04@plaxo.com",
                "name" :"Mork Hashimoto",
                "postal_code": "98052"
             }
             */
            #endregion

            _userData.ParseUserData(data?.user_id?.Value.ToString(), data?.email?.Value, data?.name?.Value);
        }
    }
}