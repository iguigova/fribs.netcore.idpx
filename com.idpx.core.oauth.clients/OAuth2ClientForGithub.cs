using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using com.idpx.core.oauth.data;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForGithub : OAuth2Client<UserData>, IOAuthClient
    {
        protected string _emailUrl;

        public OAuth2ClientForGithub(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string emailUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
            _emailUrl = emailUrl;
        }

        public override NameValueCollection DataRequestHeaders { get { return new NameValueCollection() { { HttpRequestHeader.Authorization.ToString(), $"token {_userData.AccessToken}" } }; } }

        public override HttpContent RefreshTokenRequest(string token) { return null; }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            #region Sample Response - gihub docs - https://developer.github.com/v3/users/#get-the-authenticated-user
            /*
{
  "login": "octocat",
  "id": 1,
  "node_id": "MDQ6VXNlcjE=",
  "avatar_url": "https://github.com/images/error/octocat_happy.gif",
  "gravatar_id": "",
  "url": "https://api.github.com/users/octocat",
  "html_url": "https://github.com/octocat",
  "followers_url": "https://api.github.com/users/octocat/followers",
  "following_url": "https://api.github.com/users/octocat/following{/other_user}",
  "gists_url": "https://api.github.com/users/octocat/gists{/gist_id}",
  "starred_url": "https://api.github.com/users/octocat/starred{/owner}{/repo}",
  "subscriptions_url": "https://api.github.com/users/octocat/subscriptions",
  "organizations_url": "https://api.github.com/users/octocat/orgs",
  "repos_url": "https://api.github.com/users/octocat/repos",
  "events_url": "https://api.github.com/users/octocat/events{/privacy}",
  "received_events_url": "https://api.github.com/users/octocat/received_events",
  "type": "User",
  "site_admin": false,
  "name": "monalisa octocat",
  "company": "GitHub",
  "blog": "https://github.com/blog",
  "location": "San Francisco",
  "email": "octocat@github.com",
  "hireable": false,
  "bio": "There once was...",
  "twitter_username": "monatheoctocat",
  "public_repos": 2,
  "public_gists": 1,
  "followers": 20,
  "following": 0,
  "created_at": "2008-01-14T04:33:35Z",
  "updated_at": "2008-01-14T04:33:35Z",
  "private_gists": 81,
  "total_private_repos": 100,
  "owned_private_repos": 100,
  "disk_usage": 10000,
  "collaborators": 8,
  "two_factor_authentication": true,
  "plan": {
    "name": "Medium",
    "space": 400,
    "private_repos": 20,
    "collaborators": 0
  }
}             
             */
            #endregion

            #region Sample Response
            //{ { "login": "Logonlabs-dev",  "id": 49078951,  "node_id": "MDQ6VXNlcjQ5MDc4OTUx",  "avatar_url": "https://avatars2.githubusercontent.com/u/49078951?v=4",  "gravatar_id": "",  "url": "https://api.github.com/users/Logonlabs-dev",  "html_url": "https://github.com/Logonlabs-dev",  "followers_url": "https://api.github.com/users/Logonlabs-dev/followers",  "following_url": "https://api.github.com/users/Logonlabs-dev/following{/other_user}",  "gists_url": "https://api.github.com/users/Logonlabs-dev/gists{/gist_id}",  "starred_url": "https://api.github.com/users/Logonlabs-dev/starred{/owner}{/repo}",  "subscriptions_url": "https://api.github.com/users/Logonlabs-dev/subscriptions",  "organizations_url": "https://api.github.com/users/Logonlabs-dev/orgs",  "repos_url": "https://api.github.com/users/Logonlabs-dev/repos",  "events_url": "https://api.github.com/users/Logonlabs-dev/events{/privacy}",  "received_events_url": "https://api.github.com/users/Logonlabs-dev/received_events",  "type": "User",  "site_admin": false,  "name": null,  "company": null,  "blog": "",  "location": null,  "email": null,  "hireable": null,  "bio": null,  "public_repos": 0,  "public_gists": 0,  "followers": 0,  "following": 0,  "created_at": "2019-03-29T20:22:28Z",  "updated_at": "2019-09-19T21:30:41Z"} }
            #endregion

            #region Sample Response - email
            // {[  {    "email": "developer@logonlabs.com",    "primary": true,    "verified": true,    "visibility": "private"  },  {    "email": "49078951+Logonlabs-dev@users.noreply.github.com",    "primary": false,    "verified": true,    "visibility": null  }]}
            #endregion

            var email = data?.email?.Value;

            if (string.IsNullOrEmpty(email))
            {
                var emailData = JsonConvert.DeserializeObject<dynamic>(await GetAsync(_emailUrl, DataRequestHeaders));

                email = new List<dynamic>(emailData).Select(s => new { s.email, s.primary, s.verified, s.visibility }).FirstOrDefault(s => s.primary).email?.Value;
            }

            _userData.ParseUserData(data?.id?.Value.ToString(), email, data?.name?.Value ?? data?.login?.Value, avatar: data?.avatar_url?.Value);
        }
    }
}