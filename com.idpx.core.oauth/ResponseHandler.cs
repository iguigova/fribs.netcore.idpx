using com.idpx.data;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace com.idpx.core.oauth
{
	public class ResponseHandler : core.ResponseHandler
	{
		protected readonly Provider _provider;
		protected readonly State _state;
		private readonly string _code;

        public ResponseHandler(Provider provider, State state, string code = null) : base()
		{
			_provider = provider;
			_state = state;
			_code = code;
        }

		protected OAuthWorkflow _oauthWorkflow; 

		protected override string EmailAddress { get { return _oauthWorkflow?.UserData.Email; } }
		protected override string FirstName { get { return _oauthWorkflow?.UserData.FirstName; } }
		protected override string LastName { get { return _oauthWorkflow?.UserData.LastName; } }
		protected override string Avatar { get { return _oauthWorkflow?.UserData.Avatar; } }
		protected override string UID { get { return _oauthWorkflow?.UserData.UID; } }
        protected override Dictionary<string, string> Claims { get { return _oauthWorkflow?.UserData.Claims; } }
        protected override bool AuthSucceeded { get { return _oauthWorkflow?.UserData.AuthSucceeded ?? false; } }
		protected override string Error { get { return _oauthWorkflow?.UserData.Error; } }
		protected override string ErrorMessage { get { return _oauthWorkflow?.UserData.ErrorMessage; } }
		protected override string AccessToken { get { return _oauthWorkflow?.UserData.AccessToken; } }
		protected override string AccessTokenType { get { return _oauthWorkflow?.UserData.TokenType; } }
		protected override long? AccessTokenExpiresIn { get { return _oauthWorkflow?.UserData.ExpiresIn; } }
        protected override string RefreshToken { get { return _oauthWorkflow?.UserData.RefreshToken; } }
		protected override long? RefreshTokenExpiresIn { get { return _oauthWorkflow?.UserData.RefreshTokenExpiresIn; } }
		protected override bool CanRefreshToken { get { return _oauthWorkflow?.CanRefreshToken ?? false; } }
		protected override bool CanRevokeToken { get { return _oauthWorkflow?.CanRevokeToken ?? false; } }

		protected override async Task ValidateAsync()
		{
		}

		public override async Task ProcessAsync(HttpResponse response)
		{
			await base.ProcessAsync(response);

			_oauthWorkflow = new OAuthWorkflow(_provider, _state);

			await _oauthWorkflow.GetTokenAsync(_code ?? string.Empty);
		}
	}
}