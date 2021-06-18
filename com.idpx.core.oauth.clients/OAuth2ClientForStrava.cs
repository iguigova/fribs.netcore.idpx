using com.idpx.core.oauth.data;
using System.Threading.Tasks;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForStrava:  OAuth2Client<UserData>, IOAuthClient
    {
        public OAuth2ClientForStrava(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
        }
        protected override async Task OnUserDataAsync(dynamic data)
        {
            #region Sample Data
            /*
{{
  "id": 48074738,
  "username": null,
  "resource_state": 2,
  "firstname": "Developer",
  "lastname": "LogonLabs",
  "city": "Toronto",
  "state": "Ontario",
  "country": "Canada",
  "sex": null,
  "premium": false,
  "summit": false,
  "created_at": "2019-11-07T18:32:04Z",
  "updated_at": "2019-11-13T17:48:56Z",
  "badge_type_id": 0,
  "profile_medium": "avatar/athlete/medium.png",
  "profile": "avatar/athlete/large.png",
  "friend": null,
  "follower": null
}}

{\"id\":47690079,
\"username\":null,
\"resource_state\":2,
\"firstname\":\"Alex\",
\"lastname\":\"Wilson\",
\"city\":null,\"state\":null,\"country\":null,\"sex\":\"M\",\"premium\":false,\"summit\":false,\"created_at\":\"2019-10-24T16:08:24Z\",\"updated_at\":\"2019-11-19T23:08:02Z\",\"badge_type_id\":0,\"profile_medium\":\"https://lh5.googleusercontent.com/-lvpTcxYoguc/AAAAAAAAAAI/AAAAAAAAAAA/ACHi3rdTrYHSapli4umlMtJT2u07v2IXVw/photo.jpg\",\"profile\":\"https://lh5.googleusercontent.com/-lvpTcxYoguc/AAAAAAAAAAI/AAAAAAAAAAA/ACHi3rdTrYHSapli4umlMtJT2u07v2IXVw/photo.jpg\",\"friend\":null,\"follower\":null}" 
             */
            #endregion

            _userData.ParseUserData(data?.id?.Value.ToString(), data?.username?.Value, data?.firstname?.Value, data?.lastname?.Value, data?.profile?.Value);
        }
    }
}