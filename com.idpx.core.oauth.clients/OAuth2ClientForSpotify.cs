using com.idpx.core.oauth.data;
using System.Threading.Tasks;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForSpotify: OAuth2Client<UserData>, IOAuthClient
    {
        public OAuth2ClientForSpotify(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
        }
        protected override async Task OnUserDataAsync(dynamic data)
        {
            #region Sample Data
            /*
{{
  "display_name": "logonlabs",
  "email": "developer@logonlabs.com",
  "external_urls": {
    "spotify": "https://open.spotify.com/user/2f6l4jqmw4ejq8unk28n022rc"
  },
  "followers": {
    "href": null,
    "total": 0
  },
  "href": "https://api.spotify.com/v1/users/2f6l4jqmw4ejq8unk28n022rc",
  "id": "2f6l4jqmw4ejq8unk28n022rc",
  "images": [],
  "type": "user",
  "uri": "spotify:user:2f6l4jqmw4ejq8unk28n022rc"
}}
             */
            #endregion

            #region var t = Get(data?.href.Value);
            /*
 {
  "display_name" : "logonlabs",
  "external_urls" : {
    "spotify" : "https://open.spotify.com/user/2f6l4jqmw4ejq8unk28n022rc"
  },
  "followers" : {
    "href" : null,
    "total" : 0
  },
  "href" : "https://api.spotify.com/v1/users/2f6l4jqmw4ejq8unk28n022rc",
  "id" : "2f6l4jqmw4ejq8unk28n022rc",
  "images" : [ ],
  "type" : "user",
  "uri" : "spotify:user:2f6l4jqmw4ejq8unk28n022rc"
}
             */
            #endregion

            _userData.ParseUserData(data?.id.Value, data?.email.Value, data?.display_name.Value);
        }
    }
}