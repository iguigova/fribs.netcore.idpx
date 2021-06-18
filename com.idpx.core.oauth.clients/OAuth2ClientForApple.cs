using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Jose;
using Newtonsoft.Json;
using com.idpx.core.oauth.data;
using System.Threading.Tasks;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForApple: OAuth2Client<UserDataForApple>, IOAuthClient
    {
        public OAuth2ClientForApple(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
            var secrets = JsonConvert.DeserializeAnonymousType(clientSecret, new { TeamId = string.Empty, KeyId = string.Empty, Key = string.Empty, Audience = string.Empty });

            var iat = Math.Round((DateTime.UtcNow.AddMinutes(-1) - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds, 0);
            var exp = Math.Round((DateTime.UtcNow.AddMinutes(30) - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds, 0);

            var claims = new Dictionary<string, object>()
                    {
                        { "iat", iat },
                        { "exp", exp },
                        { "iss", secrets.TeamId },
                        { "aud", secrets.Audience },
                        { "sub", _clientId }
                    };

            var headers = new Dictionary<string, object>()
                    {
                        { "alg", "ES256" },
                        { "kid", secrets.KeyId }
                    };

            var keyString = secrets.Key; //"the content of my .p8 downloaded private key, when I created the key in https://developer.apple.com/account/ios/authkey/create";

            //this works on Windows machines but not linux
            //var privateKey = CngKey.Import(Convert.FromBase64String(keyString), CngKeyBlobFormat.Pkcs8PrivateBlob);
            
            //platform agnostic
            var dats = Convert.FromBase64String(keyString);
            var ecdsa = ECDsa.Create();
            if (ecdsa != null) ecdsa.ImportPkcs8PrivateKey(dats, out var result);

            _clientSecret = JWT.Encode(claims, ecdsa, JwsAlgorithm.ES256, headers);

        }

        public override string AuthorizeRequest { get { return $"{_loginUrl}?client_id={_clientId}&state={_statePackageEncoded}&response_type=code&response_mode=form_post&redirect_uri={_redirectUrl}&scope={_scope}"; } }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            // nothing to do here
        }
    }
}