using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Web;
using System.Threading.Tasks;

namespace com.idpx.core
{
	public abstract class ResponseHandler : IResponseHandler
	{
        public ResponseHandler()
        {
        }

        protected abstract string EmailAddress { get; }
		protected abstract string FirstName { get; }
		protected abstract string LastName { get; }
        protected abstract string Avatar { get; }
        protected abstract string UID { get; }
        protected abstract Dictionary<string, string> Claims { get; }

		protected abstract bool AuthSucceeded { get; }
		protected abstract string Error { get; }
		protected abstract string ErrorMessage { get; }

		protected virtual string NameId { get { return null; } }
		protected virtual string SessionIndex { get { return null; } }

		protected virtual string AccessToken { get { return null; } }
		protected virtual string AccessTokenType { get { return null; } }
		protected virtual long? AccessTokenExpiresIn { get { return null; } }
        protected virtual string RefreshToken { get { return null; } }
        protected virtual long? RefreshTokenExpiresIn { get { return null; } }

        protected virtual bool CanRefreshToken { get { return false; } }
        protected virtual bool CanRevokeToken { get { return false; } }

        // protected virtual string Scopes { get { return Provider.scopes ?? Provider.Dynamic?.scopes?.ToString(); } }

		protected abstract Task ValidateAsync();

        protected virtual string GetRedirectUrl(string baseRedirectUrl)
        {
            var redirectUrl = new UriBuilder(HttpUtility.UrlDecode(baseRedirectUrl));

            redirectUrl.Query = ParseQuery(redirectUrl.Query);

            return redirectUrl.ToString();
        }

        protected virtual string ParseQuery(string querystring)
        {
            var query = HttpUtility.ParseQueryString(querystring);  // http://stackoverflow.com/questions/14517798/append-values-to-query-string

            query["token"] = Guid.NewGuid().ToString();

            return query.ToString();
        }

        public virtual async Task ProcessAsync(HttpResponse response)
        {
            await ValidateAsync();
        }
    }
}
