using com.idpx.core.oauth.data;
using System.Threading.Tasks;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForUber : OAuth2Client<UserData>, IOAuthClient
    {
        public OAuth2ClientForUber(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
        }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            #region Sample Response
            /*
             * https://developer.uber.com/docs/riders/references/api/v1.2/me-get
            {
              "picture": "https://d1w2poirtb3as9.cloudfront.net/f3be498cb0bbf570aa3d.jpeg",
              "first_name": "Uber",
              "last_name": "Developer",
              "uuid": "f4a416e3-6016-4623-8ec9-d5ee105a6e27",
              "rider_id": "8OlTlUG1TyeAQf1JiBZZdkKxuSSOUwu2IkO0Hf9d2HV52Pm25A0NvsbmbnZr85tLVi-s8CckpBK8Eq0Nke4X-no3AcSHfeVh6J5O6LiQt5LsBZDSi4qyVUdSLeYDnTtirw==",
              "email": "uberdevelopers@gmail.com",
              "mobile_verified": true,
              "promo_code": "uberd340ue"
            }
             */
            #endregion

            _userData.ParseUserData(data?.uuid?.Value.ToString(), data?.email?.Value, data?.first_name?.Value, data?.last_name?.Value, data?.picture?.Value);
        }
    }
}