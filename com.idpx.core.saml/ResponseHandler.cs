using ComponentPro.Saml2;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using com.idpx.data;

namespace com.idpx.core.saml
{
	public class ResponseHandler : core.ResponseHandler
	{
		protected HttpRequest _request;
		protected readonly Provider _provider;
		protected readonly State _state;

		public ResponseHandler(HttpRequest request, Provider provider, State state) 
		{
			_request = request;
			_provider = provider;
			_state = state;
		}

		//public override string StateId { get { return AuthnResponse.RelayState; } } //ip todo i don't think we actually use this property

        private ComponentPro.Saml2.Response _authnResponse;
        private ComponentPro.Saml2.Response AuthnResponse
        {
            get
            {
                if (_authnResponse == null)
                {
                    _authnResponse = ComponentPro.Saml2.Response.Create(_request);
                }

                return _authnResponse;
            }
        }

        private Assertion Assertion { get { return ((Assertion)AuthnResponse.Assertions[0]); } }

		protected override string NameId { get { return Assertion.Subject?.NameId?.NameIdentifier; } }
		protected override string EmailAddress { get { return GetTrimmedAttributeValue(Assertion, "email"); } }
		protected override string FirstName { get { return GetTrimmedAttributeValue(Assertion, "first_name"); } }
		protected override string LastName { get { return GetTrimmedAttributeValue(Assertion, "last_name"); } }
		protected override string Avatar { get { return GetTrimmedAttributeValue(Assertion, "avatar"); } }
		protected override string UID { get { return GetTrimmedAttributeValue(Assertion, "uid"); } }

        protected override Dictionary<string, string> Claims { get { return GetAllAttributeValues(Assertion); /*GetArrayedAttributeValue(Assertion, "Groups")*/; } }

        protected override bool AuthSucceeded { get { return AuthnResponse.IsSuccess(); } }
		protected override string Error { get { return AuthnResponse.Status.StatusCode?.Code; } }
		protected override string ErrorMessage { get { return AuthnResponse.Status.StatusMessage?.Message; } }
		protected override string SessionIndex { get { return Assertion.AuthenticationStatements[0]?.SessionIndex; } }     // http://xacmlinfo.org/2013/06/28/how-saml2-single-logout-works/

		protected string GetTrimmedAttributeValue(Assertion assertion, string name)
		{
			return assertion.GetAttributeValue(name)?.Trim();
		}

        protected string GetArrayedAttributeValue(Assertion assertion, string name)
        {
            return GetAttributeValue(assertion.GetAttributes(name).SelectMany(s => s.Values).ToList());
        }

        protected Dictionary<string, string> GetAllAttributeValues(Assertion assertion)
        {
            var claims = new Dictionary<string, string>();

            foreach(var statement in assertion.AttributeStatements)
            {
                foreach(var attribute in statement.Attributes)
                {
                    var attr = (ComponentPro.Saml2.Attribute)attribute;

					if (claims.ContainsKey(attr.Name))
					{
						claims[attr.Name] = GetAttributeValue(attr.Values);
					}
					else
					{
						claims.Add(attr.Name, GetAttributeValue(attr.Values));
					}                    
                }
            }

            return claims;
        }

        protected string GetAttributeValue(IList<AttributeValue> values)
        {
            return (values != null) ? string.Join(',', values.Select(s => s.Data?.ToString())) : null;
        }

		protected override async Task ValidateAsync()
		{
			//We are no longer allowing for unsigned SAML
			if (!AuthnResponse.IsSigned())
			{				
			}

			if (string.IsNullOrWhiteSpace(_provider.IdentityProviderCertificate))
			{
				throw new Exception("The SAML response is signed but identity_provider_certificate is not configured.");
			}
			else
			{
				var cert = Security.ParseCertificate(_provider.IdentityProviderCertificate);

				if (cert == null)
				{
					throw new Exception("The SAML request cert failed to parse.");
				}

				//Verify the request with the ip cert (This part is ambigious and needs more research)
				if (!AuthnResponse.Validate(cert))
				{
					throw new ApplicationException("The SAML response signature failed to verify.");
				}
				
				//Verify the request cert's thumbprint
				//if (requestCert.Thumbprint != ipCert.Thumbprint)
				//{
				//	throw new ApplicationException($"Invalid thumbprint ({requestCert.Thumbprint} does not match: {ipCert.Thumbprint})");
				//}
			}
		}
	}
}