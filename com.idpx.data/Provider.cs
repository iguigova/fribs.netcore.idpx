using Newtonsoft.Json;

namespace com.idpx.data
{
    public class Provider
    {
        public string Id { get; set; }

        public EProviders Type { get; set; }
        public string RequestTokenUrl { get; set; } // oauth 1.0a
        public string LoginUrl { get; set; }
        public string TokenUrl { get; set; }
        public string RevokeTokenUrl { get; set; }
        public string DataUrl { get; set; }
        public string EmailUrl { get; set; }
        public string Scopes { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Prompt { get; set; } // force authentication feature

        public string ServiceProviderCertificate { get; set; }  // saml
        public string IdentityProviderCertificate { get; set; } // saml

        public bool IgnoreBindingQueryString {get; set; }

        public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}
