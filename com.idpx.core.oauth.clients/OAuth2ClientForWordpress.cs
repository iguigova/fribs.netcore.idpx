using com.idpx.core.oauth.data;
using System.Threading.Tasks;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForWordpress: OAuth2Client<UserData>, IOAuthClient
    {
        public OAuth2ClientForWordpress(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
        }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            #region Sample Response
            /*
{{
"ID": 158468313,
"display_name": "logonlabs",
"username": "logonlabs",
"email": "developer@logonlabs.com",
"primary_blog": 163479709,
"primary_blog_url": "http://logonlabsdemo.wordpress.com",
"primary_blog_is_jetpack": false,
"language": "en-gb",
"locale_variant": "",
"token_site_id": false,
"token_scope": [
"auth"
],
"avatar_URL": "https://0.gravatar.com/avatar/673c0010807c5f65d08c9bba846c5e4f?s=96&d=identicon",
"profile_URL": "http://en.gravatar.com/logonlabs",
"verified": true,
"email_verified": true,
"date": "2019-06-03T15:57:15-07:00",
"site_count": 1,
"visible_site_count": 1,
"has_unseen_notes": false,
"newest_note_type": "",
"phone_account": false,
"meta": {
"links": {
  "self": "https://public-api.wordpress.com/rest/v1/me",
  "help": "https://public-api.wordpress.com/rest/v1/me/help",
  "site": "https://public-api.wordpress.com/rest/v1/sites/5836086",
  "flags": "https://public-api.wordpress.com/rest/v1/me/flags"
}
},
"is_valid_google_apps_country": true,
"user_ip_country_code": "CA",
"social_login_connections": null,
"social_signup_service": null,
"abtests": {}
}}                 
             */
            #endregion

            _userData.ParseUserData(data?.ID?.Value.ToString(), data?.email?.Value, data?.display_name?.Value, avatar: data?.avatar_URL?.Value);
        }
    }
}