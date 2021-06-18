using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using com.idpx.core.oauth.data;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace com.idpx.core.oauth.clients
{
    public abstract class OAuth2Client<TUserData> : OAuthApiClient, IOAuthClient where TUserData : IUserData
    {
        // https://msdn.microsoft.com/en-us/library/azure/dn645542.aspx

        protected string _loginUrl;
        protected string _tokenUrl;
        protected string _dataUrl;
        protected string _revokeTokenUrl;
        protected string _redirectUrl;
        protected string _clientId;
        protected string _clientSecret;
        protected string _scope;
        protected string _prompt;
        protected string _hint;
        protected bool _includeAuthDataInResponse;
        protected string _statePackageEncoded;

        public OAuth2Client(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null) : base(stateId)
        {
            _loginUrl = loginUrl;
            _tokenUrl = tokenUrl;
            _revokeTokenUrl = revokeTokenUrl;
            _dataUrl = dataUrl;
            _redirectUrl = HttpUtility.UrlEncode(redirectUrl);
            _clientId = clientId;
            _clientSecret = HttpUtility.UrlEncode(clientSecret);
            _scope = HttpUtility.UrlEncode(scope);
            _prompt = prompt;
            _hint = HttpUtility.UrlEncode(hint); // encoding takes care of + sign in email addresses
        }

        public virtual string AuthorizeRequest { get { return $"{_loginUrl}?client_id={_clientId}&state={_statePackageEncoded}&response_type=code&redirect_uri={_redirectUrl}&scope={_scope}&login_hint={_hint}&{_prompt}"; } }

        public virtual async Task AuthorizeAsync(HttpResponse response)
        {
            response.RedirectTo(AuthorizeRequest);
        }

        //public virtual NameValueCollection TokenRequestHeaders { get { return new NameValueCollection() { { HttpRequestHeader.Authorization.ToString(), "Basic " + LLSecurity.EncodeToBase64(_clientId + ":" + _clientSecret) } }; } }
        public virtual NameValueCollection TokenRequestHeaders { get { return null; } }

        public virtual HttpContent TokenRequest(string code) 
        { 
            return new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "client_id", _clientId},
                { "client_secret", _clientSecret},
                { "redirect_uri", _redirectUrl}
            }); 
        }

        public async virtual Task<string> GetTokenAsync(string code, params string[] state)
        {
            return await PostAsync(_tokenUrl, TokenRequestHeaders, TokenRequest(code));
        }

        // public virtual NameValueCollection DataRequestHeaders { get { return new NameValueCollection() { { HttpRequestHeader.Authorization.ToString(), "Basic " + LLSecurity.EncodeToBase64(_clientId + ":" + _clientSecret) } }; } }
        public virtual NameValueCollection DataRequestHeaders { get { return new NameValueCollection() { { HttpRequestHeader.Authorization.ToString(), $"Bearer {_userData.AccessToken}" } }; } }

        public virtual string DataRequest { get { return _dataUrl; } }

        public virtual async Task<IUserData> GetUserDataAsync(string data)
        {
            _userData = (TUserData)Activator.CreateInstance(typeof(TUserData), data);

            await OnUserDataAsync(JsonConvert.DeserializeObject(!string.IsNullOrEmpty(DataRequest) ? await GetAsync(DataRequest, DataRequestHeaders) : data));

            return _userData;
        }

        protected abstract Task OnUserDataAsync(dynamic data);

        public bool CanRefreshToken { get { return RefreshTokenRequest(string.Empty) != null; } }

        public virtual NameValueCollection RefreshTokenRequestHeaders { get { return null; } }

        public virtual HttpContent RefreshTokenRequest(string token)
        {
            return new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", token },
                { "client_id", _clientId},
                { "client_secret", _clientSecret}
            });
        }

        public virtual async Task<string> RefreshTokenAsync(string token)
        {
            if (!CanRefreshToken)
            {
                throw new Exception($"Requested operation is not supported for {this.GetType().Name}");
            }

            try
            {
                return await PostAsync(_tokenUrl, RefreshTokenRequestHeaders, RefreshTokenRequest(token));
            }
            catch (Exception ex)
            {
                throw new Exception("There was an error."); // { OriginalException = ex }; // e.g., PlanningCenter returns 401 Unauthorized; we want to return 400 Bad Request for consistency
            }
        }

        public bool CanRevokeToken { get { return _revokeTokenUrl != null; } }

        public virtual NameValueCollection RevokeTokenRequestHeaders(string token) { return null; }

        public virtual HttpContent RevokeTokenRequest(string token)
        {
            return new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "token", token },
            });
        }

        public virtual async Task RevokeTokenAsync(string token)
        {
            if (!CanRevokeToken)
            {
                throw new Exception($"Requested operation is not supported for {this.GetType().Name}");
            }

            try
            {
                var response = JsonConvert.DeserializeAnonymousType(await PostAsync(_revokeTokenUrl, RevokeTokenRequestHeaders(token), RevokeTokenRequest(token)), new { ok = (bool?)null, error = (string)null });

                if (!(response?.ok ?? true) || response?.error != null)
                {
                    throw new Exception(response.error);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("There was an error."); // { OriginalException = ex }; // we want to return 400 Bad Request for consistency
            }
        }
    }
}