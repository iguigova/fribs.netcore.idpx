using com.idpx.data;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace com.idpx.core.oauth
{
	public class RequestHandler : core.RequestHandler
	{
		protected readonly Provider _provider;
		protected readonly State _state;
		protected readonly string _redirectUrl;
		protected readonly string _hint; 

        public RequestHandler(HttpRequest request, Provider provider, State state, string redirectUrl, string hint = null)
            : base(request)
        {
			_provider = provider;
			_state = state;
			_redirectUrl = redirectUrl;
			_hint = hint;
        }

		public override async Task RedirectAsync(HttpResponse response)
		{
			await new OAuthWorkflow(_provider, _state, _redirectUrl, _hint).AuthorizeAsync(response);
		}
	}
}