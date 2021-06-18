using com.idpx.core.oauth.data;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForEchoIdP : OAuth2Client<UserData>, IOAuthClient
    {
        private readonly string _clientData;

        public OAuth2ClientForEchoIdP(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null, string clientData = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
            _clientData = clientData;
        }

        public override async Task<IUserData> GetUserDataAsync(string data)
        {
	        _userData = new UserData(data);
	        
            await OnUserDataAsync(JsonConvert.DeserializeObject<dynamic>(_clientData));

            return _userData;
        }

        protected override async Task OnUserDataAsync(dynamic data)
        {
	        string echoUid = data?.echo_uid ?? "";
	        string email = data?.echo_email_address ?? "";
	        string fName = data?.echo_first_name ?? "";
	        string lName = data?.echo_last_name ?? "";
            _userData.ParseUserData(echoUid, email, fName, lName);
        }
    }
}