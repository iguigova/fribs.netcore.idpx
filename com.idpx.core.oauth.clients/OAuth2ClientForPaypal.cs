using com.idpx.core.oauth.data;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForPaypal: OAuth2Client<UserDataForPaypal>, IOAuthClient
    {
        public OAuth2ClientForPaypal(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
        }

        public override string AuthorizeRequest { get { return $"{_loginUrl}?flowEntry=static&client_id={_clientId}&state={_statePackageEncoded}&response_type=code&redirect_uri={_redirectUrl}&scope={_scope}"; } }

        public override NameValueCollection TokenRequestHeaders { get { return new NameValueCollection() { { HttpRequestHeader.Authorization.ToString(), "Basic " + Security.EncodeToBase64(_clientId + ":" + _clientSecret) } }; } }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            #region Sample Data
            /*
{
  "user_id": "https://www.paypal.com/webapps/auth/identity/user/mWq6_1sU85v5EG9yHdPxJRrhGHrnMJ-1PQKtX6pcsmA",
  "name": "identity test",
  "given_name": "identity",
  "family_name": "test",
  "payer_id": "WDJJHEBZ4X2LY",
  "address": {
    "street_address": "1 Main St",
    "locality": "San Jose",
    "region": "CA",
    "postal_code": "95131",
    "country": "US"
  },
  "verified_account": "true",
  "emails": [
    {
      "value": "user1@example.com",
      "primary": true
    }
  ]
}

            {{  "user_id": "https://www.paypal.com/webapps/auth/identity/user/p8cOiK9O7j0czoxGo4tUX23wkio-TyYgsgbigwPED7g"}}

            {{  "user_id": "https://www.paypal.com/webapps/auth/identity/user/p8cOiK9O7j0czoxGo4tUX23wkio-TyYgsgbigwPED7g",  "name": "John Doe",  "emails": [    {      "value": "sb-43gytv558963@personal.example.com",      "primary": true,      "confirmed": true    }  ]}}

             */
            #endregion

            var email = (data?.emails != null) ? new List<dynamic>(data?.emails).Select(s => new { s.value, s.primary }).FirstOrDefault(s => s.primary).value?.Value : (string)null;

            _userData.ParseUserData(data?.user_id?.Value, email, data?.name?.Value);
        }
    }
}