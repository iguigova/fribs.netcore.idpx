using com.idpx.core.oauth.data;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForFitbit : OAuth2Client<UserData>, IOAuthClient
    {
        public OAuth2ClientForFitbit(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
        }

        public override NameValueCollection TokenRequestHeaders { get { return new NameValueCollection() { { HttpRequestHeader.Authorization.ToString(), "Basic " + Security.EncodeToBase64(_clientId + ":" + _clientSecret) } }; } }
        public override NameValueCollection RefreshTokenRequestHeaders { get { return new NameValueCollection() { { HttpRequestHeader.Authorization.ToString(), "Basic " + Security.EncodeToBase64(_clientId + ":" + _clientSecret) } }; } }
        public override NameValueCollection RevokeTokenRequestHeaders(string token) { return new NameValueCollection() { { HttpRequestHeader.Authorization.ToString(), "Basic " + Security.EncodeToBase64(_clientId + ":" + _clientSecret) } }; }


        protected override async Task OnUserDataAsync(dynamic data)
        {
            // https://dev.fitbit.com/build/reference/web-api/user/
            /*
{
    "user": {
        "aboutMe":<value>,
        "avatar":<value>,
        "avatar150":<value>,
        "avatar640":<value>,
        "city":<value>,
        "clockTimeDisplayFormat":<12hour|24hour>,
        "country":<value>,
        "dateOfBirth":<value>,
        "displayName":<value>,
        "distanceUnit":<value>,
        "encodedId":<value>,
        "foodsLocale":<value>,
        "fullName":<value>,
        "gender":<FEMALE|MALE|NA>,
        "glucoseUnit":<value>,
        "height":<value>,
        "heightUnit":<value>,
        "locale":<value>,
        "memberSince":<value>,
        "offsetFromUTCMillis":<value>,
        "startDayOfWeek":<value>,
        "state":<value>,
        "strideLengthRunning":<value>,
        "strideLengthWalking":<value>,
        "timezone":<value>,
        "waterUnit":<value>,
        "weight":<value>,
        "weightUnit":<value>
    }
}
             */

            _userData.ParseUserData(data?.user.encodedId?.Value, data?.user.email?.Value, data?.user.firstName?.Value, data?.user.lastName?.Value, data?.user.avatar?.Value);
        }
    }
}