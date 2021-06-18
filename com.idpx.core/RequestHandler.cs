using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace com.idpx.core
{
	public abstract class RequestHandler : IRequestHandler
	{
		protected HttpRequest _request;
		protected virtual string ReferrerUrl { get; private set; }  // The url back to the client
		protected virtual string ReturnUrl { get; private set; } // the url back to the server

		protected RequestHandler(HttpRequest request)
		{
			_request = request;

			ReferrerUrl = Web.GetReferrerUrl(_request);
			ReturnUrl = Web.GetReturnUrl(new Uri(_request.GetDisplayUrl()));
		}

		public abstract Task RedirectAsync(HttpResponse response);
	}
}
