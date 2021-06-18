using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Collections.Specialized;
using com.idpx.core.oauth.data;
using System.Threading.Tasks;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForBitbucket: OAuth2Client<UserData>, IOAuthClient
    {
        protected string _emailUrl;

        public OAuth2ClientForBitbucket(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string emailUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
            _emailUrl = emailUrl;
        }

        public override NameValueCollection DataRequestHeaders { get { return new NameValueCollection() { { HttpRequestHeader.Authorization.ToString(), "Basic " + Security.EncodeToBase64(_clientId + ":" + _clientSecret) } }; } }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            #region Sample Response
            //{{  "username": "iguigovalogonlabs",  "display_name": "Ilka Guigova",  "has_2fa_enabled": null,  "links": {    "hooks": {      "href": "https://api.bitbucket.org/2.0/users/%7B768f8c62-7f2a-4914-bf33-3f412fc1d3a4%7D/hooks"    },    "self": {      "href": "https://api.bitbucket.org/2.0/users/%7B768f8c62-7f2a-4914-bf33-3f412fc1d3a4%7D"    },    "repositories": {      "href": "https://api.bitbucket.org/2.0/repositories/%7B768f8c62-7f2a-4914-bf33-3f412fc1d3a4%7D"    },    "html": {      "href": "https://bitbucket.org/%7B768f8c62-7f2a-4914-bf33-3f412fc1d3a4%7D/"    },    "avatar": {      "href": "https://avatar-management--avatars.us-west-2.prod.public.atl-paas.net/5c65cab48bcc9e016aeece31/5f898db1-fbf6-456c-a2a5-32f917de04bf/128"    },    "snippets": {      "href": "https://api.bitbucket.org/2.0/snippets/%7B768f8c62-7f2a-4914-bf33-3f412fc1d3a4%7D"    }  },  "nickname": "Ilka Guigova",  "account_id": "5c65cab48bcc9e016aeece31",  "created_on": "2019-02-19T10:36:46.361566-08:00",  "is_staff": false,  "account_status": "active",  "type": "user",  "uuid": "{768f8c62-7f2a-4914-bf33-3f412fc1d3a4}"}}
            #endregion

            #region Sample Response - email
            // {"pagelen": 10, "values": [{"is_primary": true, "is_confirmed": true, "type": "email", "email": "iguigova@logonlabs.com", "links": {"self": {"href": "https://api.bitbucket.org/2.0/user/emails/iguigova@logonlabs.com"}}}], "page": 1, "size": 1}
            #endregion

            var emailData = JsonConvert.DeserializeObject<dynamic>(await GetAsync(_emailUrl, DataRequestHeaders));
            
            var email = new List<dynamic>(emailData.values).Select(s => new { s.email, s.is_primary, s.is_confirmed }).FirstOrDefault(s => s.is_primary).email?.Value;

            _userData.ParseUserData(data?.uuid?.Value.ToString(), email, data?.display_name?.Value);
        }
    }
}