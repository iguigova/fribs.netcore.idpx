using com.idpx.core.oauth.data;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForLinkedIn : OAuth2Client<UserData>, IOAuthClient
    {
        protected string _emailUrl;

        public OAuth2ClientForLinkedIn(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string emailUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
            _emailUrl = emailUrl;
        }

        public override HttpContent RefreshTokenRequest(string token) { return null; }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            #region Sample Response - linkedin docs - https://docs.microsoft.com/en-us/linkedin/shared/integrations/people/profile-api
            /*
  {
   "firstName":{
      "localized":{
         "en_US":"Bob"
      },
      "preferredLocale":{
         "country":"US",
         "language":"en"
      }
   },
   "localizedFirstName": "Bob",
   "headline":{
      "localized":{
         "en_US":"API Enthusiast at LinkedIn"
      },
      "preferredLocale":{
         "country":"US",
         "language":"en"
      }
   },
   "localizedHeadline": "API Enthusiast at LinkedIn",
   "vanityName": "bsmith",
   "id":"yrZCpj2Z12",
   "lastName":{
      "localized":{
         "en_US":"Smith"
      },
      "preferredLocale":{
         "country":"US",
         "language":"en"
      }
   },
   "localizedLastName": "Smith",
   "profilePicture": {
        "displayImage": "urn:li:digitalmediaAsset:C4D00AAAAbBCDEFGhiJ"
   }
}
             */
            #endregion

            #region Sample Response
            /*
{{
  "localizedLastName": "LogonLabs",
  "lastName": {
    "localized": {
      "en_US": "LogonLabs"
    },
    "preferredLocale": {
      "country": "US",
      "language": "en"
    }
  },
  "firstName": {
    "localized": {
      "en_US": "Developer"
    },
    "preferredLocale": {
      "country": "US",
      "language": "en"
    }
  },
  "id": "j9ltpQR4H5",
  "localizedFirstName": "Developer"
}}
*/
            #endregion

            #region Sample Response - emails
            /*
                        {{
              "elements": [
                {
                  "handle~": {
                    "emailAddress": "developer@logonlabs.com"
                  },
                  "handle": "urn:li:emailAddress:7782215395"
                }
              ]
            }}
            */
            #endregion

            var email = (string)null;

            if (string.IsNullOrEmpty(email))
            {
                var emailData = JsonConvert.DeserializeObject<dynamic>(await GetAsync(_emailUrl, DataRequestHeaders));
                email = emailData?.elements[0]["handle~"]["emailAddress"];
            }

            _userData.ParseUserData(data?.id?.Value.ToString(), email, data?.localizedFirstName?.Value, data?.localizedLastName?.Value, data?.profilePictire?.displayImage?.Value);
        }
    }
}