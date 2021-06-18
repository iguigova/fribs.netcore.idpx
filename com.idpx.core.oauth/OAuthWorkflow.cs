using com.idpx.core.oauth.clients;
using com.idpx.core.oauth.data;
using com.idpx.data;
using fribs.netcore.cache;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace com.idpx.core.oauth
{
	public class OAuthWorkflow
	{
        // https://msdn.microsoft.com/en-us/library/azure/dn645542.aspx

		protected readonly IOAuthClient _client;
        protected readonly ICache _cache;
        protected readonly State _state;

        public OAuthWorkflow(Provider provider, State state, string redirectUrl = null, string hint = null)
        {
            _state = state; //_cache.GetCached<States, State>(() => new State() { Id = stateId, ProviderId = provider.Id }, States.TTL, onException, stateId); 

            var stateId = state.Id;

            var type = provider.Type;
            var requestTokenUrl = provider.RequestTokenUrl; // oauth 1.0a
            var loginUrl = provider.LoginUrl;
            var tokenUrl = provider.TokenUrl;
            var revokeTokenUrl = provider.RevokeTokenUrl;
            var dataUrl = provider.DataUrl;
            var emailUrl = provider.EmailUrl;
            var scope = provider.Scopes;
            var clientId = provider.ClientId;
            var clientSecret = provider.ClientSecret;
            var prompt = provider.Prompt; // force authentication feature

            void onException(Exception ex)
            {
                //Logger.Error(ex, stateId: _stateId)
                throw ex;
            }

            void onOAuth10aTokenUpdate(string token, string tokenSecret)
            {
                _cache.GetCached<States, State>(() =>
                {
                    var state = _cache.PopCached<States, State>(onException, stateId);
                    state.OAuth10aToken = token;
                    state.OAuth10aTokenSecret = tokenSecret;
                    return state;
                }, States.TTL, onException, token);
            };

            switch (type)
            {
                case EProviders.microsoft: _client = new OAuth2ClientForMicrosoft(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId,  prompt: prompt, hint: hint); break;
                case EProviders.google: _client = new OAuth2ClientForGoogle(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.facebook: _client = new OAuth2ClientForFacebook(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.linkedin: _client = new OAuth2ClientForLinkedIn(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, emailUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.okta: _client = new OAuth2ClientForOkta(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.slack: _client = new OAuth2ClientForSlack(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.twitter: _client = new OAuth10aClientForTwitter(requestTokenUrl, loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, redirectUrl, stateId, prompt, onOAuth10aTokenUpdate); break;
                case EProviders.github: _client = new OAuth2ClientForGithub(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, emailUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.quickbooks: _client = new OAuth2ClientForQuickbooks(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.onelogin: _client = new OAuth2ClientForOneLogin(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.apple: _client = new OAuth2ClientForApple(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.basecamp: _client = new OAuth2ClientForBasecamp(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.dropbox: _client = new OAuth2ClientForDropbox(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.dwolla: _client = new Dwolla(tokenUrl, dataUrl, clientId, clientSecret, stateId); break;
                case EProviders.fitbit: _client = new OAuth2ClientForFitbit(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.goodreads: _client = new OAuth10aClientForGoodreads(requestTokenUrl, loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, redirectUrl, stateId, prompt, onOAuth10aTokenUpdate); break;
                case EProviders.echoidp: _client = new OAuth2ClientForEchoIdP(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.planningcenter: _client = new OAuth2ClientForPlanningCenter(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.shopify: _client = new OAuth2ClientForShopify(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.yammer: _client = new OAuth2ClientForYammer(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.wordpress: _client = new OAuth2ClientForWordpress(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.yahoo: _client = new OAuth2ClientForYahoo(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.pingone: _client = new OAuth2ClientForPingOne(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.twitch: _client = new OAuth2ClientForTwitch(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.foursquare: _client = new OAuth2ClientForFoursquare(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.amazon: _client = new OAuth2ClientForAmazon(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.uber: _client = new OAuth2ClientForUber(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.wix: _client = new OAuth2ClientForWix(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.salesforce: _client = new OAuth2ClientForSalesforce(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.spotify: _client = new OAuth2ClientForSpotify(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.withings: _client = new OAuth2ClientForWithings(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.discord: _client = new OAuth2ClientForDiscord(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.bitbucket: _client = new OAuth2ClientForBitbucket(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, emailUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.strava: _client = new OAuth2ClientForStrava(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.instagram: _client = new OAuth2ClientForInstagram(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.paypal: _client = new OAuth2ClientForPaypal(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
                case EProviders.keycloak: _client = new OAuth2ClientForKeycloak(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt, hint); break;
            }
		}

        public async Task AuthorizeAsync(HttpResponse response)
		{
           await _client.AuthorizeAsync(response);
        }

        public async Task GetTokenAsync(string code)
		{
            UserData = (UserData)await _client.GetUserDataAsync(await _client.GetTokenAsync(code, _state.OAuth10aToken, _state.OAuth10aTokenSecret)); ;
        }

        public async Task<string> RefreshTokenAsync(string token)
        {
            return await _client.RefreshTokenAsync(token);
        }

        public async Task RevokeTokenAsync(string token)
        {
            await _client.RevokeTokenAsync(token);
        }

        public bool CanRefreshToken { get { return _client.CanRefreshToken; } }
        public bool CanRevokeToken { get { return _client.CanRevokeToken; } }

        public IUserData UserData { get; private set; }
    }
}