//using com.idpx.data;
//using fribs.netcore.cache;
//using Microsoft.AspNetCore.Http;
//using MongoDB.Bson;
//using System;
//using System.Runtime.Serialization;
//using System.Threading.Tasks;

//namespace com.idpx.services
//{
//	// [Route("/authorize", "GET")]
//	// [Route("/authorize", "POST")]
//	[DataContract]
//	public class Authorize
//	{
//		[DataMember(IsRequired = false)]
//		public string email_address { get; set; }

//		[DataMember(IsRequired = false)]
//		public string provider_id { get; set; }

//		[DataMember(IsRequired = false)]
//		public string redirect_url { get; set; }
//	}

//	public class RequestService
//	{
//		protected ICache _cache;

//		public async Task Any(Authorize request)
//		{
//			void onException(Exception ex)
//			{
//				throw ex;
//			}

//			var stateId = ObjectId.GenerateNewId().ToString();
//			var state = _cache.GetCached<States, State>(() =>
//			{
//				return new State() { Id = stateId, ProviderId = request.provider_id};
//			}, States.TTL, onException, stateId);

//            var provider = GetProvider(state.ProviderId);

//            var identityProvider = provider.provider;
//            var providerProtocol = provider.protocol;

//            var originalRequest = Request.OriginalRequest as HttpRequest;
//            var originalResponse = Response.OriginalResponse as HttpResponse;

//            if (identityProvider == EProviders.dwolla)
//            {
//				var responseHandler = new core.oauth.ResponseHandler(provider, state);
//                await responseHandler.ProcessAsync(originalResponse);
//            }
//            else
//            {
//                var requestHandler = providerProtocol == EProtocols.oauth
//                    ? new core.oauth.RequestHandler(originalRequest, provider, state, request.redirect_url, request.email_address)
//                    : new core.saml.RequestHandler(originalRequest, provider, state, request.redirect_url, request.email_address);

//                await requestHandler.RedirectAsync(originalResponse);
//            }
//        }
//	}
//}
