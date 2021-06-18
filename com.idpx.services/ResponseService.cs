//using com.idpx.data;
//using fribs.netcore.cache;
//using Microsoft.AspNetCore.Http;
//using System;
//using System.Runtime.Serialization;
//using System.Threading.Tasks;

//namespace com.idpx.services
//{
//	// [Route("/callback", "GET")]
//	// [Route("/callback", "POST")]
//	[DataContract]
//	public class Callback
//	{
//		[DataMember(IsRequired = false)]
//		public string code { get; set; }

//		[DataMember(IsRequired = false)]
//		public string user { get; set; }

//		[DataMember(IsRequired = false)]
//		public string state { get; set; }

//		[DataMember(IsRequired = false)]
//		public string session_state { get; set; }

//		[DataMember(IsRequired = false)]
//		public string RelayState { get; set; }

//		[DataMember(IsRequired = false)]
//		public string SAMLResponse { get; set; }

//		[DataMember(IsRequired = false)]
//		public string oauth_token { get; set; }

//		[DataMember(IsRequired = false)]
//		public string oauth_verifier { get; set; }

//		[DataMember(IsRequired = false)]
//		public string error { get; set; }

//		[DataMember(IsRequired = false)]
//		public string error_description { get; set; }
//	}

//	public class ResponseService
//    {
//		protected ICache _cache;

//		public async Task Any(Callback request)
//		{
//			if (!string.IsNullOrEmpty(request.error))
//			{
//				throw new Exception($"{request.error} - {request.error_description}");
//			}

//			void onException(Exception ex)
//			{
//				//Logger.Error(ex, stateId: _stateId)
//				throw ex;
//			}

//			var stateOrToken = request.state ?? request.RelayState ?? request.oauth_token;
			
//			var state = _cache.GetCached<States, State>(() =>
//			{
//				return (State)null;
//			}, States.TTL, onException, stateOrToken); ;

//			if (state == null)
//			{
//				throw new Exception($"State id {stateOrToken} not found");
//			}

//			var provider = GetProvider(state.ProviderId);

//			var originalRequest = Request.OriginalRequest as HttpRequest;
//			var originalResponse = Response.OriginalResponse as HttpResponse;

//			var responseHandler = provider.Protocol == EProtocols.oauth
//				? new core.oauth.ResponseHandler(provider, state, request.code ?? request.oauth_verifier)
//				: new core.saml.ResponseHandler(originalRequest, provider, state);

//			await responseHandler.ProcessAsync(originalResponse);
//		}
//	}
//}
