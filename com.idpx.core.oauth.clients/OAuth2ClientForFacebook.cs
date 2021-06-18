using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;
using com.idpx.core.oauth.data;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForFacebook: OAuth2Client<UserData>, IOAuthClient
    {
        public OAuth2ClientForFacebook(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
            if (!string.IsNullOrEmpty(_prompt))
            {
                _prompt = $"{_prompt}{$"&auth_nonce={Guid.NewGuid()}"}";
            }
        }

        // NOTE: https://logonlabs.atlassian.net/browse/API-748
        // NOTE: The following code works but has been disabled for now as part of https://logonlabs.atlassian.net/browse/API-832
        //private string LongLivedTokenRequest(string token) { return $"grant_type=fb_exchange_token&fb_exchange_token={token}&client_id={_clientId}&client_secret={_clientSecret}"; }

        //public override string GetToken(string code, params string[] state)
        //{
        //    if (_includeAuthDataInResponse)
        //    {
        //        // Only do this when IncludeAuthDataInResponse is set to true

        //        var data = JsonConvert.DeserializeAnonymousType(base.GetToken(code, state), new { access_token = (string)null }); ;

        //        return Post(_tokenUrl, LongLivedTokenRequest(data.access_token));
        //    }

        //    return base.GetToken(code, state);
        //}


        // https://stackoverflow.com/questions/12804231/c-sharp-equivalent-to-hash-hmac-in-php
        private static string HashHmac(string message, string secret)
        {
            Encoding encoding = Encoding.UTF8;
            using (var hmac = new HMACSHA256(encoding.GetBytes(secret)))
            {
                var msg = encoding.GetBytes(message);
                var hash = hmac.ComputeHash(msg);

                //return BitConverter.ToString(hash).ToLower().Replace("-", string.Empty);
                return ByteToString(hash);
            }
        }

        private static string ByteToString(byte[] buff)
        {
            string sbinary = "";
            for (int i = 0; i < buff.Length; i++)
                sbinary += buff[i].ToString("X2"); /* hex format */
            return sbinary;
        }

        public override string DataRequest { get { return $"{_dataUrl}&appsecret_proof={HashHmac(_userData.AccessToken, _clientSecret).ToLower()}"; } }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            #region Sample Response
            // {\"email\":\"developer\\u0040logonlabs.com\",\"first_name\":\"Devo\",\"last_name\":\"LogonLabs\",\"id\":\"166102127910347\"}

            //{ { "email": "voccggcroo_1609874676@tfbnw.net",  "first_name": "Linda",  "last_name": "Baostein",  "picture": { "data": { "height": 50,      "is_silhouette": false,      "url": "https://platform-lookaside.fbsbx.com/platform/profilepic/?asid=102163515173074&height=50&width=50&ext=1612469616&hash=AeSsgpRMV1wjvylzcaQ",      "width": 50    } },  "id": "102163515173074"} }
            #endregion

            _userData.ParseUserData(data?.id?.Value.ToString(), data?.email?.Value?.Replace("\\u0040", "@"), data?.first_name?.Value, data?.last_name?.Value, data?.picture?.data?.url?.Value);
        }

        public override HttpContent RefreshTokenRequest(string token) { return null; }
    }
}