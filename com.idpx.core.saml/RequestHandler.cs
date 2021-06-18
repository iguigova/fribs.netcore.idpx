using com.idpx.data;
using ComponentPro.Saml2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace com.idpx.core.saml
{
	public class RequestHandler : core.RequestHandler
	{
		protected readonly Provider _provider;
		protected readonly State _state;
		protected readonly string _redirectUrl;
		protected readonly string _hint;

		public RequestHandler(HttpRequest request, Provider provider, State state, string redirectUrl, string hint = null) : base(request) 
		{ 
			_provider = provider;
			_state = state;
			_redirectUrl = redirectUrl;
			_hint = hint;
	}

		public override async Task RedirectAsync(HttpResponse response)
		{	
			AuthnRequest.Redirect(response, RedirectUrl, _state.Id, ServiceProviderCertificate?.PrivateKey);
		}

		protected X509Certificate2 ServiceProviderCertificate
		{
			get
			{
				return Security.ParseCertificate(_provider.ServiceProviderCertificate);
			}
		}

		protected AuthnRequest _authnRequest;
		protected AuthnRequest AuthnRequest
		{
			get
			{
				if (_authnRequest == null)
				{
					_authnRequest = new AuthnRequest
					{
						Id = string.Format("id{0}", Guid.NewGuid()),
						Destination = RedirectUrl,
						Issuer = new Issuer(_provider.ClientId),
						ForceAuthn = _provider.Prompt != null,
						NameIdPolicy = new NameIdPolicy(null, null, true),
						ProtocolBinding = SamlBindingUri.HttpPost,
						AssertionConsumerServiceUrl = ReturnUrl
					};

					// https://stackoverflow.com/questions/20074775/can-i-provide-the-username-to-use-in-a-saml-request-ad-fs
					if (!string.IsNullOrEmpty(_hint))
					{
						_authnRequest.Subject = new Subject(new NameId(_hint));
					}

					if (ServiceProviderCertificate != null)
					{
						var cert = ServiceProviderCertificate;
						if (cert == null)
						{
							throw new Exception($"Error parsing Service Provider Certificate for Provider {_provider.Id}");
						}
						_authnRequest.Sign(cert);
					}
				}

				return _authnRequest;
			}
		}

		protected override string ReturnUrl
		{
			get { return Web.GetSamlUrl(Web.GetReturnUrl(new Uri(_request.GetDisplayUrl())), SamlBindingUri.HttpPost, _provider.IgnoreBindingQueryString); }
		}
		
        protected string RedirectUrl
		{
			get { return Web.GetSamlUrl(_provider.LoginUrl, SamlBindingUri.HttpRedirect, _provider.IgnoreBindingQueryString); }
		}
	}
}