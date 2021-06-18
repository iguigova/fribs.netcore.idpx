//using ComponentPro.Saml2;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Http.Extensions;
//using System;
//using System.Collections.Generic;

//namespace com.idpx.core.saml
//{
//	public class LogoutRequestHandler : core.LogoutRequestHandler
//	{
//		public LogoutRequestHandler(HttpRequest request, App app, Provider provider, IdPx.Data.MongoDb.Models.State state) : base(request, app, provider, state) { }

//		protected override void Logout(HttpResponse response)
//		{
//			//Workflow.CacheState(State); //ip todo what about here

//			LogoutRequest.Redirect(response, ReferrerUrl, State.id.ToString(), Security.ParseCertificate(Provider.service_provider_certificate)?.PrivateKey);
//		}

//		protected LogoutRequest _logoutRequest;
//		protected LogoutRequest LogoutRequest
//		{
//			get
//			{
//				if (_logoutRequest == null)
//				{
//					_logoutRequest = new LogoutRequest();
//					_logoutRequest.Id = Guid.NewGuid().ToString();
//					_logoutRequest.Destination = ReferrerUrl;
//					_logoutRequest.Issuer = new Issuer(Provider.client_id);
//					_logoutRequest.NameId = new NameId(State?.name_id ?? State.email_address);
//					_logoutRequest.SessionIndexes = new List<SessionIndex>() { new SessionIndex(State?.session_index) };

//					if (Provider.service_provider_certificate != null)
//					{
//						_logoutRequest.Sign(Security.ParseCertificate(Provider.service_provider_certificate));
//					}
//				}

//				return _logoutRequest;
//			}
//		}

//		protected override string ReturnUrl { get { return Web.GetSamlUrl(Web.GetResponseLogoutUrl(new Uri(_request.GetDisplayUrl())), SamlBindingUri.HttpPost, Provider.Dynamic?.IgnoreBindingQueryString?.Value ?? false); } }
//		protected override string ReferrerUrl { get { return Web.GetSamlUrl(Provider.logout_url, SamlBindingUri.HttpRedirect, Provider.Dynamic?.IgnoreBindingQueryString?.Value ?? false); } }
//	}
//}