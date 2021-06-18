using com.idpx.core.oauth.data;

namespace com.idpx.core.oauth.clients
{
    public class OAuthApiClient : ApiClient
    {
        protected string _stateId;
        protected IUserData _userData;

        public OAuthApiClient(string stateId) : base()
        {
            _stateId = stateId;
        }
    }
}